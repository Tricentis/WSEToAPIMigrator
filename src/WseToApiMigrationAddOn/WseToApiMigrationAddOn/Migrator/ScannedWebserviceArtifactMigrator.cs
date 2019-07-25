using System.Linq;

using Tricentis.Automation.WseToApiMigrationAddOn.Helper;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Handler;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Interfaces;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Parser;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Factory;
using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator {
    /// <summary>
    /// Migrates WSE Artifacts created using NON-UI Scan.
    /// </summary>
    public class ScannedWebserviceArtifactMigrator : IMigrator {
        #region Fields

        private readonly XModule wseModule;

        #endregion

        #region Constructors and Destructors

        public ScannedWebserviceArtifactMigrator(XModule wseModule) {
            this.wseModule = wseModule;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Migrates the Wse Module and all referencing XTestSteps.
        /// </summary>
        /// <param name="objectToExecuteOn">Only WSE XTestSteps and XModules which are present under this object will be migrated.</param>
        public void Migrate(TCObject objectToExecuteOn) {
            var wseTestSteps = CommonUtilities.GetFilteredWseTestSteps(objectToExecuteOn, wseModule.TestSteps);
            if (!wseTestSteps.Any()) {
                WseModuleParser wseParser = new WseModuleParser();
                wseParser.Parse(wseModule);
                FolderStructureHandler folderStructureHandler = new FolderStructureHandler();
                TCFolder apiModuleFolder = folderStructureHandler.CreateFolderForApiModules(wseModule);
                ApiModuleHandler.CreateApiModulePair(apiModuleFolder, wseModule.Name, wseParser);
            }
            else {
                foreach (var wseTestStep in wseTestSteps) {
                    WseTestStepParser wseTestStepParser = new WseTestStepParser();
                    wseTestStepParser.Parse(wseTestStep, new XmlPayloadParser());
                    (ApiModule requestApiModule, ApiModule responseApiModule) =
                            GetExistingOrCreateNewApiModulePair(objectToExecuteOn, wseTestStep, wseTestStepParser);

                    WseTestStepMigrator wseTestStepMigrator = new WseTestStepMigrator();
                    wseTestStepMigrator.Migrate(objectToExecuteOn,
                                                requestApiModule,
                                                responseApiModule,
                                                new XmlPayloadParser(),
                                                new XmlPayloadSetterFactory(),
                                                wseTestStep);
                }
            }
        }

        #endregion

        #region Methods

        private (ApiModule requestApiModule, ApiModule responseApiModule) GetExistingOrCreateNewApiModulePair(
                TCObject objectToExecuteOn,
                XTestStep wseTestStep,
                IWseArtifactsParser wseParser) {
            ApiModule requestApiModule = null;
            ApiModule responseApiModule = null;
            (requestApiModule, responseApiModule) =
                    CommonUtilities.SearchExistingApiModule(objectToExecuteOn, wseParser, wseTestStep);
            if (requestApiModule != null) return (requestApiModule, responseApiModule);

            FolderStructureHandler folderStructureHandler = new FolderStructureHandler();
            TCFolder apiModuleFolder = folderStructureHandler.CreateFolderForApiModules(wseModule);
            return ApiModuleHandler.CreateApiModulePair(apiModuleFolder, wseModule.Name, wseParser);
        }

        #endregion
    }
}