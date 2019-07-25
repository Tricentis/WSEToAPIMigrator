using System;

using Tricentis.Automation.WseToApiMigrationAddOn.Extensions;
using Tricentis.Automation.WseToApiMigrationAddOn.Helper;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Parser;
using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Handler {
    /// <summary>
    /// Create request and response module pair
    /// </summary>
    public static class ApiModuleHandler {
        #region Public Methods and Operators

        /// <summary>
        /// Creates request and response module pair
        /// </summary>
        /// <param name="apiModuleFolder">Folder path reference to create Request and Response module</param>
        /// <param name="moduleName">Name of Request and Response module</param>
        /// <param name="wseParser">Contains transport information like Endpoint, Headers, Payload used to fill in Request and Response Module</param>
        /// <returns></returns>
        public static (ApiModule, ApiModule) CreateApiModulePair(TCFolder apiModuleFolder,
                                                                 string moduleName,
                                                                 IWseArtifactsParser wseParser
        ) {
            string correlationId = Guid.NewGuid().ToString();
            ApiModule requestApiModule =
                    CreateRequestModule(apiModuleFolder,
                                        moduleName,
                                        wseParser,
                                        correlationId
                    );
            ApiModule responseApiModule =
                    CreateResponseModule(apiModuleFolder,
                                         $"{moduleName} Response",
                                         wseParser,
                                         correlationId
                    );
            return (requestApiModule, responseApiModule);
        }

        /// <summary>
        /// Creates request module 
        /// </summary>
        /// <param name="moduleFolder">Folder path reference to create Request module</param>
        /// <param name="name">Name of Request module</param>
        /// <param name="wseModuleParser">Contains transport information used to fill in Request Module</param>
        /// <param name="correlationId">Uniquely identify the Request Module</param>
        /// <returns></returns>
        public static ApiModule CreateRequestModule(TCFolder moduleFolder,
                                                    string name,
                                                    IWseArtifactsParser wseModuleParser,
                                                    string correlationId) {
            //Module Properties
            ApiModule requestApiModule =
                    SetModuleStandardProperties(moduleFolder,
                                                name,
                                                correlationId,
                                                ScanTag.GetRequestScanTag(wseModuleParser));

            //Request Properties
            requestApiModule.AddTechnicalIdParam("Direction", Direction.Out.ToString());
            requestApiModule.AddTechnicalIdParam("InactiveNodes", "Remove");
            requestApiModule.AddTechnicalIdParam("IsRequest", "True");
            requestApiModule.AddTechnicalIdParam("MessagesGenerated", "True");
            requestApiModule.AddConfigurationParam("Executor", "HttpSend");
            requestApiModule.AddTechnicalIdParam("Method", wseModuleParser.Method);
            requestApiModule.AddTechnicalIdParam("Endpoint", wseModuleParser.Endpoint);
            requestApiModule.AddTechnicalIdParam("Resource", wseModuleParser.Resource);
            requestApiModule.AddQueryParams(wseModuleParser.QueryParams);
            requestApiModule.AddPathParams(wseModuleParser.PathParams);
            requestApiModule.AddHeaders(wseModuleParser.Headers);
            if (!string.IsNullOrEmpty(wseModuleParser.RequestPayload))
                requestApiModule.APISetMessagePayload(wseModuleParser.RequestPayload);
            return requestApiModule;
        }

        /// <summary>
        /// Creates response module
        /// </summary>
        /// <param name="moduleFolder">Folder path reference to create Response module</param>
        /// <param name="name">Name of Response module</param>
        /// <param name="wseModuleParser">Contains transport information used to fill in Response Module</param>
        /// <param name="correlationId">Uniquely identify the Response Module</param>
        /// <returns></returns>
        public static ApiModule CreateResponseModule(TCFolder moduleFolder,
                                                     string name,
                                                     IWseArtifactsParser wseModuleParser,
                                                     string correlationId
        ) {
            //tosca specific properties
            ApiModule responseApiModule =
                    SetModuleStandardProperties(moduleFolder,
                                                name,
                                                correlationId,
                                                ScanTag.GetResponseScanTag(wseModuleParser));
            responseApiModule.AddTechnicalIdParam("Direction", Direction.In.ToString());
            responseApiModule.AddTechnicalIdParam("InactiveNodes", "Keep");
            responseApiModule.AddTechnicalIdParam("IsRequest", "False");
            responseApiModule.AddConfigurationParam("Executor", "HttpReceive");
            responseApiModule.AddHeaders(wseModuleParser.ResponseHeaders);
            responseApiModule.AddTechnicalIdParam("StatusCode", wseModuleParser.ResponseStatus);
            if (!string.IsNullOrEmpty(wseModuleParser.ResponsePayload))
                responseApiModule.APISetMessagePayload(wseModuleParser.ResponsePayload);
            return responseApiModule;
        }

        #endregion

        #region Methods

        private static ApiModule SetModuleStandardProperties(TCFolder moduleFolder,
                                                             string name,
                                                             string correlationId,
                                                             string scanTag) {
            ApiModule requestApiModule = moduleFolder.CreateApiModule();
            requestApiModule.Name = name;
            requestApiModule.EnsureUniqueName();
            requestApiModule.AddTechnicalIdParam("CorrelationId", correlationId);
            requestApiModule.AddTechnicalIdParam("Version", AddOnConstants.ToscaVersion);
            requestApiModule.AddConfigurationParam("Engine", "API");
            requestApiModule.AddTechnicalIdParam("ScanTag", scanTag);
            return requestApiModule;
        }

        #endregion
    }
}