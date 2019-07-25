using Tricentis.TCAPIObjects.Objects;

using WseToApiMigrationAddOn.Helper;
using WseToApiMigrationAddOn.Migrator.Parser;
using WseToApiMigrationAddOn.Migrator.Parser.Interfaces;
using WseToApiMigrationAddOn.Migrator.Setter.Factory;
using WseToApiMigrationAddOn.Migrator.Setter.Interfaces;

namespace WseToApiMigrationAddOn.Migrator {
    /// <summary>
    /// Migrates WSE artifacts which are created using "Communicate with Web service" standard module.
    /// </summary>
    public class CommunicateWithWebServiceXmlArtifactMigrator : AbstractCommunicateWithWebServiceArtifactMigrator {
        #region Constructors and Destructors

        public CommunicateWithWebServiceXmlArtifactMigrator(XModule wseModule)
                : base(wseModule) {
        }

        #endregion

        #region Public Properties

        public override IPayloadParser PayloadParser => new XmlPayloadParser();

        #endregion

        #region Properties

        protected override IPayloadSetterFactory PayloadSetterFactory => new XmlPayloadSetterFactory();

        #endregion

        #region Methods

        protected override bool IsModuleSearchCriteriaEmpty(IWseArtifactsParser parserResult) {
            return string.IsNullOrEmpty(CommonUtilities.GetSoapAction(parserResult.Headers));
        }

        #endregion
    }
}