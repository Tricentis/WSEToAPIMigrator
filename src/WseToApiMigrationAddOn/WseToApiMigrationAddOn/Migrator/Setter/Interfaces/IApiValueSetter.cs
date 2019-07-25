using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Interfaces {
    public interface IApiValueSetter {
        #region Public Methods and Operators

        void Execute(XTestStep apiTestStep, XTestStep wseTestStep);

        #endregion
    }
}