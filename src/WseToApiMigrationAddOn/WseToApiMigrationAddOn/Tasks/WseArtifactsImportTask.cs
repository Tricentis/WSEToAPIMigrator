using System;

using Tricentis.Automation.WseToApiMigrationAddOn.Extensions;
using Tricentis.Automation.WseToApiMigrationAddOn.Helper;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Factory;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Interfaces;
using Tricentis.Automation.WseToApiMigrationAddOn.Shared;
using Tricentis.TCAddOns;
using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Tasks {
    /// <summary>
    /// Migration task for Migrating WSE Artifacts to API Engine Artifacts.
    /// </summary>
    public abstract class WseArtifactsImportTask : TCAddOnTask {
        #region Public Properties

        public override Type ApplicableType { get; }

        public override string Name => "WSE to API";

        #endregion

        #region Public Methods and Operators

        public override TCObject Execute(TCObject objectToExecuteOn, TCAddOnTaskContext taskContext) {
            BackupProject.CreateBackup(objectToExecuteOn);
            DoImport(objectToExecuteOn);
            return null;
        }

        #endregion

        #region Methods

        private void DoImport(TCObject objectToExecuteOn) {
            foreach (XModule wseModule in objectToExecuteOn.GetWseModules()) {
                try {
                    IMigrator migrator = MigratorFactory.GetMigrator(wseModule);
                    migrator.Migrate(objectToExecuteOn);
                }
                catch (Exception e) {
                    FileLogger.Instance.Error(
                            $"Migration of WSE Module 'migration for WSE Module :'{wseModule.DisplayedName}' failed due an error. This might leave the module in a inconsistent state.",
                            e);
                }
            }

            CommonUtilities.ReplaceResourceWithLastResponseResource(objectToExecuteOn);
        }

        #endregion
    }

    internal class ImportAtComponentFolderTask : WseArtifactsImportTask {
        #region Public Properties

        public override Type ApplicableType => typeof(TCComponentFolder);

        #endregion
    }

    internal class ImportAtProjectRootTask : WseArtifactsImportTask {
        #region Public Properties

        public override Type ApplicableType => typeof(TCProject);

        #endregion
    }
}