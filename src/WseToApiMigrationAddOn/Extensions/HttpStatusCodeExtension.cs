using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Web;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Extensions {
    /// <summary>
    /// HTTP Status Code Helper Methods
    /// </summary>
    public static class HttpStatusCodeExtension {
        #region Public Methods and Operators

        /// <summary>
        /// Gets Status Code with Description.
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public static string GetString(int statusCode) {
            return GetString(statusCode, Description(statusCode));
        }

        /// <summary>
        /// Get all possible values of Status codes.
        /// </summary>
        /// <returns></returns>
        public static string ValueRange() {
            var statuscodes = EnumHelper.Values<HttpStatusCode>().ToList();
            statuscodes.Remove(HttpStatusCode.Unused);
            statuscodes.Remove(HttpStatusCode.UpgradeRequired);
            return string.Join(";", statuscodes.Select(GetString).Distinct().ToArray());
        }

        #endregion

        #region Methods

        private static string Description(int statusCode) {
            return HttpWorkerRequest.GetStatusDescription(statusCode);
        }

        private static string GetString(this HttpStatusCode statusCode) {
            return GetString((int)statusCode);
        }

        private static string GetString(int statusCode, string description) {
            return $"{statusCode} {description}";
        }

        #endregion

        public static class EnumHelper {
            #region Public Methods and Operators

            public static string EnumDescription<T>(T val) {
                return val.GetType().GetField(val.ToString())
                          ?.GetCustomAttributes(typeof(DescriptionAttribute), false) is
                               DescriptionAttribute[] list && list.Length > 0
                               ? list[0].Description
                               : val.ToString();
            }

            public static string Name<T>(T val) {
                return Enum.GetName(typeof(T), val);
            }

            public static IEnumerable<T> Values<T>() {
                return Enum.GetValues(typeof(T)).Cast<T>();
            }

            #endregion
        }
    }
}