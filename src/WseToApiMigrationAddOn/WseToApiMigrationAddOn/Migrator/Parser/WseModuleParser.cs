using System;
using System.Collections.Generic;
using System.Text;

using Tricentis.Automation.WseToApiMigrationAddOn.Extensions;
using Tricentis.Automation.WseToApiMigrationAddOn.Helper;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Parser.Model;
using Tricentis.Automation.WseToApiMigrationAddOn.Shared;
using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Parser {
    /// <summary>
    /// Extracts data required for creation of Api Module creation from WSE Module.
    /// </summary>
    public class WseModuleParser : IWseArtifactsParser {
        #region Public Properties

        public string Endpoint { get; set; }

        public int HashCode { get; set; }

        public Dictionary<string, string> Headers { get; private set; } = new Dictionary<string, string>();

        public string Method { get; set; }

        public Dictionary<string, string> PathParams { get; } = new Dictionary<string, string>();

        public Dictionary<string, string> QueryParams { get; private set; } = new Dictionary<string, string>();

        public string RequestPayload { get; set; }

        public string Resource { get; set; }

        public Dictionary<string, string> ResponseHeaders { get; private set; } = new Dictionary<string, string>();

        public string ResponsePayload { get; set; }

        public string ResponseStatus { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Parses WSE XModule to get data.
        /// </summary>
        /// <param name="wseModule">WSE XModule</param>
        public void Parse(XModule wseModule) {
            try {
                var methodParser = new MethodParser();
                Method = methodParser.Parse(wseModule);

                var addressParser = new AddressParser();
                AddressParserResult addressParserResult = addressParser.Parse(wseModule);
                Endpoint = addressParserResult.Endpoint;
                Resource = addressParserResult.Resource;
                QueryParams = addressParserResult.QueryParams;

                var headerParser = new HeaderParser();
                Headers = headerParser.Parse(wseModule, AddOnConstants.RequestHeadersTql);
                Headers = CommonUtilities.ModifyContentTypeToEmpty(Headers);
                ResponseHeaders = headerParser.Parse(wseModule, AddOnConstants.ResponseHeadersTql);

                var payloadParser = new XmlPayloadParser();
                RequestPayload = payloadParser.Parse(wseModule, AddOnConstants.RequestPayloadTql);
                ResponsePayload = payloadParser.Parse(wseModule, AddOnConstants.ResponsePayloadTql);

                var statusCodeParser = new StatusCodeParser();
                ResponseStatus = statusCodeParser.ParseResponseStatus(wseModule);

                HashCode = GetHashCode(wseModule);
            }
            catch (Exception e) {
                FileLogger.Instance.Error(e);
            }
        }

        #endregion

        #region Methods

        private static int GetHashCode(XModule xModule) {
            var stringBuilder = new StringBuilder();
            var treeObjects = xModule.Search("=>SUBPARTS:XModuleAttribute[Name==\"Request\"]=>SubAttributes");
            foreach (var t in treeObjects) {
                stringBuilder.Append(t.DisplayedName);
            }

            return stringBuilder.ToString().GetHashCode();
        }

        #endregion
    }
}