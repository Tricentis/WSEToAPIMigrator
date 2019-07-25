using System.ComponentModel;

namespace WseToApiMigrationAddOn.Helper {
    public enum ModuleType {
        [Description("Null")]
        Null,

        [Description("Communicate with Web service")]
        CommunicatewithWebservice,

        [Description("Communicate with Web service (REST/JSON)")]
        CommunicatewithWebserviceRestJson,
    }

    public enum BusinessParameterPathTypes {
        XPATH,

        XQUERY,

        JSONPATH,

        ENDPOINT,

        HEADER,

        RESOURCE,

        URLPARAM
    }
}