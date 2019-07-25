using Tricentis.TCAPIObjects.Objects;

namespace WseToApiMigrationAddOn.Migrator.Setter.Interfaces {
    public interface IApiValueSetter {
        #region Public Methods and Operators

        void Execute(XTestStep apiTestStep, XTestStep wseTestStep);

        #endregion
    }
}