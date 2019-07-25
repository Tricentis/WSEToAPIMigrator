using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;

using Tricentis.Automation.WseToApiMigrationAddOn.Extensions.Tricentis.Automation.Api.Core.Extensions;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Handler;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Parser;
using Tricentis.Automation.WseToApiMigrationAddOn.Shared;
using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Helper {
    /// <summary>
    /// Contains methods which are commonly used by different classes
    /// </summary>
    public static class CommonUtilities {
        #region Public Methods and Operators

        /// <summary>
        /// construct json payload for Api modules
        /// </summary>
        /// <param name="rawRequestPayload">WSE XmoduleAttributes</param>
        /// <param name="jObject">json payload</param>
        /// <returns>json payload</returns>
        public static JObject ConstructJsonStructure(XModuleAttribute rawRequestPayload, JObject jObject) {
            try {
                foreach (var attribute in rawRequestPayload.Attributes) {
                    if (attribute.BusinessType == "JsonValue") {
                        jObject.Add(attribute.Name, attribute.DefaultValue);
                    }
                    else if (attribute.BusinessType == "JsonObject") {
                        JObject nestedObject = new JObject();
                        nestedObject = ConstructJsonStructure(attribute, nestedObject);
                        jObject.Add(attribute.Name, nestedObject);
                    }
                    else if (attribute.BusinessType == "JsonArray") {
                        JArray arrayAttributes = new JArray();
                        JObject arrayObject = new JObject();
                        arrayObject = ConstructJsonStructure(attribute, arrayObject);
                        arrayAttributes.Add(arrayObject.Values());
                        jObject.Add(attribute.Name, arrayAttributes);
                    }
                }
            }
            catch (Exception ex) {
                FileLogger.Instance.Error(ex.Message);
            }

            return jObject;
        }

        /// <summary>
        /// construct json payload for Api modules
        /// </summary>
        /// <param name="rawRequestPayload">WSE XTestStepValues</param>
        /// <param name="jObject">json payload</param>
        /// <returns>json payload</returns>
        public static JObject ConstructJsonStructure(XTestStepValue rawRequestPayload, JObject jObject) {
            try {
                foreach (var testStepValue in rawRequestPayload.SubValues) {
                    if (testStepValue.ModuleAttribute.BusinessType == "JsonValue") {
                        jObject.Add(testStepValue.Name, testStepValue.Value);
                    }
                    else if (testStepValue.ModuleAttribute.BusinessType == "JsonObject") {
                        JObject nestedObject = new JObject();
                        nestedObject = ConstructJsonStructure(testStepValue, nestedObject);
                        jObject.Add(testStepValue.Name, nestedObject);
                    }
                    else if (testStepValue.ModuleAttribute.BusinessType == "JsonArray") {
                        JArray arrayAttributes = new JArray();
                        JObject arrayObject = new JObject();
                        arrayObject = ConstructJsonStructure(testStepValue, arrayObject);
                        arrayAttributes.Add(arrayObject.Values());
                        jObject.Add(testStepValue.Name, arrayAttributes);
                    }
                }
            }
            catch (Exception ex) {
                FileLogger.Instance.Error(ex.Message);
            }

            return jObject;
        }

        /// <summary>
        /// Create API Modules Folder Name on the basis of SoapAction, Resource, Endpoint and Method(HttpVerbs)
        /// </summary>
        /// <param name="xTestStep">Wse Teststep</param>
        /// <param name="parserResult">Contains transport info(Endpoint, Method, Resource, Headers)</param>
        /// <returns>FolderName</returns>
        public static string CreateModuleFolderName(XTestStep xTestStep, IWseArtifactsParser parserResult) {
            try {
                var soapAction = GetSoapAction(parserResult.Headers);

                if (!string.IsNullOrEmpty(soapAction)) {
                    var action = soapAction.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last();
                    if (!string.IsNullOrEmpty(action)) return AppendMethod(action, parserResult.Method);
                }

                if (!string.IsNullOrEmpty(parserResult.Resource))
                    return AppendMethod(parserResult.Resource, parserResult.Method);

                if (!string.IsNullOrEmpty(parserResult.Endpoint))
                    return AppendMethod(parserResult.Endpoint, parserResult.Method);
            }
            catch (Exception) {
                // ignored
            }

            return "UnknownResource";
        }

        /// <summary>
        /// Replace some special characters in nodepath 
        /// </summary>
        /// <param name="nodePath">tcObject NodePath</param>
        /// <returns>NodePath</returns>
        public static string GetEscapedNodePath(string nodePath) {
            nodePath = Regex.Replace(nodePath, @"\\", @"\\");
            nodePath = Regex.Replace(nodePath, @"\/", @"\/");
            nodePath = Regex.Replace(nodePath, "\"", "\\\"");
            return nodePath;
        }

        /// <summary>
        /// Get WseTestSteps on the basis of TQL
        /// </summary>
        /// <param name="rootComponentFolder">Component Folder on which migration is executing</param>
        /// <param name="wseModuleTestSteps">WSE XTestSteps</param>
        /// <returns>List of WSE XTesSteps</returns>
        public static List<XTestStep> GetFilteredWseTestSteps(TCObject rootComponentFolder,
                                                              IEnumerable<XTestStep> wseModuleTestSteps) {
            var list = new List<XTestStep>();

            foreach (XTestStep testStep in wseModuleTestSteps) {
                try {
                    XTestStep xtestStep = (XTestStep)rootComponentFolder
                                                     .Search(
                                                             $"=>COMPLEMENT(=>UNION(=>SUBPARTS: XTestStep[UniqueId == \"{testStep.UniqueId}\"],=>SUBPARTS:TestStepFolderReference=>ReusedItem=>SUBPARTS:XTestStep[UniqueId == \"{testStep.UniqueId}\"]),=>SUBPARTS:TestCaseTemplateInstance=>SUBPARTS:XTestStep[UniqueId == \"{testStep.UniqueId}\"])")
                                                     .FirstOrDefault();
                    if (xtestStep == null) continue;
                    list.Add(xtestStep);
                }
                catch (Exception ex) {
                    FileLogger.Instance.Error("Failed to get WSETestStep", ex);
                }
            }

            return list;
        }

        /// <summary>
        /// Get Query Param Keys from list of query params list
        /// </summary>
        /// <param name="parserResultQueryParams">Query params list in key-value pair extraxt from url</param>
        /// <returns>Query Param Key</returns>
        public static string GetQueryParamsKeys(Dictionary<string, string> parserResultQueryParams) {
            try {
                return string.Concat(parserResultQueryParams.Keys);
            }
            catch (Exception) {
                // ignored
            }

            return string.Empty;
        }

        /// <summary>
        /// Get SoapAction from headers
        /// </summary>
        /// <param name="headers">headers key-value pair</param>
        /// <returns>SoapAction</returns>
        public static string GetSoapAction(Dictionary<string, string> headers) {
            string soapAction = String.Empty;
            try {
                if (headers == null || !headers.Any()) return soapAction;
                if (headers.ContainsKey("SOAPAction")) {
                    var soapActionValue =
                            headers.Where(x => x.Key == "SOAPAction").Select(x => x.Value).FirstOrDefault();
                    if (String.IsNullOrEmpty(soapActionValue)) return soapAction;
                    if (!soapActionValue.Contains("\"")) return soapActionValue;
                    var arr = soapActionValue.Split('"');
                    return arr[1];
                }

                if (headers.ContainsKey("Content-Type")) {
                    var contentTypeValue =
                            headers.Where(x => x.Key == "Content-Type").Select(x => x.Value).FirstOrDefault();
                    if (!String.IsNullOrEmpty(contentTypeValue) && contentTypeValue.Contains("action=")) {
                        contentTypeValue = RemoveExtraDoubleQuotes(contentTypeValue);
                        var arr = contentTypeValue.Split('"');
                        return arr[1];
                    }
                }
            }
            catch (Exception e) {
                FileLogger.Instance.Error("Failed to get Soap Action", e);
            }

            return soapAction;
        }

        /// <summary>
        /// Modify ContentType Header value to empty in case it contains buffer, CP and XL reference.
        /// </summary>
        /// <param name="headers">headers in Key-Value pair</param>
        /// <returns>headers in Key-Value pair</returns>
        public static Dictionary<string, string> ModifyContentTypeToEmpty(Dictionary<string, string> headers) {
            var blnValue = headers.TryGetValue("Content-Type", out string contentTypeValue);
            if (blnValue && !string.IsNullOrEmpty(contentTypeValue) && Regex.IsMatch(contentTypeValue, @"\{.*\}")) {
                headers["Content-Type"] = "";
            }

            return headers;
        }

        /// <summary>
        /// Remove extra double quotes from ContentType to ""
        /// </summary>
        /// <param name="contentTypeValue">ContentType</param>
        /// <returns>ContentType</returns>
        public static string RemoveExtraDoubleQuotes(string contentTypeValue) {
            if (String.IsNullOrEmpty(contentTypeValue)) return contentTypeValue;
            string newStr = contentTypeValue;
            if (contentTypeValue.Contains("\"\"\"")) {
                newStr = contentTypeValue.Replace("\"\"\"", "\"");
            }

            return newStr;
        }

        /// <summary>
        /// Replace Resource with LastResponseResource in Existing TestSteps
        /// </summary>
        /// <param name="objectToExecuteOn">TCObject</param>
        public static void ReplaceResourceWithLastResponseResource(TCObject objectToExecuteOn) {
            try {
                var getResourceValueTuple =
                        ResourceManagerHandler.GetResourceFromResourceId(ResourceManagerHandler.ResourceId);
                if (getResourceValueTuple.Item1 && getResourceValueTuple.Item2 != null) {
                    foreach (var testCaseResource in getResourceValueTuple.Item2.TestCaseResources) {
                        var testcases = objectToExecuteOn.Search(
                                String.Format("=>SUBPARTS: TestCase[Name == \"{0}\"]", testCaseResource.TestCaseName));
                        foreach (var testcase in testcases) {
                            foreach (var resourceName in testCaseResource.ResourceNameList) {
                                var xTestStepValues = testcase.Search(
                                        String.Format(
                                                "=>INTERSECTION(=>UNION(=>SUBPARTS:XTestStepValue[Value==\"{0}\"], =>return SUBPARTS:XTestStepValue[Value==\"{{RES[{0}]}}\"]),=>return UNION(=>SUBPARTS:XTestStepValue[Value==\"{0}\"], =>return SUBPARTS:XTestStepValue[Value==\"{{RES[{0}]}}\"])->ModuleAttribute->Module:XModule[Engine != \"Webservice\"])",
                                                resourceName));
                                foreach (var xTestStepValue in xTestStepValues) {
                                    if (xTestStepValue is XTestStepValue) {
                                        var x = xTestStepValue as XTestStepValue;
                                        x.Value = x.Value.Replace(x.Value.ExtractStringFromResource(),
                                                                  "LastResponseResource");
                                    }
                                }
                            }
                        }
                    }

                    ResourceManagerHandler.DeleteResource(ResourceManagerHandler.ResourceId);
                }
            }
            catch (Exception ex) {
                FileLogger.Instance.Error("There is an issue in creating resource manager list.", ex);
            }
        }

        /// <summary>
        /// Get Request/Response API Module on the basis of scanTag.
        /// </summary>
        /// <param name="objectToExecuteOn">TcObjects</param>
        /// <param name="wseArtifactsParser">Contains Transport information</param>
        /// <param name="wseTestStep">WSE TestSteps</param>
        /// <returns>Request/Response API Module</returns>
        public static (ApiModule, ApiModule) SearchExistingApiModule(TCObject objectToExecuteOn,
                                                                     IWseArtifactsParser wseArtifactsParser,
                                                                     XTestStep wseTestStep) {
            if (SpecializationHelper.IsUsingEmbeddedResource(wseTestStep, out TCFolder searchFolder)) {
                var requestModule = SearchRequestModule(searchFolder, wseArtifactsParser);
                var responseModule = SearchResponseModule(searchFolder, wseArtifactsParser);
                if (requestModule != null) return (requestModule, responseModule);
            }

            return (SearchRequestModule(objectToExecuteOn, wseArtifactsParser),
                       SearchResponseModule(objectToExecuteOn, wseArtifactsParser));
        }

        #endregion

        #region Methods

        private static string AppendMethod(string action, string parserResultMethod) {
            return $"{action}_{parserResultMethod}";
        }

        private static ApiModule SearchRequestModule(TCObject rootComponentFolder, IWseArtifactsParser testStepParser) {
            return ScanTag.SearchModuleByScanTag(rootComponentFolder,
                                                 ScanTag.GetRequestScanTag(testStepParser));
        }

        private static ApiModule
                SearchResponseModule(TCObject rootComponentFolder, IWseArtifactsParser testStepParser) {
            return ScanTag.SearchModuleByScanTag(rootComponentFolder,
                                                 ScanTag.GetResponseScanTag(testStepParser));
        }

        #endregion
    }
}