using System.Collections.Generic;
using System.Linq;

using Tricentis.Automation.WseToApiMigrationAddOn.Extensions;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Interfaces;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Templates;
using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.ResponseSetter {
    /// <summary>
    /// Extracts all response headers and creates module attributes in API Response module.
    /// </summary>
    public class ApiResponseHeadersSetter : ApiMultipleValuesSetterTemplate, IApiResponseValueSetter {
        #region Properties

        protected override bool IsResponseTestStepValue => true;

        protected override string TqlToGetApiTestStepValue => "=>SUBPARTS: XTestStepValue[Name==\"{0}\"]";

        protected override string TqlToGetModuleAttributeInApiModule => "=>SUBPARTS:XModuleAttribute[Name==\"{0}\"]";

        private string TqlToGetWseTestStepValue =>
                "=>SUBPARTS: XTestStepValue[Name==\"Communicate\"]=>SUBPARTS: XTestStepValue[Name==\"Receive\"]=>SUBPARTS:XTestStepValue[Name==\"Headers\"]=>SUBPARTS:XTestStepValue";

        #endregion

        #region Methods

        /// <summary>
        /// Creates module attribute for response headers.
        /// </summary>
        /// <param name="apiModule">apiModule</param>
        /// <param name="wseTestStepValue">testStepValue of WSE</param>
        /// <returns>returns module attribute</returns>
        protected override XModuleAttribute CreateModuleAttribute(ApiModule apiModule,
                                                                  KeyValuePair<string, string> wseTestStepValue) {
            return apiModule.CreateModuleAttribute(
                    wseTestStepValue.Key,
                    wseTestStepValue.Value,
                    wseTestStepValue.Value,
                    "Header",
                    wseTestStepValue.Key,
                    XTestStepActionMode.Insert);
        }

        protected override string GetValueInApiModule(ApiModule apiModule, string key) {
            return string.Empty;
        }

        /// <summary>
        /// Fetches all response headers in dictionary
        /// </summary>
        /// <param name="testStep">wse testStep contains header information</param>
        /// <returns></returns>
        protected override Dictionary<string, string> GetWseTestStepValueAsKeyValPair(XTestStep testStep) {
            var headers =
                    testStep.Search(
                                    TqlToGetWseTestStepValue)
                            .Cast<XTestStepValue>();
            return headers.Select(x => new { x.Name, x.Value }).ToDictionary(t => t.Name, t => t.Value);
        }

        protected override void UpdateValueRange(XTestStepValue apiTeststepValue, string apiValue, string wseValue) {
        }

        #endregion
    }
}