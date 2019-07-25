using System.Collections.Generic;
using System.Linq;

using Tricentis.TCAPIObjects.Objects;

using WseToApiMigrationAddOn.Migrator.Parser.Interfaces;
using WseToApiMigrationAddOn.Migrator.Setter.Interfaces;

namespace WseToApiMigrationAddOn.Migrator.Handler.SpecializationHandlers {
    /// <summary>
    /// Handles Web service request/response data in Xml/JSON Resource specialization
    /// Handles Web service request/response data in Json Plain Text specialization  
    /// </summary>
    public abstract class AbstractResourceSpecializationHandler : AbstractSpecializationHandler {
        #region Properties

        protected abstract string TqlToGetWseTestStepSpecializationModule { get; }

        protected abstract string TqlToGetWseTestStepValue { get; }

        #endregion

        #region Public Methods and Operators
        /// <summary>
        /// Handles Web service request/response data in Xml/JSON Resource specialization
        /// </summary>
        /// <param name="wseTestStep">TestStep of WSE Artifacts</param>
        /// <param name="apiTestStep">TestStep of API Engine Artifacts</param>
        /// <param name="payloadParser">Parse xml and json payload from Wse XModules and TestSteps</param>
        /// <param name="payloadSetterFactory">Create module attributes and Set values for payload in for Api artifacts </param>  
        public override void HandleSpecialization(XTestStep wseTestStep,
                                                  XTestStep apiTestStep,
                                                  IPayloadParser payloadParser,
                                                  IPayloadSetterFactory payloadSetterFactory) {
            XModule specializationModule =
                    (XModule)wseTestStep.Search(TqlToGetWseTestStepSpecializationModule)?.FirstOrDefault();

            if (specializationModule != null) {
                var wseTestStepValue = (XTestStepValue)wseTestStep.Search(TqlToGetWseTestStepValue)?.FirstOrDefault();
                BodyParamHandler.CreateBodyParameterForRequestAndResponse(
                        apiTestStep,
                        new List<XTestStepValue>() { wseTestStepValue });
            }
            else {
                successor?.HandleSpecialization(wseTestStep, apiTestStep, payloadParser, payloadSetterFactory);
            }
        }

        #endregion
    }
}