using System;
using System.Linq;

using Tricentis.Automation.WseToApiMigrationAddOn.Extensions;
using Tricentis.Automation.WseToApiMigrationAddOn.Shared;
using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Parser {
    /// <summary>
    /// Extracts HTTP Method from WSE artifact
    /// </summary>
    public class MethodParser {
        #region Public Methods and Operators

        /// <summary>
        /// Extracts method from WSE Module
        /// </summary>
        /// <param name="xModule">WSE Module</param>
        /// <returns>HTTP Method</returns>
        public string Parse(XModule xModule) {
            try {
                XModuleAttribute method = (XModuleAttribute)
                        xModule.Search(AddOnConstants.MethodTql).FirstOrDefault();
                if (method == null) return string.Empty;
                return method.DefaultValue;
            }
            catch (Exception ex) {
                FileLogger.Instance.Error(ex);
            }

            return string.Empty;
        }

        /// <summary>
        /// Extracts method from WSE TestStep
        /// </summary>
        /// <param name="xTestStep">WSE TestStep</param>
        /// <returns>HTTP Method</returns>
        public string Parse(XTestStep xTestStep) {
            try {
                XTestStepValue method =
                        (XTestStepValue)xTestStep.Search(AddOnConstants.TestStepMethodTql).FirstOrDefault();
                if (method == null) return string.Empty;
                return method.Value;
            }
            catch (Exception ex) {
                FileLogger.Instance.Error(ex);
            }

            return string.Empty;
        }

        #endregion
    }
}