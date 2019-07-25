using System;
using System.Collections.Generic;
using System.Linq;

using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Interfaces;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Templates;
using Tricentis.Automation.WseToApiMigrationAddOn.Shared;
using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.RequestSetter {
    /// <summary>
    /// Extracts and sets Request payload of XML of wse into API teststep.
    /// </summary>
    public class ApiRequestXmlPayloadSetter : ApiXmlPayloadSetterTemplate, IApiRequestValueSetter {
        #region Properties

        protected override string TqlToGetWseTestStepValue =>
                "=>SUBPARTS:XTestStepValue[Name==\"Request\"]->SUBPARTS";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Recursive call to method to fill request payload for XML from WSE to API
        /// </summary>
        /// <param name="tcObjects">whole XML payload in form of XTestStep value</param>
        /// <param name="apiTestStep">apiTestStep to create module attribute under its module</param>
        /// <param name="wseTestStep"></param>
        /// <param name="xPath">Xpath of wseTestStepValue</param>
        /// <param name="parent">xmodule and xmoduleattribute of Api</param>
        /// <param name="parentTest">teststep and teststepvalue for ApiTestStep</param>
        public override void FillPayloadInternal(IEnumerable<XTestStepValue> tcObjects,
                                                 XTestStep apiTestStep,
                                                 XTestStep wseTestStep,
                                                 string xPath,
                                                 dynamic parent,
                                                 dynamic parentTest) {
            XTestStepValue wseTestStepValue = null;
            string tXpathFormat = "/*[local-name()='{0}']{1}";
            if (!tcObjects.Any()) return;
            var firstObject = tcObjects.First();
            var lastObject = tcObjects.Last();
            var index = 1;
            foreach (var tcObject in tcObjects.Where(x => x.ModuleAttribute.BusinessType == "XmlElement")) {
                try {
                    wseTestStepValue = tcObject;
                    string currXpath;
                    if (firstObject.Name == lastObject.Name && !ReferenceEquals(firstObject, lastObject)) {
                        currXpath = xPath + string.Format(tXpathFormat,
                                                          wseTestStepValue.Name,
                                                          $"[{Convert.ToString(index++)}]");
                    }
                    else {
                        currXpath = xPath + string.Format(tXpathFormat,
                                                          wseTestStepValue.Name,
                                                          $"[{Convert.ToString(index)}]");
                    }

                    XTestStepValue apiTestStepValue = CreateModuleAttributeForPayload(wseTestStepValue,
                                                                                      currXpath,
                                                                                      parent,
                                                                                      (ApiModule)apiTestStep.Module,
                                                                                      parentTest,
                                                                                      "0-1",
                                                                                      false
                    );
                    FillPayloadInternal(tcObject.SubValues,
                                        apiTestStep,
                                        wseTestStep,
                                        currXpath,
                                        apiTestStepValue.ModuleAttribute,
                                        apiTestStepValue);
                }
                catch (Exception ex) {
                    FileLogger.Instance.Error(
                            $"Error occurred while creating Business Parameter for WSE TestStep : 'Name: {wseTestStep?.Name}' NodePath:'{wseTestStep?.NodePath}' TestStepValue:'{wseTestStepValue?.Name}' Exception:'{ex.ToString()}'");
                }
            }
        }

        #endregion
    }
}