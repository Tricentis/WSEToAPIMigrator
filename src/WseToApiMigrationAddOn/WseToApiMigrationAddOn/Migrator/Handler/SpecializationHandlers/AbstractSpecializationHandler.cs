using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Parser.Interfaces;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Interfaces;
using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Handler.SpecializationHandlers {
    /// <summary>
    /// Handles all types of Specialization for WSE Artifacts
    /// </summary>
    public abstract class AbstractSpecializationHandler {
        #region Fields

        protected AbstractSpecializationHandler successor;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Handles all types of Specialization for WSE TestStep
        /// </summary>
        /// <param name="wseTestStep">TestStep of WSE Artifacts</param>
        /// <param name="apiTestStep">TestStep of API Engine Artifacts</param>
        /// <param name="payloadParser">Parse xml and json payload from Wse XModules and TestSteps</param>
        /// <param name="payloadSetterFactory">Create module attributes and Set values for payload in for Api artifacts </param>
        public abstract void HandleSpecialization(XTestStep wseTestStep,
                                                  XTestStep apiTestStep,
                                                  IPayloadParser payloadParser,
                                                  IPayloadSetterFactory payloadSetterFactory);

        public void SetSuccessor(AbstractSpecializationHandler successor) {
            this.successor = successor;
        }

        #endregion
    }
}