using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

using Tricentis.TCAPIObjects.Objects;

using WseToApiMigrationAddOn.Extensions;
using WseToApiMigrationAddOn.Helper;
using WseToApiMigrationAddOn.Migrator.Setter.Interfaces;
using WseToApiMigrationAddOn.Migrator.Setter.Templates;

namespace WseToApiMigrationAddOn.Migrator.Setter.RequestSetter {
    /// <summary>
    /// Extracts headers of tranpport information and creates module attribute for them.
    /// Retrieves all headers in APi module.
    /// </summary>
    public class ApiRequestHeadersSetter : ApiMultipleValuesSetterTemplate, IApiRequestValueSetter {
        #region Properties

        protected override string TqlToGetApiTestStepValue => "=>SUBPARTS: XTestStepValue[Name==\"{0}\"]";

        protected override string TqlToGetModuleAttributeInApiModule =>
                "=>SUBPARTS:XModuleAttribute[Name==\"{0}\"]";

        private string TqlToGetWseTestStepValue =>
                "=>SUBPARTS: XTestStepValue[Name==\"Communicate\"]=>SUBPARTS: XTestStepValue[Name==\"Send\"]=>SUBPARTS:XTestStepValue[Name==\"Headers\"]=>SUBPARTS:XTestStepValue";

        #endregion

        #region Methods

        /// <summary>
        /// Creates module attrribute for header values
        /// </summary>
        /// <param name="apiModule">api module under which moue attribute hets created.</param>
        /// <param name="wseTestStepValue">Contains value of headers.</param>
        /// <returns></returns>
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
            try {
                if (apiModule?.Headers != null && apiModule.Headers.Length > 0) {
                    var l = Deserialize<List<List<KeyValuePair<string, string>>>>(apiModule.Headers);
                    foreach (var keyvaluepairList in l) {
                        var k = keyvaluepairList.First().Value;
                        if (k == key) return CommonUtilities.RemoveExtraDoubleQuotes(keyvaluepairList.Last().Value);
                    }
                }
            }
            catch (Exception) {
                // ignored
            }

            return string.Empty;
        }
        /// <summary>
        /// Extracts all headers from wseteststep value as key value pairs
        /// </summary>
        /// <param name="testStep">Teststep under which all headers retain</param>
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

        private T Deserialize<T>(byte[] byteArray) {
            using (var ms = new MemoryStream(byteArray)) {
                return (T)new BinaryFormatter().Deserialize(ms);
            }
        }

        #endregion
    }
}