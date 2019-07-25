namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Handler.SpecializationHandlers {
    /// <summary>
    /// Handles Web service request data in Xml/JSON Resource specialization
    /// </summary>
    public class RequestAsResourceSpecializationHandler : AbstractResourceSpecializationHandler {
        #region Properties

        protected override string TqlToGetWseTestStepSpecializationModule =>
                "=>SUBPARTS:XTestStepValue->SpecializationModule[Name==\"Web service request data in XML Resource\" OR Name==\"Web service request data in JSON Resource\"]";

        protected override string TqlToGetWseTestStepValue => "=>SUBPARTS:XTestStepValue[Name==\"Request\"]";

        #endregion
    }
}