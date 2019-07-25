using System;
using System.Linq;

using Tricentis.Automation.WseToApiMigrationAddOn.Extensions;
using Tricentis.Automation.WseToApiMigrationAddOn.Shared;
using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Parser {
    /// <summary>
    /// Gets Response Status Code from  WSE artifacts.
    /// </summary>
    public class StatusCodeParser {
        #region Public Methods and Operators

        /// <summary>
        /// Gets Response Status Code from WSE Module
        /// </summary>
        /// <param name="xModule">WSE Module</param>
        /// <returns>Status code</returns>
        public string ParseResponseStatus(XModule xModule) {
            try {
                TCObject statusCodeModuleAttribute =
                        xModule.Search(AddOnConstants.ResponseStatusTql)
                               .FirstOrDefault();
                if (statusCodeModuleAttribute == null) return string.Empty;

                XModuleAttribute statusCodeXModuleAttribute = statusCodeModuleAttribute as XModuleAttribute;
                return statusCodeXModuleAttribute?.DefaultValue;
            }
            catch (Exception ex) {
                FileLogger.Instance.Error("Failed to retrieve response Status Code :", ex);
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets Response Status Code from WSE TestStep
        /// </summary>
        /// <param name="xTestStep">WSE TestStep</param>
        /// <returns>Status code</returns>
        public string ParseResponseStatus(XTestStep xTestStep) {
            try {
                TCObject statusCodeModuleAttribute =
                        xTestStep.Search(AddOnConstants.TestStepResponseStatusTql)
                                 .FirstOrDefault();
                if (statusCodeModuleAttribute == null) return string.Empty;

                XTestStepValue statusCodeXModuleAttribute = statusCodeModuleAttribute as XTestStepValue;
                return statusCodeXModuleAttribute?.Value;
            }
            catch (Exception ex) {
                FileLogger.Instance.Error("Failed to retrieve response Status Code :", ex);
            }

            return string.Empty;
        }

        #endregion
    }
}