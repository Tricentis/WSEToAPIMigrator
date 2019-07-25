using System;
using System.Linq;

using Newtonsoft.Json.Linq;

using Tricentis.TCAPIObjects.Objects;

using WseToApiMigrationAddOn.Helper;
using WseToApiMigrationAddOn.Migrator.Parser.Interfaces;
using WseToApiMigrationAddOn.Shared;

namespace WseToApiMigrationAddOn.Migrator.Parser {
    /// <summary>
    /// Extracts payload in string format from WSE artifacts
    /// </summary>
    public class JsonPayloadParser : IPayloadParser {
        #region Public Methods and Operators

        /// <summary>
        /// Parses WSE Module to get payload in string format
        /// </summary>
        /// <param name="wseModule">WSE Module</param>
        /// <param name="tql"> Not used</param>
        /// <returns>payload in string format</returns>
        public string Parse(XModule wseModule, string tql = "") {
            try {
                if (wseModule == null) return string.Empty;
                XModuleAttribute jsonObject = wseModule
                                              .Search("=>SUBPARTS:XModuleAttribute").Cast<XModuleAttribute>()
                                              .FirstOrDefault(x => x.BusinessType
                                                                   == "JsonObject" || x.BusinessType == "JsonArray");

                if (jsonObject != null) {
                    JArray arrayObject = new JArray();
                    JObject plainObject = CommonUtilities.ConstructJsonStructure(jsonObject, new JObject());
                    if (jsonObject.BusinessType == "JsonObject")
                        return Convert.ToString(plainObject);

                    arrayObject.Add(plainObject.Values());
                    return Convert.ToString(arrayObject);
                }
            }
            catch (Exception e) {
                FileLogger.Instance.Error(
                        $"Failed to create Json payload for request :{wseModule?.Name}",
                        e);
            }

            return string.Empty;
        }

        /// <summary>
        /// Parses WSE XTestStep to get payload
        /// </summary>
        /// <param name="xTestStep">WSE XTestStep</param>
        /// <param name="tql">tql to get root json object from request/reponse object</param>
        /// <returns>payload</returns>
        public string Parse(XTestStep xTestStep, string tql = "") {
            try {
                if (xTestStep == null) return string.Empty;
                XTestStepValue jsonObject = xTestStep.Search(tql).Cast<XTestStepValue>().FirstOrDefault(
                        x => x.ModuleAttribute.BusinessType
                             == "JsonObject"
                             || x.ModuleAttribute.BusinessType == "JsonArray");

                if (jsonObject != null) {
                    JArray arrayObject = new JArray();
                    JObject plainObject = CommonUtilities.ConstructJsonStructure(jsonObject, new JObject());
                    if (jsonObject.ModuleAttribute.BusinessType == "JsonObject")
                        return Convert.ToString(plainObject);

                    arrayObject.Add(plainObject.Values());
                    return Convert.ToString(arrayObject);
                }
            }
            catch (Exception e) {
                FileLogger.Instance.Error(
                        $"Failed to create Json payload for request :{xTestStep?.Name}",
                        e);
            }

            return string.Empty;
        }

        #endregion
    }
}