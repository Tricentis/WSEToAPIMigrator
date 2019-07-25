using System;
using System.Collections.Generic;
using System.Text;

using Tricentis.Automation.WseToApiMigrationAddOn.Extensions;
using Tricentis.Automation.WseToApiMigrationAddOn.Helper;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Parser.Interfaces;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Parser.Model;
using Tricentis.Automation.WseToApiMigrationAddOn.Shared;
using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Parser {
    /// <summary>
    /// Extract data required for creation of Api Module creation from WSE TestStep.
    /// </summary>
    public class WseTestStepParser : IWseArtifactsParser {
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
        /// Parse WSE XTestStep to get data.
        /// </summary>
        /// <param name="xTestStep">WSE XTestStep</param>
        /// <param name="payloadParser">XML or Json Payload Parser.
        /// The request payload is extracted from XTestStep.
        /// The response paylod is extracted from Module.
        /// </param>
        public void Parse(XTestStep xTestStep, IPayloadParser payloadParser) {
            try {
                var methodParser = new MethodParser();
                Method = methodParser.Parse(xTestStep);

                var addressParser = new AddressParser();
                AddressParserResult addressParserResult = addressParser.Parse(xTestStep);
                Endpoint = addressParserResult.Endpoint;
                Resource = addressParserResult.Resource;
                QueryParams = addressParserResult.QueryParams;

                var headerParser = new HeaderParser();
                Headers = headerParser.Parse(xTestStep, AddOnConstants.TestStepRequestHeadersTql);
                Headers = CommonUtilities.ModifyContentTypeToEmpty(Headers);
                ResponseHeaders = headerParser.Parse(xTestStep, AddOnConstants.TestStepResponseHeadersTql);

                var statusCodeParser = new StatusCodeParser();
                ResponseStatus = statusCodeParser.ParseResponseStatus(xTestStep);

                RequestPayload = payloadParser.Parse(xTestStep,
                                                     "=>SUBPARTS:XTestStepValue[Name==\"Request\"]->SUBPARTS:XTestStepValue");
                ResponsePayload = payloadParser.Parse(xTestStep.Module, AddOnConstants.ResponsePayloadTql);
                HashCode = GetHashCode(xTestStep);
            }
            catch (Exception e) {
                FileLogger.Instance.Error(e);
            }
        }

        #endregion

        #region Methods

        private static int GetHashCode(XTestStep xTestStep) {
            var stringBuilder = new StringBuilder();
            var treeObjects = xTestStep.Search("=>SUBPARTS:XTestStepValue[Name==\"Request\"]=>SUBPARTS");
            foreach (var t in treeObjects) {
                stringBuilder.Append(t.DisplayedName);
            }

            return stringBuilder.ToString().GetHashCode();
        }

        #endregion
    }
}