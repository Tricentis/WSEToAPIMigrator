﻿using WseToApiMigrationAddOn.Migrator.Setter.Interfaces;
using WseToApiMigrationAddOn.Migrator.Setter.Templates;

namespace WseToApiMigrationAddOn.Migrator.Setter.ResponseSetter {
    /// <summary>
    /// Extracts wsetestStepvalues using TQL to process XML payload
    /// </summary>
    public class ApiResponseXmlPayloadSetter : ApiXmlPayloadSetterTemplate, IApiResponseValueSetter {
        #region Properties

        /// <summary>
        /// TQL returns wsetesstep values from response module of wse.
        /// </summary>
        protected override string TqlToGetWseTestStepValue =>
                "=>SUBPARTS:XTestStepValue[Name==\"Response\"]->SUBPARTS";

        #endregion
    }
}