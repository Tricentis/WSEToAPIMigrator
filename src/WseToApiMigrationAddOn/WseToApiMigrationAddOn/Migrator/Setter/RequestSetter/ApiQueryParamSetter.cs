using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Tricentis.Automation.WseToApiMigrationAddOn.Extensions;
using Tricentis.Automation.WseToApiMigrationAddOn.Helper;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Interfaces;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Templates;
using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.RequestSetter {
    /// <summary>
    /// Extracts Query params from wse teststep Address property and sets them in API teststep in Params section. 
    /// </summary>
    public class ApiQueryParamSetter : ApiMultipleValuesSetterTemplate, IApiRequestValueSetter {
        #region Properties

        protected override string TqlToGetApiTestStepValue => "=>SUBPARTS: XTestStepValue[Name==\"{0}\"]";

        protected override string TqlToGetModuleAttributeInApiModule => "=>SUBPARTS:XModuleAttribute[Name==\"{0}\"]";

        #endregion

        #region Methods

        /// <summary>
        /// Creates module attribute for Query params in Api module.
        /// </summary>
        /// <param name="apiModule">apiModule under which module attribute gets created.</param>
        /// <param name="wseTestStepValue">wsetesstep value to set in api module attribute</param>
        /// <returns></returns>
        protected override XModuleAttribute CreateModuleAttribute(ApiModule apiModule,
                                                                  KeyValuePair<string, string> wseTestStepValue) {
            return apiModule.CreateModuleAttribute(wseTestStepValue.Key,
                                                   String.Empty,
                                                   "UrlParam",
                                                   $"{wseTestStepValue.Key};Query;0",
                                                   XTestStepActionMode.Insert);
        }

        protected override string GetValueInApiModule(ApiModule apiModule, string key) {
            return string.Empty;
        }

        protected override Dictionary<string, string> GetWseTestStepValueAsKeyValPair(XTestStep testStep) {
            var wseTestStepAddress = (XTestStepValue)testStep
                                                     .Search(
                                                             "=>SUBPARTS: XTestStepValue[Name==\"Communicate\"]=>SUBPARTS: XTestStepValue[Name==\"Address\"]")
                                                     .FirstOrDefault();
            if (wseTestStepAddress == null) return new Dictionary<string, string>();
            var queryParametersWseTestStep = UriHelper.DecodeQueryParameters(wseTestStepAddress.Value);
            return queryParametersWseTestStep;
        }

        /// <summary>
        /// Updates Value range of api module
        /// </summary>
        /// <param name="apiTeststepValue">value range to update in module attribute of apiTeststepValue</param>
        /// <param name="apiValue">value to set in Value range</param>
        /// <param name="wseValue">value to set in Value range in case of header</param>
        protected override void UpdateValueRange(XTestStepValue apiTeststepValue, string apiValue, string wseValue) {
            if (String.IsNullOrEmpty(apiTeststepValue.ModuleAttribute.ValueRange)) {
                if (!String.IsNullOrEmpty(apiValue)) {
                    apiTeststepValue.ModuleAttribute.ValueRange = Regex.Replace(apiValue, ";", @"\;");
                    apiTeststepValue.ModuleAttribute.DefaultValue = Regex.Replace(apiValue, ";", @"\;");
                }

                UpdateValueRange(apiTeststepValue, wseValue);
            }
            else {
                UpdateValueRange(apiTeststepValue, wseValue);
            }
        }

        //Implement this for header
        private static void UpdateValueRange(XTestStepValue apiTeststepValue, string wseValue) {
            if (!String.IsNullOrEmpty(wseValue) && !apiTeststepValue.ModuleAttribute.ValueRange.Contains(wseValue)) {
                if (!string.IsNullOrEmpty(apiTeststepValue.ModuleAttribute.ValueRange)) {
                    apiTeststepValue.ModuleAttribute.ValueRange =
                            $"{apiTeststepValue.ModuleAttribute.ValueRange};{Regex.Replace(wseValue, ";", @"\;")}";
                }
                else {
                    apiTeststepValue.ModuleAttribute.ValueRange = Regex.Replace(wseValue, ";", @"\;");
                }
            }
        }

        #endregion
    }
}