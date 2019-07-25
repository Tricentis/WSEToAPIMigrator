using System.Collections.Generic;

namespace WseToApiMigrationAddOn.Migrator.Parser.Model {
    public class AddressParserResult {
        #region Public Properties

        public string Endpoint { get; set; } = string.Empty;

        public Dictionary<string, string> QueryParams { get; set; } = new Dictionary<string, string>();

        public string Resource { get; set; } = string.Empty;

        #endregion
    }
}