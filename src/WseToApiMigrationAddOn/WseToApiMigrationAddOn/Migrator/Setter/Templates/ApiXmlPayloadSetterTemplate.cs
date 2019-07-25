using System;
using System.Collections.Generic;
using System.Linq;

using Tricentis.TCAPIObjects.Objects;

using WseToApiMigrationAddOn.Helper;
using WseToApiMigrationAddOn.Migrator.Handler;
using WseToApiMigrationAddOn.Migrator.Setter.Interfaces;
using WseToApiMigrationAddOn.Shared;

namespace WseToApiMigrationAddOn.Migrator.Setter.Templates {
    public abstract class ApiXmlPayloadSetterTemplate : IApiValueSetter, IPayloadSetter {
        #region Properties

        protected abstract string TqlToGetWseTestStepValue { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Creates module attributes for payload
        /// </summary>
        /// <param name="wseTestStepValue">TestStepValues of WSE Teststep</param>
        /// <param name="xpath">XPath and JsonPath for xml</param>
        /// <param name="parent">xModule and xModuleAttribute of ApiEngine</param>
        /// <param name="apiModule">ApiModule</param>
        /// <param name="parentTest">TestStep and TestStepValues for ApiTestStep</param>
        /// <param name="cardinality">Cardinality of WSE XModule</param>
        /// <param name="isArray">True if Wse Xmodule attributes is array</param>
        /// <returns>returns XtestTestValue of API</returns>
        public XTestStepValue CreateModuleAttributeForPayload(XTestStepValue wseTestStepValue,
                                                              string xpath,
                                                              dynamic parent,
                                                              ApiModule apiModule,
                                                              dynamic parentTest,
                                                              string cardinality,
                                                              bool isArray = false) {
            return BusinessParameterHandler.CreateBusinessParameterInTosca(wseTestStepValue,
                                                                           xpath,
                                                                           parent,
                                                                           apiModule,
                                                                           BusinessParameterPathTypes.XPATH,
                                                                           parentTest,
                                                                           cardinality,
                                                                           isArray);
        }

        public void Execute(XTestStep apiTestStep, XTestStep wseTestStep) {
            FillPayload(apiTestStep, wseTestStep);
        }

        private void FillPayload(XTestStep apiTestStep,
                                XTestStep wseTestStep) {
            var tcObjects = wseTestStep.Search(TqlToGetWseTestStepValue)
                                       .Cast<XTestStepValue>().ToList();

            FillPayloadInternal(tcObjects,
                                apiTestStep,
                                wseTestStep,
                                string.Empty,
                                (ApiModule)apiTestStep.Module,
                                apiTestStep);
        }

        /// <summary>
        /// Fills XML payload using recursive call to method
        /// </summary>
        /// <param name="tcObjects">XML payload testStepValues</param>
        /// <param name="apiTestStep">apiTestStep</param>
        /// <param name="wseTestStep">wseTestStep</param>
        /// <param name="xPath">xpTah</param>
        /// <param name="parent">xModule and xModuleAttribute of API</param>
        /// <param name="parentTest">XTestStep and xTestStepValue of ApiTesstep</param>
        public virtual void FillPayloadInternal(IEnumerable<XTestStepValue> tcObjects,
                                                XTestStep apiTestStep,
                                                XTestStep wseTestStep,
                                                string xPath,
                                                dynamic parent,
                                                dynamic parentTest) {
            XTestStepValue wseTestStepValue = null;
            string tXpathFormat = "/*[local-name()='{0}']";
            if (!tcObjects.Any()) return;
            var firstObject = tcObjects.First();
            var lastObject = tcObjects.Last();
            foreach (var tcObject in tcObjects.Where(x => x.ModuleAttribute.BusinessType == "XmlElement")) {
                bool isArray = false;
                try {
                    wseTestStepValue = tcObject;
                    string currXpath;
                    if (firstObject.Name == lastObject.Name && !ReferenceEquals(firstObject, lastObject)) {
                        currXpath = xPath + string.Format(tXpathFormat,
                                                          wseTestStepValue.Name);
                        isArray = true;
                    }
                    else {
                        currXpath = xPath + string.Format(tXpathFormat, wseTestStepValue.Name);
                    }

                    XTestStepValue apiTestStepValue = CreateModuleAttributeForPayload(wseTestStepValue,
                                                                                      currXpath,
                                                                                      parent,
                                                                                      (ApiModule)apiTestStep.Module,
                                                                                      parentTest,
                                                                                      wseTestStepValue
                                                                                              .ModuleAttribute
                                                                                              ?.Cardinality,
                                                                                      isArray
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