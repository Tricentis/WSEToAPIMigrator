using System;
using System.Text.RegularExpressions;

namespace WseToApiMigrationAddOn.Extensions {
    namespace Tricentis.Automation.Api.Core.Extensions {
        public static class StringExtensions {
            #region Public Methods and Operators

            public static bool EqualsIgnoreCase(this string a, string b) {
                return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
            }

            public static string EscapeQuotes(this string value) {
                if (value == null) {
                    return null;
                }

                if (!value.Contains("\"")) {
                    return value;
                }

                var startIndex = value.IndexOf('"');
                var endIndex = value.LastIndexOf('"');
                var start = value.Substring(0, value.IndexOf('"'));
                var end = value.Substring(endIndex + 1, value.Length - endIndex - 1);
                return start + "\"" + value.Substring(startIndex, endIndex - startIndex + 1).Replace("\"", "\"\"")
                       + "\"" + end;
            }

            public static string ExtractStringFromResource(this string str) {
                Regex regex = new Regex(@"RES\[(.*)\]");
                while (regex.IsMatch(str)) {
                    str = regex.Match(str).Groups[1].Value;
                }

                return str;
            }

            public static bool IsNullOrBlankInTosca(this string value) {
                if (value == string.Empty) {
                    return true;
                }

                if (string.IsNullOrWhiteSpace(value)) {
                    return true;
                }

                if (value.Contains("{NULL}"))
                    return true;

                return false;
            }

            #endregion
        }
    }
}