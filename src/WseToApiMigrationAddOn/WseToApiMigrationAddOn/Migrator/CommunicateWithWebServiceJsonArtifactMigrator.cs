using Tricentis.TCAPIObjects.Objects;

using WseToApiMigrationAddOn.Migrator.Parser;
using WseToApiMigrationAddOn.Migrator.Parser.Interfaces;
using WseToApiMigrationAddOn.Migrator.Setter.Factory;
using WseToApiMigrationAddOn.Migrator.Setter.Interfaces;

namespace WseToApiMigrationAddOn.Migrator {
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