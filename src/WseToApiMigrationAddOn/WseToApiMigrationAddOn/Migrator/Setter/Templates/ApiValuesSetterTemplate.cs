using System;
using System.Linq;
using System.Text.RegularExpressions;

using Tricentis.TCAPIObjects.Objects;

using WseToApiMigrationAddOn.Migrator.Setter.Interfaces;
using WseToApiMigrationAddOn.Shared;

namespace WseToApiMigrationAddOn.Migrator.Setter.Templates {
    /// <summary>
    /// 
    /// </summary>
    public abstract class ApiValuesSetterTemplate : IApiValueSetter {
        #region Properties

        protected virtual bool IsResponseTestStepValue { get; } = false;

        protected abstract string TqlToGetApiTestStepValue { get; }

        protected abstract string TqlToGetModuleAttributeInApiModule { get; }

        protected abstract string TqlToGetWseTestStepValue { get; }

        #endregion

        #region Public Methods and Operators

        public void Execute(XTestStep apiTestStep, XTestStep wseTestStep) {
            try {
                //Get Raw Value
                XTestStepValue wseTestStepValue =
                        (XTestStepValue)wseTestStep.Search(TqlToGetWseTestStepValue).FirstOrDefault();
                if (wseTestStepValue == null) return;

                //Extract value need to be filled in api test step
                string wseValue = GetRefinedWseTestStepValue(wseTestStepValue.Value);

                //Note: we ignore this step in case of response. 
                if (!IsResponseTestStepValue) {
                    //get api value
                    string apiValue = GetValueInApiModule(apiTestStep.Module as ApiModule);
                    //if values match,do nothing.
                    if (!Regex.IsMatch(wseValue, @"\{.*\}") && apiValue == wseValue) return;
                }

                //if values does not match , Search if there is already a API xTestStepValue present.
                XTestStepValue apiTeststepValue =
                        (XTestStepValue)apiTestStep.Search(TqlToGetApiTestStepValue).FirstOrDefault();

                //if xTestStepValue not present,create it.
                if (apiTeststepValue == null) {
                    //check if module attribute is present.
                    var xModuleAttribute =
                            (XModuleAttribute)apiTestStep
                                              .Module.Search(TqlToGetModuleAttributeInApiModule).FirstOrDefault();
                    //if not present, Create it.
                    if (xModuleAttribute == null) {
                        XModuleAttribute m =
                                CreateModuleAttribute(apiTestStep.Module as ApiModule, wseTestStepValue);
                        apiTeststepValue = apiTestStep.CreateXTestStepValue(m);
                    }
                    //if already present, use it.
                    else
                        apiTeststepValue = apiTestStep.CreateXTestStepValue(xModuleAttribute);
                }

                //API xTestStepValue is present now. Assign Values to it now.
                apiTeststepValue.Disabled = wseTestStepValue.Disabled;
                apiTeststepValue.Value = wseValue;
                apiTeststepValue.ActionMode = wseTestStepValue.ActionMode;
                apiTeststepValue.Condition = wseTestStepValue.Condition;
                apiTeststepValue.Path = wseTestStepValue.Path;
            }
            catch (Exception e) {
                FileLogger.Instance.Error("Setting value failed : ", e);
            }
        }

        #endregion

        #region Methods

        protected abstract XModuleAttribute CreateModuleAttribute(ApiModule apiModule, XTestStepValue wseTestStepValue);

        protected abstract string GetRefinedWseTestStepValue(string value);

        protected abstract string GetValueInApiModule(ApiModule apiModule);

        #endregion
    }
}