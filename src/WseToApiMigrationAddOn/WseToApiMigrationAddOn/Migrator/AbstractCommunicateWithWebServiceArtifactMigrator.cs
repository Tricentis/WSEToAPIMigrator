using System;

using Tricentis.TCAPIObjects.Objects;

using WseToApiMigrationAddOn.Helper;
using WseToApiMigrationAddOn.Migrator.Handler;
using WseToApiMigrationAddOn.Migrator.Interfaces;
using WseToApiMigrationAddOn.Migrator.Parser;
using WseToApiMigrationAddOn.Migrator.Parser.Interfaces;
using WseToApiMigrationAddOn.Migrator.Setter.Interfaces;
using WseToApiMigrationAddOn.Shared;

namespace WseToApiMigrationAddOn.Migrator {
    /// <summary>
    /// Abstract Migrator for migrating WSE Artifacts created using "CommunicateWithWebService" Standard Module.
    /// </summary>
    public abstract class AbstractCommunicateWithWebServiceArtifactMigrator : IMigrator {
        #region Constructors and Destructors

        protected AbstractCommunicateWithWebServiceArtifactMigrator(XModule wseModule) {
            WseModule = wseModule;
        }

        #endregion

        #region Public Properties

        public abstract IPayloadParser PayloadParser { get; }

        public XModule WseModule { get; }

        #endregion

        #region Properties

        protected abstract IPayloadSetterFactory PayloadSetterFactory { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Migrates the Wse Module and all referencing XTestSteps.
        /// </summary>
        /// <param name="objectToExecuteOn">Only WSE XTestSteps and XModules which are present under this objectToExecuteOn object will be migrated.</param>
        public void Migrate(TCObject objectToExecuteOn) {
            foreach (var wseTestStep in CommonUtilities.GetFilteredWseTestSteps(objectToExecuteOn, WseModule.TestSteps)
            ) {
                try {
                    FileLogger.Instance.Debug(
                            $"Started migration for WSE TestStep : 'Name: {wseTestStep.Name}' NodePath:'{wseTestStep.NodePath}'");

                    var testStepParser = new WseTestStepParser();
                    testStepParser.Parse(wseTestStep, PayloadParser);

                    (ApiModule requestApiModule, ApiModule responseApiModule) =
                            GetExistingOrCreateNewApiModulePair(objectToExecuteOn, wseTestStep, testStepParser);
                    WseTestStepMigrator wseTestStepMigrator = new WseTestStepMigrator();
                    wseTestStepMigrator.Migrate(objectToExecuteOn,
                                                requestApiModule,
                                                responseApiModule,
                                                PayloadParser,
                                                PayloadSetterFactory,
                                                wseTestStep
                    );
                    FileLogger.Instance.Debug(
                            $"Completed migration for WSE TestStep : 'Name: {wseTestStep.Name}' NodePath:'{wseTestStep.NodePath}'");
                }
                catch (Exception e) {
                    FileLogger.Instance.Error(e);
                }
            }
        }

        #endregion

        #region Methods

        protected abstract bool IsModuleSearchCriteriaEmpty(IWseArtifactsParser parserResult);

        private (ApiModule requestApiModule, ApiModule responseApiModule) GetExistingOrCreateNewApiModulePair(
                TCObject objectToExecuteOn,
                XTestStep wseTestStep,
                IWseArtifactsParser wseParser) {
            ApiModule requestApiModule = null;
            ApiModule responseApiModule = null;

            if (!IsModuleSearchCriteriaEmpty(wseParser))
                (requestApiModule, responseApiModule) =
                        CommonUtilities.SearchExistingApiModule(objectToExecuteOn, wseParser, wseTestStep);

            if (requestApiModule != null) return (requestApiModule, responseApiModule);

            FolderStructureHandler folderStructureHandler = new FolderStructureHandler();
            (TCFolder apiModuleFolder, string moduleName) =
                    folderStructureHandler.CreateFolderForApiModules(objectToExecuteOn, wseTestStep, wseParser);
            return ApiModuleHandler.CreateApiModulePair(apiModuleFolder, moduleName, wseParser);
        }

        #endregion
    }
}