using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Parser;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Parser.Interfaces;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Factory;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Interfaces;
using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator {
    /// <summary>
    /// Migrates WSE artifacts which are created using "Communicate with Web service (REST/JSON)" standard module.
    /// </summary>
    public class CommunicateWithWebServiceJsonArtifactMigrator : AbstractCommunicateWithWebServiceArtifactMigrator {
        #region Constructors and Destructors

        public CommunicateWithWebServiceJsonArtifactMigrator(XModule wseModule)
                : base(wseModule) {
        }

        #endregion

        #region Public Properties

        public override IPayloadParser PayloadParser => new JsonPayloadParser();

        #endregion

        #region Properties

        protected override IPayloadSetterFactory PayloadSetterFactory => new JsonPayloadSetterFactory();

        #endregion

        #region Methods

        protected override bool IsModuleSearchCriteriaEmpty(IWseArtifactsParser parserResult) {
            return string.IsNullOrEmpty(parserResult.Endpoint);
        }

        #endregion
    }
}