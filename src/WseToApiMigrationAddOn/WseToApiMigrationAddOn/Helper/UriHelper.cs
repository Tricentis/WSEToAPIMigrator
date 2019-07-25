using System;
using System.Collections.Generic;
using System.Linq;

namespace WseToApiMigrationAddOn.Helper {
    /// <summary>
    /// Get Endpoint and Resource from uri
    /// </summary>
    public static class UriHelper {
        #region Public Methods and Operators
        /// <summary>
        /// Split QueryParams from uri
        /// </summary>
        /// <param name="uri">Address of wse artifact</param>
        /// <returns>Query params in key-value pair</returns>
        public static Dictionary<string, string> DecodeQueryParameters(string uri) {
            if (uri == null)
                return new Dictionary<string, string>();
            try {
                if (uri.Contains("?")) {
                    var query = uri.Split(new[] { '?' }, 2, StringSplitOptions.RemoveEmptyEntries)[1];
                    if (query != null) return GetQueryParams(query);
                }
            }
            catch (Exception) {
                // ignored
            }

            return new Dictionary<string, string>();
        }
        /// <summary>
        /// Split Endpoint from uri
        /// </summary>
        /// <param name="uri">Address of wse artifact</param>
        /// <returns>Get Endpoint which need to be set in Endpoint section of ApiModules</returns>
        public static string GetEndPoint(string uri) {
            if (string.IsNullOrEmpty(uri)) return string.Empty;
            try {
                var resource = GetResourceWithQuery(uri);
                if (!String.IsNullOrEmpty(resource)) {
                    return uri.Replace(resource, "");
                }
                else {
                    if (uri.Contains("?"))
                        uri = uri.Split(new[] { '?' }, 2, StringSplitOptions.RemoveEmptyEntries)[0];
                }

                return uri;
            }
            catch (Exception) {
                return uri;
            }
        }
        /// <summary>
        /// Split Resource from uri
        /// </summary>
        /// <param name="uri">Address of wse artifact</param>
        /// <returns>Get Resource which need to be set in Resource section of ApiModules</returns>
        public static string GetResource(string uri) {
            try {
                if (string.IsNullOrEmpty(uri) || !uri.Contains("/"))
                    return string.Empty;

                if (uri.Contains("?"))
                    uri = uri.Split(new[] { '?' }, 2, StringSplitOptions.RemoveEmptyEntries)[0];

                return GetResourceWithQuery(uri);
            }
            catch (Exception) {
                return string.Empty;
            }
        }

        #endregion

        #region Methods

        private static Dictionary<string, string> GetQueryParams(string uri) {
            return uri.TrimStart('?')
                      .Split(new[] { '&', ';' }, StringSplitOptions.RemoveEmptyEntries)
                      .Select(parameter => parameter.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries))
                      .GroupBy(parts => parts[0],
                               parts => parts.Length > 2
                                                ? string.Join("=", parts, 1, parts.Length - 1)
                                                : (parts.Length > 1 ? parts[1] : ""))
                      .ToDictionary(grouping => grouping.Key,
                                    grouping => string.Join(",", grouping));
        }

        private static string GetResourceWithQuery(string uri) {
            try {
                if (string.IsNullOrEmpty(uri) || !uri.Contains("/"))
                    return string.Empty;
                if (uri.Contains("http://"))
                    uri = uri.Replace("http://", "");
                if (uri.Contains("https://"))
                    uri = uri.Replace("https://", "");
                string resource = uri.Split(new[] { '/' }, 2, StringSplitOptions.RemoveEmptyEntries)[1];
                return string.IsNullOrEmpty(resource) ? resource : string.Concat("/", resource);
            }
            catch (Exception) {
                return string.Empty;
            }
        }

        #endregion
    }
}