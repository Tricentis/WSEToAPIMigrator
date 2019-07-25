using Tricentis.TCAddOns;

namespace Tricentis.Automation.WseToApiMigrationAddOn {
    /// <summary>
    /// TCAddOn for migrating WSE Engine Artifacts to API Engine Artifacts (Supported Tosca Versions 11.3,12.0,12.1,12.2)
    /// </summary>
    public class WseToApiMigrationAddOn : TCAddOn {
        #region Public Properties

        public override string DisplayedName => "Migration";

        public override string UniqueName => "WseToApiMigrationAddOn";

        #endregion
    }
}