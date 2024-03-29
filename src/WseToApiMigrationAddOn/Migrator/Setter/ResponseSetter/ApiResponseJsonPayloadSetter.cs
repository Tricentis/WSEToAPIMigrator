﻿using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Interfaces;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Templates;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.ResponseSetter {
    /// <summary>
    /// Extracts wsetestStepvalues using TQL to process json payload
    /// </summary>
    public class ApiResponseJsonPayloadSetter : ApiJsonPayloadSetterTemplate, IApiResponseValueSetter {
        #region Properties

        /// <summary>
        /// TQL returns wsetesstep values from response module of wse.
        /// </summary>
        protected override string TqlToGetAllWseTestStepValue =>
                "=>SUBPARTS:XTestStepValue[Name==\"Response\"]->SUBPARTS->SUBPARTS";

        #endregion
    }
}