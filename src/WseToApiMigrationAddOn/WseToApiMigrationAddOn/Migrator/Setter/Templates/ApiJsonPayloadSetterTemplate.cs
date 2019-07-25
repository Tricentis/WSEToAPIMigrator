using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Tricentis.Automation.WseToApiMigrationAddOn.Helper;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Handler;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Interfaces;
using Tricentis.Automation.WseToApiMigrationAddOn.Shared;
using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Templates {
    /// <summary>
    /// Extracts payload from WSE teststep and sets it in API module object
    /// Creates module attributes for payload
    /// Extracts Json path of every node of payload 
    /// </summary>
    public abstract class ApiJsonPayloadSetterTemplate : IApiValueSetter, IPayloadSetter {
        #region Properties

        protected abstract string TqlToGetAllWseTestStepValue { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Evaluates the regex pattern and returns dynamic character "*" for json path
        /// </summary>
        /// <param name="match"> regex pattern to be matched</param>
        /// <returns></returns>
        public virtual string Evaluator(Match match) {
            return "[" + "*" + "]";
        }

        /// <summary>
        /// Extracts json payload from wse teststep and creates corresponding API teststep
        /// </summary>
        /// <param name="apiTestStep">testStep of API</param>
        /// <param name="wseTestStep">testStep of WSE</param>
        public void Execute(XTestStep apiTestStep, XTestStep wseTestStep) {
            FillPayload(apiTestStep, wseTestStep);
        }

        /// <summary>
        /// Returns Json path supported by Tosca for every field of payload from nodepath. 
        /// </summary>
        /// <param name="rootObjectNodePath"> node path of root/main object</param>
        /// <param name="wseTestStepValue">test step value for which json path is to be evaluated</param>
        /// <param name="jsonPath">raw json path</param>
        /// <returns></returns>
        public string ExtractToscaJsonPath(string rootObjectNodePath,
                                           XTestStepValue wseTestStepValue,
                                           string jsonPath) {
            string nodePath = wseTestStepValue.NodePath;
            nodePath = nodePath.Replace(wseTestStepValue.ParentValue.NodePath, string.Empty);

            var name = wseTestStepValue.RelevantXParams.FirstOrDefault(x => x.Name == "Name");
            if (name?.Value != null && name.Value.EndsWith("*"))
                nodePath = Regex.Replace(nodePath, @"/item#(\d*)$", Evaluator);
            else {
                nodePath = Regex.Replace(nodePath, @"/item#(\d*)$", Evaluator2);
            }

            jsonPath = Regex.Replace(jsonPath, @"\[[\d | ""*""]*\]$", nodePath);

            return jsonPath;
        }

        /// <summary>
        /// Evaluates Json path of wseteststep values.
        /// </summary>
        /// <param name="wseTestStep">parent node of wseteststep</param>
        /// <param name="wseTestStepValue">node for which json path is to be evaluated</param>
        /// <param name="jsonPath">reform json path for static or dynamic payload</param>
        /// <returns></returns>
        public virtual string GetJsonPath(XTestStep wseTestStep, XTestStepValue wseTestStepValue, string jsonPath) {
            TCObject rootObject = wseTestStep
                                  .Search("=>SUBPARTS:XTestStepValue[Name==\"Response\"]->SUBPARTS")
                                  .FirstOrDefault();

            var rootObjectNodePath = rootObject?.NodePath;
            return ExtractToscaJsonPath(rootObjectNodePath, wseTestStepValue, jsonPath);
        }

        #endregion

        #region Methods

        private XTestStepValue CreateModuleAttributeForPayload(XTestStepValue wseTestStepValue,
                                                               string jsonPath,
                                                               dynamic parent,
                                                               ApiModule apiModule,
                                                               dynamic parentTest,
                                                               string cardinality,
                                                               bool isArray = false) {
            if (jsonPath.StartsWith(".")) jsonPath = jsonPath.Remove(0, 1);
            return BusinessParameterHandler.CreateBusinessParameterInTosca(wseTestStepValue,
                                                                           jsonPath,
                                                                           parent,
                                                                           apiModule,
                                                                           BusinessParameterPathTypes.JSONPATH,
                                                                           parentTest,
                                                                           cardinality,
                                                                           isArray);
        }

        private string Evaluator2(Match match) {
            var v = Convert.ToInt32(match.Groups[1].Value) - 1;
            return "[" + v + "]";
        }

        private void FillPayload(XTestStep apiTestStep,
                                 XTestStep wseTestStep) {
            try {
                var tcObjects = wseTestStep.Search(TqlToGetAllWseTestStepValue).Cast<XTestStepValue>().ToList();
                FillPayloadRec(apiTestStep, wseTestStep, tcObjects, apiTestStep.Module, apiTestStep, string.Empty);
            }
            catch (Exception ex) {
                FileLogger.Instance.Error("Unable to creation payload ", ex);
            }
        }

        private void FillPayloadRec(XTestStep apiTestStep,
                                    XTestStep wseTestStep,
                                    List<XTestStepValue> tcObjects,
                                    dynamic moduleData,
                                    dynamic testStepData,
                                    string jsonPath) {
            var arrayTemplate = "{0}";
            var objectTemplate = ".{0}";
            try {
                int index = 0;
                foreach (var wseTestStepValue in tcObjects) {
                    string curJsonPath;
                    if (wseTestStepValue.ParentValue.ModuleAttribute.BusinessType == "JsonArray") {
                        curJsonPath = jsonPath + string.Format(arrayTemplate, $"[{Convert.ToString(index++)}]");
                    }
                    else {
                        curJsonPath = jsonPath + string.Format(objectTemplate, wseTestStepValue.Name);
                    }

                    curJsonPath = GetJsonPath(wseTestStep, wseTestStepValue, curJsonPath);
                    var apiXTestStepValue = CreateModuleAttributeForPayload(wseTestStepValue,
                                                                            curJsonPath,
                                                                            moduleData,
                                                                            (ApiModule)apiTestStep.Module,
                                                                            testStepData,
                                                                            wseTestStepValue
                                                                                    .ModuleAttribute?.Cardinality);
                    FillPayloadRec(apiTestStep,
                                   wseTestStep,
                                   wseTestStepValue.SubValues.ToList(),
                                   apiXTestStepValue.ModuleAttribute,
                                   apiXTestStepValue,
                                   curJsonPath);
                }
            }
            catch (Exception ex) {
                FileLogger.Instance.Error("Error while creating module attribute {1} ", ex);
            }
        }

        #endregion
    }
}