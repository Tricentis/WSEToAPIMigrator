using System;

using Tricentis.TCAPIObjects.Objects;

using WseToApiMigrationAddOn.Extensions;
using WseToApiMigrationAddOn.Helper;
using WseToApiMigrationAddOn.Migrator.Setter.Interfaces;
using WseToApiMigrationAddOn.Migrator.Setter.Templates;

namespace WseToApiMigrationAddOn.Migrator.Setter.RequestSetter {
    /// <summary>
    /// Extracts Address from WSE teststep and sets endpoint in API teststep 
    /// </summary>
    public class ApiEndpointSetter : ApiValuesSetterTemplate, IApiRequestValueSetter {
        #region Properties

        protected override string TqlToGetApiTestStepValue => $"=>SUBPARTS: XTestStepValue[Name==\"Endpoint\"]";

        protected override string TqlToGetModuleAttributeInApiModule =>
                "=>SUBPARTS:XModuleAttribute[Name==\"Endpoint\"]";

        protected override string TqlToGetWseTestStepValue =>
                "=>SUBPARTS: XTestStepValue[Name==\"Communicate\"]=>SUBPARTS: XTestStepValue[Name==\"Address\"]";

        #endregion

        #region Methods

        /// <summary>
        /// Creates module attribute for Endpoint
        /// </summary>
        /// <param name="apiModule">apiModule under which module attribute is to created.</param>
        /// <param name="wseTestStepValue">teststepvalues of WSE</param>
        /// <returns></returns>
        protected override XModuleAttribute
                CreateModuleAttribute(ApiModule apiModule, XTestStepValue wseTestStepValue) {
            return apiModule.CreateModuleAttribute(
                    "Endpoint",
                    GetRefinedWseTestStepValue(wseTestStepValue.Value),
                    GetRefinedWseTestStepValue(wseTestStepValue.Value),
                    "Endpoint",
                    "Endpoint",
                    wseTestStepValue.ModuleAttribute.DefaultActionMode,
                    wseTestStepValue.ModuleAttribute.DefaultDataType);
        }

        protected override string GetRefinedWseTestStepValue(string value) {
            return UriHelper.GetEndPoint(value);
        }

        protected override string GetValueInApiModule(ApiModule apiModule) {
            try {
                return apiModule.GetPropertyValue("Endpoint");
            }
            catch (Exception) {
                return string.Empty;
            }
        }

        #endregion
    }
}