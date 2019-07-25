using System;
using System.IO;

using Tricentis.TCAPIObjects.Objects;

namespace WseToApiMigrationAddOn.Helper {
    /// <summary>
    /// Creation of backup of workspace.
    /// </summary>
    public static class BackupProject {
        #region Public Methods and Operators

        /// <summary>
        /// Takes backup at project or component level.
        /// </summary>
        /// <param name="tcObject"></param>
        public static void CreateBackup(TCObject tcObject) {
            string backupPath = Environment.GetEnvironmentVariable("TRICENTIS_ALLUSERS_APPDATA")
                                + "\\Automation\\WSEToAPIMigrationBackup";
            if (!Directory.Exists(backupPath)) {
                Directory.CreateDirectory(backupPath);
            }

            backupPath += "\\Backup-" + Guid.NewGuid().ToString() + ".tsu";
            TCObject backupObject = (tcObject is TCProject project)
                                            ? project.ExportSubset(backupPath)
                                            : (tcObject as TCComponentFolder).ExportSubset(backupPath);
        }

        #endregion
    }
}