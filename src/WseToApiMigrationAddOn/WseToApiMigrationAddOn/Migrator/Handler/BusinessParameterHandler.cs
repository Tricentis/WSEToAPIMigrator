using System;
using System.Collections.Generic;
using System.Linq;

using Tricentis.TCAPIObjects.Objects;

using WseToApiMigrationAddOn.Extensions;
using WseToApiMigrationAddOn.Extensions.Tricentis.Automation.Api.Core.Extensions;
using WseToApiMigrationAddOn.Helper;

namespace WseToApiMigrationAddOn.Migrator.Handler {
    /// <summary>
    /// Creates and set module attributes at module and teststep
    /// </summary>
    public static class BusinessParameterHandler {
        #region Public Methods and Operators

        /// <summary>
        /// Creates and set module attributes at module and teststep
        /// </summary>
        /// <param name="wseTestStepValue">TestStepValues of WSE Teststep</param>
        /// <param name="tcPath">XPath and JsonPath for xml and Json respectively</param>
        /// <param name="xModule">xModule and xModuleAttribute of ApiEngine</param>
        /// <param name="xModuleDemo">ApiModule</param>
        /// <param name="parameterPathType">Enum which define creation of xml and json moduleattributes</param>
        /// <param name="parentTest">TestStep and TestStepValues for ApiTestStep</param>
        /// <param name="cardinality">Cardinality of WSE XModule</param>
        /// <param name="isArray">True if Wse Xmodule attributes is array</param>
        /// <returns></returns>
        public static XTestStepValue CreateBusinessParameterInTosca(XTestStepValue wseTestStepValue,
                                                                    string tcPath,
                                                                    dynamic xModule,
                                                                    ApiModule xModuleDemo,
                                                                    BusinessParameterPathTypes parameterPathType,
                                                                    dynamic parentTest,
                                                                    string cardinality,
                                                                    bool isArray) {
            string businessParameterName = wseTestStepValue.Name;
            string businessParameterValue = wseTestStepValue.Value == "{NULL}" ? string.Empty : wseTestStepValue.Value;
            XTestStepActionMode xTestStepActionMode = wseTestStepValue.ActionModeToUse;
            ModuleAttributeDataType attributeDataType = wseTestStepValue.DataType;

            var mAttributes = xModuleDemo.Search("=>SUBPARTS:XModuleAttribute").Cast<XModuleAttribute>();

            XModuleAttribute xModuleAttribute = GetExistingXModuleAttribute(mAttributes, tcPath, parameterPathType);

            if (xModuleAttribute != null) {
                if (!string.IsNullOrEmpty(xModuleAttribute.ValueRange)) {
                    if (!xModuleAttribute.ValueRange.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                         .Contains(businessParameterValue)) {
                        xModuleAttribute.ValueRange =
                                $"{xModuleAttribute.ValueRange};{businessParameterValue}";
                    }
                }
                else {
                    xModuleAttribute.ValueRange =
                            $"{businessParameterValue}";
                }
            }
            else {
                xModuleAttribute = xModule.CreateModuleAttribute();
                xModuleAttribute.Name = businessParameterName;
                xModuleAttribute.AddXParamToModuleAttribute("PathType",
                                                            parameterPathType == BusinessParameterPathTypes.XPATH
                                                                    ? "XPath"
                                                                    : parameterPathType
                                                                      == BusinessParameterPathTypes.JSONPATH
                                                                            ? "JsonPath"
                                                                            : "",
                                                            ParamTypeE.TechnicalID);
                xModuleAttribute.AddXParamToModuleAttribute("Path",
                                                            tcPath,
                                                            ParamTypeE.TechnicalID);
                xModuleAttribute.AddXParamToModuleAttribute("ExplicitName",
                                                            "True",
                                                            ParamTypeE.Configuration);
                xModuleAttribute.DefaultActionMode = xTestStepActionMode;
                xModuleAttribute.DefaultDataType = wseTestStepValue.ModuleAttribute.DefaultDataType;
                xModuleAttribute.DefaultValue = wseTestStepValue.ModuleAttribute?.DefaultValue == "{NULL}"
                                                        ? string.Empty
                                                        : wseTestStepValue.ModuleAttribute?.DefaultValue;
                xModuleAttribute.ValueRange = $"{businessParameterValue}";
                xModuleAttribute.Cardinality = isArray ? "0-N" : cardinality;
                xModuleAttribute.EnsureUniqueName();
            }

            XTestStepValue testStepValue = parentTest.CreateXTestStepValue(xModuleAttribute);
            testStepValue.ActionProperty = wseTestStepValue.ActionProperty;
            testStepValue.Operator = wseTestStepValue.Operator;
            testStepValue.Value = wseTestStepValue.Value;
            testStepValue.ActionMode = xTestStepActionMode;
            testStepValue.DataType = attributeDataType;
            testStepValue.Disabled = wseTestStepValue.Disabled;
            SetConstraintIndexProperty(wseTestStepValue, testStepValue);
            return testStepValue;
        }

        #endregion

        #region Methods

        private static XModuleAttribute GetExistingXModuleAttribute(IEnumerable<XModuleAttribute> mAttributes,
                                                                    string tcpath,
                                                                    BusinessParameterPathTypes parameterPathType) {
            IEnumerable<XModuleAttribute> xModuleAttributes = mAttributes.ToList();
            if (mAttributes != null && xModuleAttributes.Any()) {
                foreach (var mAttribute in xModuleAttributes) {
                    var existingXPath = mAttribute
                                        .XParams.Where(x => x.ParamType == ParamTypeE.TechnicalID && x.Name == "Path")
                                        .Select(x => x.Value).FirstOrDefault();
                    if (parameterPathType == BusinessParameterPathTypes.JSONPATH) {
                        if (existingXPath == tcpath) {
                            return mAttribute;
                        }
                    }
                    else if (parameterPathType == BusinessParameterPathTypes.XPATH) {
                        if (existingXPath == tcpath) {
                            return mAttribute;
                        }
                    }
                    else {
                        return null;
                    }
                }
            }

            return null;
        }

        private static void SetConstraintIndexProperty(XTestStepValue wseTestStepValue, XTestStepValue testStepValue) {
            string constraintIndex =
                    wseTestStepValue.RelevantXParams.FirstOrDefault(x => x.Name == "ConstraintIndex")?.Value;
            if (!StringExtensions.IsNullOrBlankInTosca(constraintIndex)) {
                testStepValue.ActionProperty = $"Index";
                testStepValue.Operator = Operator.Equals;
                testStepValue.Value = constraintIndex;
            }
        }

        #endregion
    }
}