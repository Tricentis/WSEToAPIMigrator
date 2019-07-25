using Tricentis.Automation.WseToApiMigrationAddOn.Helper;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Parser;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Parser.Interfaces;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Factory;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Interfaces;
using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator {
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