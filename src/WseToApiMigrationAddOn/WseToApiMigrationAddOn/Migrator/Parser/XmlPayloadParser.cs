using System;
using System.Linq;
using System.Xml.Linq;

using Tricentis.Automation.WseToApiMigrationAddOn.Helper.Xml;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Parser.Interfaces;
using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Parser {
    /// <summary>
    /// Extracts payload in string format from WSE artifacts
    /// </summary>
    public class XmlPayloadParser : IPayloadParser {
        #region Public Methods and Operators

        /// <summary>
        /// Gets payload from Wse Module 
        /// </summary>
        /// <param name="wseModule">WSE Module</param>
        /// <param name="tql"> Tql to get root xml element from WSE Module Request or Response XModuleAttribute  </param>
        /// <returns></returns>
        public string Parse(XModule wseModule, string tql = "") {
            if (wseModule == null) return string.Empty;
            try {
                var rootAttribute = wseModule.Search(tql).Cast<XModuleAttribute>().FirstOrDefault();
                return Parse(rootAttribute);
            }
            catch (Exception) {
                // do nothing as this could happen possibly, just move on with the other attributes
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets payload from WSE TestStep
        /// </summary>
        /// <param name="xTestStep">WSE TestStep</param>
        /// <param name="tql">Tql to get root xml element from WSE Request or Response XTestStepValue</param>
        /// <returns></returns>
        public string Parse(XTestStep xTestStep, string tql = "") {
            if (xTestStep == null) return string.Empty;
            try {
                var rootAttribute = xTestStep.Search(tql).Cast<XTestStepValue>().FirstOrDefault();
                return Parse(rootAttribute);
            }
            catch (Exception) {
                // do nothing as this could happen possibly, just move on with the other attributes
            }

            return string.Empty;
        }

        #endregion

        #region Methods

        private string Parse(XTestStepValue root) {
            string payload = string.Empty;
            if (root == null) return payload;
            XElement xmlStructure = XmlHelper.ConstructXmlStructure(root);

            xmlStructure = XmlHelper.RemoveNodeFromXml(xmlStructure, "Fault", "Body");
            payload = Convert.ToString(xmlStructure);
            return payload;
        }

        private string Parse(XModuleAttribute root) {
            string payload = string.Empty;
            if (root == null) return payload;

            XElement xmlStructure =
                    XmlHelper.ConstructXmlStructure(root);
            xmlStructure = XmlHelper.RemoveNodeFromXml(xmlStructure, "Fault", "Body");
            payload = Convert.ToString(xmlStructure);
            return payload;
        }

        #endregion
    }
}