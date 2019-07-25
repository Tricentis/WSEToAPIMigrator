using System.Collections.Generic;
using System.Linq;

using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Helper {
    /// <summary>
    /// Identify whether WSE XtestStepvalues(Request/Response) is using any type specialization
    /// </summary>
    public class SpecializationHelper {
        #region Public Methods and Operators

        /// <summary>
        /// Identify whether WSE XtestStepvalues(Request) is using Web service request data in JSON Resource specialization
        /// Identify whether WSE XtestStepvalues(Request) is using Web service request data in XML Resource specialization
        /// </summary>
        /// <param name="testStep">Wse TestStep</param>
        /// <param name="embeddedModule">Specialization Module for Request</param>
        /// <returns>true if xmodule is using specialization</returns>
        public static bool IsRequestIsUsingEmbeddedModule(XTestStep testStep, out XModule embeddedModule) {
            var name = "Request";
            var specializationModulesToExclude = new List<string>() {
                    "Web service request data in JSON Resource",
                    "Web service request data in XML Resource"
            };
            return IsUsingEmbeddedModule(testStep, name, specializationModulesToExclude, out embeddedModule);
        }

        /// <summary>
        /// Identify whether WSE XtestStepvalues(Response) is using Web service response data in JSON Resource specialization
        /// Identify whether WSE XtestStepvalues(Response) is using Web service response data in XML Resource specialization
        /// </summary>
        /// <param name="testStep">Wse TestStep</param>
        /// <param name="embeddedModule">Specialization Module for response</param>
        /// <returns>true if xmodule is using specialization</returns>
        public static bool IsResponseIsUsingEmbeddedModule(XTestStep testStep, out XModule embeddedModule) {
            var name = "Response";
            var specializationModulesToExclude = new List<string>() {
                    "Web service response data in JSON Resource",
                    "Web service response data in XML Resource",
                    "Plain Text"
            };
            return IsUsingEmbeddedModule(testStep, name, specializationModulesToExclude, out embeddedModule);
        }

        /// <summary>
        /// Identify whether WSE XtestStepvalues(Request/Response) is using Web service request/response data in JSON Resource specialization
        /// Identify whether WSE XtestStepvalues(Request/Response) is using Web service request/response data in XML Resource specialization
        /// </summary>
        /// <param name="wseTestStep">Wse TestStep</param>
        /// <param name="tcFolder">Specialization Module</param>
        /// <returns>true if xmodule is using specialization</returns>
        public static bool IsUsingEmbeddedResource(XTestStep wseTestStep, out TCFolder tcFolder) {
            if (IsRequestIsUsingEmbeddedModule(wseTestStep, out XModule requestSpecializationModule)) {
                tcFolder = (TCFolder)requestSpecializationModule.ParentFolder;
                return true;
            }

            if (IsResponseIsUsingEmbeddedModule(wseTestStep, out XModule responseSpecializationModule)) {
                tcFolder = (TCFolder)responseSpecializationModule.ParentFolder;
                return true;
            }

            tcFolder = null;
            return false;
        }

        #endregion

        #region Methods

        private static bool IsUsingEmbeddedModule(XTestStep testStep,
                                                  string name,
                                                  List<string> specializationModulesToExclude,
                                                  out XModule embeddedModule) {
            var module = testStep.TestStepValues
                                 .FirstOrDefault(x => x.Name == name && x.SpecializationModule != null
                                                                     && !specializationModulesToExclude.Contains(
                                                                             x.SpecializationModule.Name))
                                 ?.SpecializationModule;

            if (module != null) {
                embeddedModule = module;
                return true;
            }

            embeddedModule = null;
            return false;
        }

        #endregion
    }
}