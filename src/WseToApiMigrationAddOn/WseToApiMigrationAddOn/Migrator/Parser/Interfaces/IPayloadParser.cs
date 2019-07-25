using Tricentis.TCAPIObjects.Objects;

namespace WseToApiMigrationAddOn.Migrator.Parser.Interfaces {
    public interface IPayloadParser {
        #region Public Methods and Operators

        string Parse(XModule wseModule, string tql = "");

        string Parse(XTestStep xTestStep, string tql = "");

        #endregion
    }
}