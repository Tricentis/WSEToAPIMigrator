using System;

using Tricentis.Automation.WseToApiMigrationAddOn.Extensions;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Interfaces;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Templates;
using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.ResponseSetter {
    /// <summary>
    /// Setter to create modue attribute for response attribute.
    /// </summary>
    public class ApiStatusCodeSetter : ApiValuesSetterTemplate, IApiResponseValueSetter {
        #region Properties

        protected override bool IsResponseTestStepValue => true;

        protected override string TqlToGetApiTestStepValue => $"=>SUBPARTS: XTestStepValue[Name==\"StatusCode\"]";

        protected override string TqlToGetModuleAttributeInApiModule =>
                "=>SUBPARTS:XModuleAttribute[Name==\"StatusCode\"]";

        protected override string TqlToGetWseTestStepValue =>
                "=>SUBPARTS: XTestStepValue[Name==\"Communicate\"]=>SUBPARTS: XTestStepValue[Name==\"Receive\"]=>SUBPARTS:XTestStepValue[Name==\"Status code name\"]";

        #endregion

        #region Methods

        /// <summary>
        /// Create module attribute for status code in respinse module of API
        /// </summary>
        /// <param name="apiModule">apiModue under which module attribute gets created</param>
        /// <param name="wseTestStepValue">wseTestStepValue</param>
        /// <returns></returns>
        protected override XModuleAttribute
                CreateModuleAttribute(ApiModule apiModule, XTestStepValue wseTestStepValue) {
            return apiModule.CreateModuleAttribute(
                    "StatusCode",
                    HttpStatusCodeExtension.ValueRange(),
                    wseTestStepValue.ModuleAttribute.DefaultValue,
                    "StatusCode",
                    "StatusCode",
                    wseTestStepValue.ModuleAttribute.DefaultActionMode,
                    wseTestStepValue.ModuleAttribute.DefaultDataType);
        }

        protected override string GetRefinedWseTestStepValue(string value) {
            return value;
        }

        protected override string GetValueInApiModule(ApiModule apiModule) {
            throw new NotImplementedException();
        }

        #endregion
    }
}