using System;
using System.Collections.Generic;

using Tricentis.Automation.WseToApiMigrationAddOn.Helper;
using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Parser {
    /// <summary>
    /// Gets HTTP Headers from WSE Artifacts 
    /// </summary>
    public class HeaderParser {
        #region Public Methods and Operators

        /// <summary>
        /// Gets headers from WSE Module
        /// </summary>
        /// <param name="wseModule">WSE Module</param>
        /// <param name="tql">tql to get Header XModuleAttribute from WSE Module</param>
        /// <returns>All headers as a dictionary</returns>
        public Dictionary<string, string> Parse(XModule wseModule, string tql) {
            var headersDict = new Dictionary<string, string>();
            try {
                List<TCObject> headers =
                        wseModule.Search(tql);
                foreach (TCObject header in headers) {
                    XModuleAttribute headerAttribute = (XModuleAttribute)header;
                    if (string.IsNullOrEmpty(headerAttribute.DefaultValue)) continue;
                    headersDict.Add(headerAttribute.Name,
                                    CommonUtilities.RemoveExtraDoubleQuotes(headerAttribute.DefaultValue));
                }
            }
            catch (Exception) {
                // do nothing as this could happen possibly, just move on with the other attributes
            }

            return headersDict;
        }

        /// <summary>
        /// Gets headers from WSE XTestStep
        /// </summary>
        /// <param name="xTestStep">WSE XTestStep</param>
        /// <param name="tql">tql to get Header XTestStepValue from WSE XTestStep</param>
        /// <returns>All headers as a dictionary</returns>
        public Dictionary<string, string> Parse(XTestStep xTestStep, string tql) {
            var headersDict = new Dictionary<string, string>();
            try {
                List<TCObject> headers =
                        xTestStep.Search(tql);
                foreach (TCObject header in headers) {
                    XTestStepValue headerAttribute = (XTestStepValue)header;
                    if (string.IsNullOrEmpty(headerAttribute.Value)) continue;
                    headersDict.Add(headerAttribute.Name,
                                    CommonUtilities.RemoveExtraDoubleQuotes(headerAttribute.Value));
                }
            }
            catch (Exception) {
                // do nothing as this could happen possibly, just move on with the other attributes
            }

            return headersDict;
        }

        #endregion
    }
}