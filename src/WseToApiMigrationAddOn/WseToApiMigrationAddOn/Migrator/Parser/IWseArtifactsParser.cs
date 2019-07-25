using System.Collections.Generic;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Parser {
    public interface IWseArtifactsParser {
        #region Public Properties

        string Endpoint { get; set; }

        int HashCode { get; set; }

        Dictionary<string, string> Headers { get; }

        string Method { get; set; }

        Dictionary<string, string> PathParams { get; }

        Dictionary<string, string> QueryParams { get; }

        string RequestPayload { get; set; }

        string Resource { get; set; }

        Dictionary<string, string> ResponseHeaders { get; }

        string ResponsePayload { get; set; }

        string ResponseStatus { get; set; }

        #endregion
    }
}