using System;
using System.Linq;

using Tricentis.TCAPIObjects.Objects;

using WseToApiMigrationAddOn.Extensions.Tricentis.Automation.Api.Core.Extensions;
using WseToApiMigrationAddOn.Migrator.Parser.Interfaces;
using WseToApiMigrationAddOn.Migrator.Setter.Interfaces;
using WseToApiMigrationAddOn.Shared;

namespace WseToApiMigrationAddOn.Migrator.Handler.SpecializationHandlers {
    /// <summary>
    /// Handles Web service response data in Xml/JSON Resource specialization 
    /// </summary>
    public class ResponseAsResourceSpecializationHandler : AbstractResourceSpecializationHandler {
        #region Properties

        protected override string TqlToGetWseTestStepSpecializationModule =>
                "=>SUBPARTS:XTestStepValue->SpecializationModule[Name==\"Web service response data in XML Resource\" OR Name==\"Web service response data in JSON Resource\"]";

        protected override string TqlToGetWseTestStepValue => "=>SUBPARTS:XTestStepValue[Name==\"Response\"]";

        private string TqlToSearchTestCase => "=>SUPERPART:TestCase";

        #endregion

        #region Public Methods and Operators
        /// <summary>
        /// Handles response data in Xml/JSON Resource specialization
        /// </summary>
        /// <param name="wseTestStep">TestStep of WSE Artifacts</param>
        /// <param name="apiTestStep">TestStep of API Engine Artifacts</param>
        /// <param name="payloadParser">Parse xml and json payload from Wse XModules and TestSteps</param>
        /// <param name="payloadSetterFactory">Create module attributes and Set values for payload in for Api artifacts </param> 
        public override void HandleSpecialization(XTestStep wseTestStep,
                                                  XTestStep apiTestStep,
                                                  IPayloadParser payloadParser,
                                                  IPayloadSetterFactory payloadSetterFactory) {
            XModule specializationModule =
                    (XModule)wseTestStep.Search(TqlToGetWseTestStepSpecializationModule)?.FirstOrDefault();

            if (specializationModule != null) {
                try {
                    var wseTestStepValue =
                            (XTestStepValue)wseTestStep.Search(TqlToGetWseTestStepValue)?.FirstOrDefault();
                    if (wseTestStepValue != null && !StringExtensions.IsNullOrBlankInTosca(wseTestStepValue.Value)) {
                        TestCase testCase = (TestCase)wseTestStep.Search(TqlToSearchTestCase)?.FirstOrDefault();

                        if (testCase != null)
                            ResourceManagerHandler.AddResourceToResourceId(testCase.Name, wseTestStepValue.Value);
                    }
                }
                catch (Exception ex) {
                    FileLogger.Instance.Error("Unable to fetch WSE tesstep value ", ex);
                }
            }
            else {
                successor?.HandleSpecialization(wseTestStep, apiTestStep, payloadParser, payloadSetterFactory);
            }
        }

        #endregion
    }
}