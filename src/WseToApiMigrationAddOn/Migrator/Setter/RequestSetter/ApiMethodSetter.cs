using System;

using Tricentis.Automation.WseToApiMigrationAddOn.Extensions;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Interfaces;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Templates;
using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.RequestSetter {
    /// <summary>
    /// Extracts method from WSE teststep and sets respective field in API teststep 
    /// </summary>
    public class ApiMethodSetter : ApiValuesSetterTemplate, IApiRequestValueSetter {
        #region Properties

        protected override string TqlToGetApiTestStepValue => $"=>SUBPARTS: XTestStepValue[Name==\"Method\"]";

        protected override string TqlToGetModuleAttributeInApiModule => "=>SUBPARTS:XModuleAttribute[Name==\"Method\"]";

        protected override string TqlToGetWseTestStepValue =>
                "=>SUBPARTS: XTestStepValue[Name==\"Communicate\"]=>SUBPARTS: XTestStepValue[Name==\"Send\"]=>SUBPARTS:XTestStepValue[Name==\"Method\"]";

        #endregion

        #region Methods

        /// <summary>
        /// Creates module attribute for Method property under API module 
        /// </summary>
        /// <param name="apiModule">apiModule under which modue attribute gets created</param>
        /// <param name="wseTestStepValue">sets default value, default action mode, default datatype of wseteststepValue in api module attribute.</param>
        /// <returns></returns>
        protected override XModuleAttribute
                CreateModuleAttribute(ApiModule apiModule, XTestStepValue wseTestStepValue) {
            return apiModule.CreateModuleAttribute(
                    "Method",
                    HttpMethodExtension.ValueRangeString(),
                    wseTestStepValue
                            .ModuleAttribute.DefaultValue,
                    "Method",
                    "Method",
                    wseTestStepValue
                            .ModuleAttribute
                            .DefaultActionMode,
                    wseTestStepValue
                            .ModuleAttribute
                            .DefaultDataType);
        }

        protected override string GetRefinedWseTestStepValue(string value) {
            return value;
        }

        /// <summary>
        /// returns value of "Method" property of Api Module. return empt string if property doesn't exist
        /// </summary>
        /// <param name="apiModule">apiModule from which value is to be extracted.</param>
        /// <returns></returns>
        protected override string GetValueInApiModule(ApiModule apiModule) {
            try {
                return apiModule.GetPropertyValue("Method");
            }
            catch (Exception) {
                return string.Empty;
            }
        }

        #endregion
    }
}