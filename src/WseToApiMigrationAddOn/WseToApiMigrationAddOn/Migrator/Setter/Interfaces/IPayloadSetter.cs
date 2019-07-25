using Tricentis.TCAPIObjects.Objects;

namespace WseToApiMigrationAddOn.Migrator.Setter.Interfaces {
    public interface IPayloadSetter {
        #region Public Methods and Operators

        void Execute(XTestStep apiTestStep, XTestStep wseTestStep);

        #endregion
    }
}