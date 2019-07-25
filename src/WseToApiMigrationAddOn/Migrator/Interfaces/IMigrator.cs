using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Interfaces {
    public interface IMigrator {
        #region Public Methods and Operators

        void Migrate(TCObject objectToExecuteOn);

        #endregion
    }
}