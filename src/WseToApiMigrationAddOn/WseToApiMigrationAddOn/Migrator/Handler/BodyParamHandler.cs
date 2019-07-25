using System.Collections.Generic;
using System.Linq;

using Tricentis.TCAPIObjects.Objects;

using WseToApiMigrationAddOn.Extensions;
using WseToApiMigrationAddOn.Extensions.Tricentis.Automation.Api.Core.Extensions;
using WseToApiMigrationAddOn.Helper;

namespace WseToApiMigrationAddOn.Migrator.Handler {
    /// <summary>
    /// Create and set Body parameter at module and teststep level.
    /// </summary>
    public static class BodyParamHandler {
        #region Public Methods and Operators

        /// <summary>
        /// Creates and set Body Param at request and response module and teststeps.
        /// </summary>
        /// <param name="apiTestStep">Teststep of ApiEngine</param>
        /// <param name="wseTestStepValues">Teststepvalues of WseEngine</param>
        public static void CreateBodyParameterForRequestAndResponse(XTestStep apiTestStep,
                                                                    List<XTestStepValue> wseTestStepValues) {
            if (!wseTestStepValues.Any()) return;

            XModuleAttribute bodyXModuleAttribute = (XModuleAttribute)
                    apiTestStep.Module.Search(
                                       "=>SUBPARTS:XModuleAttribute[Name==\"Body\" and Path==\"Body\" and PathType==\"Body\"]")
                               .FirstOrDefault();
            if (bodyXModuleAttribute == null) {
                bodyXModuleAttribute = (apiTestStep.Module as ApiModule).CreateModuleAttribute("Body",
                                                                                               string.Empty,
                                                                                               string.Empty,
                                                                                               "Body",
                                                                                               "Body",
                                                                                               XTestStepActionMode
                                                                                                       .Select,
                                                                                               ModuleAttributeDataType
                                                                                                       .String);
                bodyXModuleAttribute.Cardinality = "0-N";
            }

            CreateBodyParameterAtTeststepLevel(wseTestStepValues, bodyXModuleAttribute, apiTestStep);
        }

        #endregion

        #region Methods

        private static void CreateBodyParameterAtTeststepLevel(List<XTestStepValue> tcObjects,
                                                               XModuleAttribute bodyAttribute,
                                                               XTestStep apiTestStep) {
            foreach (XTestStepValue wseTestStepValue in tcObjects) {
                var apiTestStepValue = apiTestStep.CreateXTestStepValue(bodyAttribute);
                if (wseTestStepValue.SpecializationModule.Name == "Web service request data in XML Resource"
                    || wseTestStepValue.SpecializationModule.Name == "Web service request data in JSON Resource") {
                    apiTestStepValue.DataType = wseTestStepValue.DataType;
                    apiTestStepValue.ActionMode = XTestStepActionMode.Insert;
                    if (!wseTestStepValue.Value.IsNullOrBlankInTosca()) {
                        apiTestStepValue.Value = "{RES[" + wseTestStepValue.Value + "]}";
                    }

                    break;
                }

                if (wseTestStepValue.SpecializationModule.Name == "Plain Text") {
                    if (wseTestStepValue.SubValues != null && wseTestStepValue.SubValues.Any()) {
                        apiTestStepValue.DataType = wseTestStepValue.SubValues.First().DataType;
                        apiTestStepValue.ActionMode = wseTestStepValue.SubValues.First().ActionMode;
                        //if (StringExtensions.IsNullOrBlank(wTestStepValue)) 
                        apiTestStepValue.Value =
                                wseTestStepValue.SubValues.FirstOrDefault(x => x.Name == "Value")?.Value;
                    }

                    if (!string.IsNullOrEmpty(apiTestStepValue.Value))
                        apiTestStepValue.Value = CommonUtilities.RemoveExtraDoubleQuotes(apiTestStepValue.Value);
                    break;
                }

                var wTestStepValue = wseTestStepValue.SubValues.FirstOrDefault(x => x.Name == "Filepath")?.Value;
                if (!wTestStepValue.IsNullOrBlankInTosca()) {
                    apiTestStepValue.ActionProperty = $"File";
                    apiTestStepValue.Operator = Operator.Equals;
                    apiTestStepValue.Value = wTestStepValue;
                    apiTestStepValue.ActionMode = XTestStepActionMode.Select;
                }
            }
        }

        #endregion
    }
}