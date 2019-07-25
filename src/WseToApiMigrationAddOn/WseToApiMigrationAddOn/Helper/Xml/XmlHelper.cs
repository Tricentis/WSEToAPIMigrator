using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

using Tricentis.TCAPIObjects.Objects;

namespace WseToApiMigrationAddOn.Helper.Xml {
    /// <summary>
    /// Creates Xml structure from Wse XModuleAttributes and XTestStepValues for API Engine Payload section
    /// </summary>
    public static class XmlHelper {
        #region Public Methods and Operators
        /// <summary>
        /// Creates Xml structure from Wse XModuleAttributes for API Modules Payload section
        /// </summary>
        /// <param name="xModuleAttribute">Wse XModuleAttributes</param>
        /// <param name="xmlStructure">XElement used to create Payload Structure</param>
        /// <param name="parent">XElement used to identify and create parent nodes in xml structure </param>
        /// <returns>xml structure for API Modules Payload Section</returns>
        public static XElement ConstructXmlStructure(XModuleAttribute xModuleAttribute,
                                                     XElement xmlStructure = null,
                                                     XElement parent = null) {
            XNamespace xNamespace = GetNamespace(xModuleAttribute);
            List<XAttribute> xAttribute = GetXAttribute(xModuleAttribute);
            if (xmlStructure == null) {
                xmlStructure =
                        new XElement(xNamespace != null ? xNamespace + xModuleAttribute.Name : xModuleAttribute.Name,
                                     xAttribute,
                                     xModuleAttribute.DefaultValue == "{NULL}" ? "" : xModuleAttribute.DefaultValue);
                parent = xmlStructure;
            }
            else if (xmlStructure == parent) {
                parent = new XElement(
                        xNamespace != null ? xNamespace + xModuleAttribute.Name : xModuleAttribute.Name,
                        xAttribute,
                        xModuleAttribute.DefaultValue == "{NULL}" ? "" : xModuleAttribute.DefaultValue);
                xmlStructure.Add(parent);
            }
            else {
                var e = new XElement(
                        xNamespace != null ? xNamespace + xModuleAttribute.Name : xModuleAttribute.Name,
                        xAttribute,
                        xModuleAttribute.DefaultValue == "{NULL}" ? "" : xModuleAttribute.DefaultValue);
                parent?.Add(e);
                parent = e;
            }

            IEnumerable<XModuleAttribute> childSubAttributes =
                    xModuleAttribute.SubAttributes.Where(x => x.BusinessType == "XmlElement");
            foreach (var childSubAttribute in childSubAttributes) {
                xmlStructure = ConstructXmlStructure(childSubAttribute, xmlStructure, parent);
            }

            return xmlStructure;
        }
        /// <summary>
        /// Creates Xml structure from Wse XTestStepValue for API Modules Payload section
        /// </summary>
        /// <param name="xTestStepValue">Wse XTestStepValues</param>
        /// <param name="xmlStructure">XElement used to create Payload Structure</param>
        /// <param name="parent">XElement used to identify and create parent nodes in xml structure </param>
        /// <returns>xml structure for API Modules Payload Section</returns>
        public static XElement ConstructXmlStructure(XTestStepValue xTestStepValue,
                                                     XElement xmlStructure = null,
                                                     XElement parent = null) {
            XNamespace xNamespace = GetNamespace(xTestStepValue.ModuleAttribute);
            List<XAttribute> xAttribute = GetXAttribute(xTestStepValue.ModuleAttribute);
            if (xmlStructure == null) {
                xmlStructure =
                        new XElement(xNamespace != null ? xNamespace + xTestStepValue.Name : xTestStepValue.Name,
                                     xAttribute,
                                     xTestStepValue.Value == "{NULL}" ? "" : xTestStepValue.Value);
                parent = xmlStructure;
            }
            else if (xmlStructure == parent) {
                parent = new XElement(
                        xNamespace != null ? xNamespace + xTestStepValue.Name : xTestStepValue.Name,
                        xAttribute,
                        xTestStepValue.Value == "{NULL}" ? "" : xTestStepValue.Value);
                xmlStructure.Add(parent);
            }
            else {
                var e = new XElement(
                        xNamespace != null ? xNamespace + xTestStepValue.Name : xTestStepValue.Name,
                        xAttribute,
                        xTestStepValue.Value == "{NULL}" ? "" : xTestStepValue.Value);
                parent?.Add(e);
                parent = e;
            }

            var childSubAttributes =
                    xTestStepValue.SubValues;
            foreach (var childSubAttribute in childSubAttributes.Where(
                    x => x.ModuleAttribute.BusinessType == "XmlElement")) {
                xmlStructure = ConstructXmlStructure(childSubAttribute, xmlStructure, parent);
            }

            return xmlStructure;
        }
        /// <summary>
        /// Removes a child node from its parent node in a Xml Structure
        /// </summary>
        /// <param name="xmlStructure">payload in xml</param>
        /// <param name="nodeName">Name of child Xml Node</param>
        /// <param name="parentNodeName">Name of Parent Xml Node</param>
        /// <returns>xml structure for API Modules Payload Section</returns>
        public static XElement RemoveNodeFromXml(XElement xmlStructure, string nodeName, string parentNodeName) {
            if (xmlStructure != null) {
                var xElements = xmlStructure.DescendantsAndSelf().Where(x => x.Name.LocalName == nodeName);
                if (xElements != null && xElements.Any())
                    foreach (var xElement in xElements)
                        if (xElement.Parent.Name.LocalName == parentNodeName) {
                            xElement.Remove();
                            break;
                        }
            }

            return xmlStructure;
        }

        #endregion

        #region Methods

        private static XNamespace GetNamespace(XModuleAttribute xdoc) {
            XParam namespaceXparam = xdoc.XParams.FirstOrDefault(x => x.Name == "NamespaceURI");
            return namespaceXparam?.Value;
        }

        private static List<XAttribute> GetXAttribute(XModuleAttribute xModuleAttribue) {
            List<XAttribute> lstXAttributes = null;
            var xMAttributes = xModuleAttribue.SubAttributes.Where(x => x.BusinessType == "XmlAttribute");
            foreach (var mAttribute in xMAttributes) {
                if (!mAttribute.Name.Contains("xmlns")) {
                    var regexItem = new Regex("^[a-zA-Z0-9]*$");
                    if (regexItem.IsMatch(mAttribute.Name)) {
                        if (lstXAttributes == null) {
                            lstXAttributes = new List<XAttribute> {
                                    new XAttribute(mAttribute.Name, mAttribute.DefaultValue)
                            };
                        }
                        else {
                            lstXAttributes.Add(new XAttribute(mAttribute.Name, mAttribute.DefaultValue));
                        }
                    }
                }
            }

            return lstXAttributes;
        }

        #endregion
    }
}