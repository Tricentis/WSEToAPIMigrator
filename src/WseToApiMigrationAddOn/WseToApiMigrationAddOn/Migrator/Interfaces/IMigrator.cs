using Tricentis.TCAPIObjects.Objects;

namespace WseToApiMigrationAddOn.Migrator.Interfaces {
    public interface IMigrator {
        #region Public Methods and Operators

        void Migrate(TCObject objectToExecuteOn);

        #endregion
    }
}