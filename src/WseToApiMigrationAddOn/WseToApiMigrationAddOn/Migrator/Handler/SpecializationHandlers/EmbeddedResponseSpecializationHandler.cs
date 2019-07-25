using Tricentis.TCAPIObjects.Objects;

using WseToApiMigrationAddOn.Extensions;
using WseToApiMigrationAddOn.Helper;
using WseToApiMigrationAddOn.Migrator.Parser.Interfaces;
using WseToApiMigrationAddOn.Migrator.Setter.Interfaces;

namespace WseToApiMigrationAddOn.Migrator.Handler.SpecializationHandlers {
    /// <summary>
    /// Handles Web service response data in XML/JSON embedded specialization
    /// </summary>
    public class EmbeddedResponseSpecializationHandler : AbstractSpecializationHandler {
        #region Public Methods and Operators
        /// <summary>
        /// Handles Web service response data in XML/JSON embedded specialization
        /// </summary>
        /// <param name="wseTestStep">TestStep of WSE Artifacts</param>
        /// <param name="apiTestStep">TestStep of API Engine Artifacts</param>
        /// <param name="payloadParser">Parse xml and json payload from Wse XModules and TestSteps</param>
        /// <param name="payloadSetterFactory">Create module attributes and Set values for payload in for Api artifacts </param> 
        public override void HandleSpecialization(XTestStep wseTestStep,
                                                  XTestStep apiTestStep,
                                                  IPayloadParser payloadParser,
                                                  IPayloadSetterFactory payloadSetterFactory) {
            if (wseTestStep.TestStepValues == null) return;

            if (SpecializationHelper.IsResponseIsUsingEmbeddedModule(wseTestStep,
                                                                     out XModule responseSpecializationModule)) {
                var responsePayload =
                        payloadParser.Parse(responseSpecializationModule, AddOnConstants.TestStepXmlPayloadTql);
                var apiModule = (apiTestStep.Module as ApiModule);
                apiModule?.APISetMessagePayload(responsePayload);
            }
            else {
                successor?.HandleSpecialization(wseTestStep, apiTestStep, payloadParser, payloadSetterFactory);
            }
        }

        #endregion
    }
}