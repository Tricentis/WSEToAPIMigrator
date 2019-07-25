using System.Collections.Generic;
using System.Linq;

using WseToApiMigrationAddOn.Extensions.Tricentis.Automation.Api.Core.Extensions;

namespace WseToApiMigrationAddOn.Extensions {
    /// <summary>
    /// All HTTP Methods Supported by Tosca
    /// </summary>
    public enum HttpMethod {
        POST = 0,

        GET = 1,

        HEAD = 2,

        PUT = 3,

        DELETE = 4,

        TRACE = 5,

        OPTIONS = 6,

        PATCH = 7,

        COPY = 8,

        LINK = 9,

        UNLINK = 10,

        PURGE = 11,

        LOCK = 12,

        UNLOCK = 13,

        PROPFIND = 14,

        VIEW = 15
    }

    public static class HttpMethodExtension {
        #region Public Methods and Operators

        public static bool TryGetEnum(string type, out HttpMethod result) {
            var r = HttpStatusCodeExtension.EnumHelper.Values<HttpMethod>()
                                           .Where(e => type.EqualsIgnoreCase(e.ToString())).ToArray();
            if (r.Any()) {
                result = r.First();
                return true;
            }

            result = default(HttpMethod);
            return false;
        }

        public static List<string> ValueRange() {
            return HttpStatusCodeExtension.EnumHelper.Values<HttpMethod>().Select(x => x.ToString()).ToList();
        }

        public static string ValueRangeString() {
            return string.Join(";", ValueRange());
        }

        #endregion
    }
}