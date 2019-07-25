using System;

using Tricentis.Automation.WseToApiMigrationAddOn.Extensions;
using Tricentis.Automation.WseToApiMigrationAddOn.Helper;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Interfaces;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Templates;
using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.RequestSetter {
    /// <summary>
    /// Sets Resource property in Api Module
    /// </summary>
    public class ApiResourceSetter : ApiValuesSetterTemplate, IApiRequestValueSetter {
        #region Properties

        protected override string TqlToGetApiTestStepValue => $"=>SUBPARTS: XTestStepValue[Name==\"Resource\"]";

        protected override string TqlToGetModuleAttributeInApiModule =>
                "=>SUBPARTS:XModuleAttribute[Name==\"Resource\"]";

        protected override string TqlToGetWseTestStepValue =>
                "=>SUBPARTS: XTestStepValue[Name==\"Communicate\"]=>SUBPARTS: XTestStepValue[Name==\"Address\"]";

        #endregion

        #region Methods

        /// <summary>
        /// Creates Module attribute for Resource property
        /// </summary>
        /// <param name="apiModule">apiModule under which module attribute gets created</param>
        /// <param name="wseTestStepValue">wseTestStepValue contains value, default datatype, Default action mode and other properties of wse teststep.</param>
        /// <returns></returns>
        protected override XModuleAttribute
                CreateModuleAttribute(ApiModule apiModule, XTestStepValue wseTestStepValue) {
            return apiModule.CreateModuleAttribute(
                    "Resource",
                    GetRefinedWseTestStepValue(wseTestStepValue.Value),
                    GetRefinedWseTestStepValue(wseTestStepValue.Value),
                    "Resource",
                    "Resource",
                    wseTestStepValue.ModuleAttribute.DefaultActionMode,
                    wseTestStepValue.ModuleAttribute.DefaultDataType);
        }

        protected override string GetRefinedWseTestStepValue(string value) {
            return UriHelper.GetResource(value);
        }

        /// <summary>
        /// Extracts value of Resource property from apiModule
        /// </summary>
        /// <param name="apiModule">apiModule</param>
        /// <returns></returns>
        protected override string GetValueInApiModule(ApiModule apiModule) {
            try {
                return apiModule.GetPropertyValue("Resource");
            }
            catch (Exception) {
                return string.Empty;
            }
        }

        #endregion
    }
}