namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Handler.SpecializationHandlers {
    /// <summary>
    /// Handles Web service response data in Json Plain Text specialization 
    /// </summary>
    public class ResponseAsPlainTextSpecializationHandler : AbstractResourceSpecializationHandler {
        #region Properties

        protected override string TqlToGetWseTestStepSpecializationModule =>
                "=>SUBPARTS:XTestStepValue->SpecializationModule[Name==\"Plain Text\"]";

        protected override string TqlToGetWseTestStepValue => "=>SUBPARTS:XTestStepValue[Name==\"Response\"]";

        #endregion
    }
}