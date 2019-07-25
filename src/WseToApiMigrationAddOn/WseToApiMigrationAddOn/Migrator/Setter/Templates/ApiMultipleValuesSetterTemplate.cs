using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Tricentis.Automation.WseToApiMigrationAddOn.Helper;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Interfaces;
using Tricentis.Automation.WseToApiMigrationAddOn.Shared;
using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Templates {
    public abstract class ApiMultipleValuesSetterTemplate : IApiValueSetter {
        #region Properties

        protected virtual bool IsResponseTestStepValue { get; } = false;

        protected abstract string TqlToGetApiTestStepValue { get; }

        protected abstract string TqlToGetModuleAttributeInApiModule { get; }

        #endregion

        #region Public Methods and Operators

        public void Execute(XTestStep apiTestStep, XTestStep wseTestStep) {
            try {
                Dictionary<string, string> testStepKeyValueList = GetWseTestStepValueAsKeyValPair(wseTestStep);
                if (!testStepKeyValueList.Any()) return;

                foreach (var wseTestStepValue in testStepKeyValueList) {
                    //Extract value need to be filled in api test step
                    string wseValue = wseTestStepValue.Value;
                    string apiValue = String.Empty;
                    //Note: we ignore this step in case of response. 
                    if (!IsResponseTestStepValue) {
                        //get api value
                        apiValue =
                                GetValueInApiModule(apiTestStep.Module as ApiModule, wseTestStepValue.Key);
                        //if values match,do nothing.

                        if (!Regex.IsMatch(wseValue, @"\{.*\}")
                            && apiValue == CommonUtilities.RemoveExtraDoubleQuotes(wseValue)) return;
                    }

                    //if values does not match , Search if there is already a API xTestStepValue present.
                    XTestStepValue apiTeststepValue = GetOrCreateApiXTestStepValue(apiTestStep, wseTestStepValue);

                    //API xTestStepValue is present now. Assign Values to it now.
                    UpdateValueRange(apiTeststepValue, apiValue, wseValue);
                    apiTeststepValue.Value = wseTestStepValue.Value;
                    apiTeststepValue.ActionMode =
                            IsResponseTestStepValue ? XTestStepActionMode.Verify : XTestStepActionMode.Insert;
                }
            }
            catch (Exception e) {
                FileLogger.Instance.Error("Multiple value setter failed", e);
            }
        }

        #endregion

        #region Methods

        protected abstract XModuleAttribute CreateModuleAttribute(ApiModule apiModule,
                                                                  KeyValuePair<string, string> wseTestStepValue);

        protected abstract string GetValueInApiModule(ApiModule apiModule, string key);

        protected abstract Dictionary<string, string> GetWseTestStepValueAsKeyValPair(XTestStep testStep);

        protected abstract void UpdateValueRange(XTestStepValue apiTeststepValue, string apiValue, string wseValue);

        private XTestStepValue GetOrCreateApiXTestStepValue(XTestStep apiTestStep,
                                                            KeyValuePair<string, string> wseTestStepValue) {
            XTestStepValue apiTeststepValue =
                    (XTestStepValue)apiTestStep
                                    .Search(string.Format(TqlToGetApiTestStepValue, wseTestStepValue.Key))
                                    .FirstOrDefault();

            //if xTestStepValue not present,create it.
            if (apiTeststepValue == null) {
                //check if module attribute is present.
                var xModuleAttribute =
                        (XModuleAttribute)apiTestStep
                                          .Module.Search(
                                                  string.Format(TqlToGetModuleAttributeInApiModule,
                                                                wseTestStepValue.Key)).FirstOrDefault();
                //if not present, Create it.
                if (xModuleAttribute == null) {
                    XModuleAttribute m =
                            CreateModuleAttribute(apiTestStep.Module as ApiModule, wseTestStepValue);
                    apiTeststepValue = apiTestStep.CreateXTestStepValue(m);
                }
                //if already present, use it.
                else {
                    apiTeststepValue = apiTestStep.CreateXTestStepValue(xModuleAttribute);
                }
            }

            return apiTeststepValue;
        }

        #endregion
    }
}