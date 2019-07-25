using System;
using System.Text.RegularExpressions;

using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Interfaces;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Templates;
using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.RequestSetter {
    /// <summary>
    /// Matches regex pattern for json path and retrns json path
    /// </summary>
    public class ApiRequestJsonPayloadSetter : ApiJsonPayloadSetterTemplate, IApiRequestValueSetter {
        #region Properties

        protected override string TqlToGetAllWseTestStepValue =>
                "=>SUBPARTS:XTestStepValue[Name==\"Request\"]->SUBPARTS->SUBPARTS";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Evaluates index of Json Path
        /// </summary>
        /// <param name="match">regex pattern match</param>
        /// <returns></returns>
        public override string Evaluator(Match match) {
            var v = Convert.ToInt32(match.Groups[1].Value) - 1;
            return "[" + v + "]";
        }

        public override string GetJsonPath(XTestStep wseTestStep, XTestStepValue wseTestStepValue, string jsonPath) {
            return jsonPath;
        }

        #endregion
    }
}