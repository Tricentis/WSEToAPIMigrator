using System;
using System.Collections.Generic;
using System.Linq;

using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Handler;
using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Helper {
    /// <summary>
    /// Create Body Parameter for Request and Response Api Modules for Transform Request/Response Wse XTestStepValues
    /// </summary>
    public class CommonParserMethods {
        #region Public Methods and Operators

        /// <summary>
        /// Create Body Parameter for Request API Modules for Transform Request WSE XTestStepValues
        /// </summary>
        /// <param name="apiTestStep">TestStep of API Engine</param>
        /// <param name="wseTestStep">TestStep of WSE Engine</param>
        public static void FillTransformRequest(XTestStep apiTestStep, XTestStep wseTestStep) {
            try {
                List<XTestStepValue> wseTestStepValues = wseTestStep
                                                         .Search(
                                                                 "=>SUBPARTS:XTestStepValue[Name==\"Transform request\"]=>SubValues[Name==\"Request transformation\"]")
                                                         .Where(
                                                                 x => x.DisplayedName
                                                                      == "Request transformation: Save Request")
                                                         .Cast<XTestStepValue>().ToList();
                BodyParamHandler.CreateBodyParameterForRequestAndResponse(apiTestStep, wseTestStepValues);
            }
            catch (Exception) {
                //ignored
            }
        }

        /// <summary>
        /// Create Body Parameter for Response API Modules for Transform Response WSE XTestStepValues
        /// </summary>
        /// <param name="apiTestStep">TestStep of API Engine</param>
        /// <param name="wseTestStep">TestStep of WSE Engine</param>
        public static void FillTransformResponse(XTestStep apiTestStep, XTestStep wseTestStep) {
            try {
                List<XTestStepValue> wseTestStepValues = wseTestStep
                                                         .Search(
                                                                 "=>SUBPARTS:XTestStepValue[Name==\"Transform response\"]=>SubValues[Name==\"Response transformation\"]")
                                                         .Where(x => x.DisplayedName
                                                                     == "Response transformation: Save Response")
                                                         .Cast<XTestStepValue>()
                                                         .ToList();
                BodyParamHandler.CreateBodyParameterForRequestAndResponse(apiTestStep, wseTestStepValues);
            }
            catch (Exception) {
                //ignored
            }
        }

        #endregion
    }
}