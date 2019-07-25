using System;
using System.Collections.Generic;
using System.Linq;

using Tricentis.Automation.WseToApiMigrationAddOn.Helper;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Handler.SpecializationHandlers;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Handler.SpecializationHandlers.Factory;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Parser.Interfaces;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Interfaces;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.RequestSetter;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.ResponseSetter;
using Tricentis.Automation.WseToApiMigrationAddOn.Shared;
using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator {
    /// <summary>
    /// Creates API TestStep Request-Response pair and Migrates WSE TestStep data to API TestStep pair
    /// </summary>
    public class WseTestStepMigrator {
        #region Properties

        /// <summary>
        /// List of setters which are used to set API Request XTestStep data. Each setter reads data from WSE XTestStep and Sets it in API Request XTestStep.
        /// Add a new setter here if you want to set any new property of api request.
        /// </summary>
        private static List<IApiRequestValueSetter> RequestValueSetters { get; } = new List<IApiRequestValueSetter>() {
                new ApiMethodSetter(),
                new ApiEndpointSetter(),
                new ApiResourceSetter(),
                new ApiRequestHeadersSetter(),
                new ApiQueryParamSetter(),
                new ApiRequestBasicAuthSetter()
        };

        /// <summary>
        /// List of setters which are used to set API Response XTestStep data. Each setter reads data from WSE XTestStep and Sets it in API Response XTestStep.
        /// Add a new setter here if you want to set any new property of api response.
        /// </summary>
        private static List<IApiResponseValueSetter> ResponseValueSetters { get; } = new List<IApiResponseValueSetter>() {
                new ApiStatusCodeSetter(),
                new ApiResponseHeadersSetter()
        };

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Migrates WSE XTestStep to API XTestStep.(Also Applies any transformation which are performed on Request or Response.Currently a limited list
        /// of transformation is supported.)
        /// </summary>
        /// <param name="rootComponentFolder">Folder on which the migration is performed</param>
        /// <param name="requestApiModule">Request API Module</param>
        /// <param name="responseApiModule">Response API Module</param>
        /// <param name="payloadParser">Instance of XML or JSON parser depending upon Module Type</param>
        /// <param name="payloadSetterFactory">Instance of XML or JSON Setter depending upon Module Type</param>
        /// <param name="wseTestStep">WSE XTestStep to be migrated</param>
        public void Migrate(TCObject rootComponentFolder,
                            ApiModule requestApiModule,
                            ApiModule responseApiModule,
                            IPayloadParser payloadParser,
                            IPayloadSetterFactory payloadSetterFactory,
                            XTestStep wseTestStep) {
            try {
                FileLogger.Instance.Info(
                        $"Migrating XTestStep:{wseTestStep.Name} -- NodePath:{wseTestStep.NodePath}");
                bool isWseStepDisabled = wseTestStep.Disabled;
                //disable wse teststep
                wseTestStep.Disabled = true;

                //Get Parent of wse teststep
                var parent = wseTestStep.ParentFolder ?? (dynamic)wseTestStep.TestCase;

                //Create API Request TestStep
                XTestStep requestTestStep = (XTestStep)parent.CreateXTestStepFromXModule(requestApiModule);
                requestTestStep.Name = wseTestStep.Name;

                AbstractSpecializationHandler requestSpecializationHandler =
                        SpecializationFactory.GetRequestSpecializationHandler();
                requestSpecializationHandler.HandleSpecialization(wseTestStep,
                                                                  requestTestStep,
                                                                  payloadParser,
                                                                  payloadSetterFactory);

                RequestValueSetters.ForEach(setter => setter.Execute(requestTestStep, wseTestStep));
                wseTestStep.Reorder(requestTestStep, "Yes");

                //Create API Response TestStep
                XTestStep responseTestStep = (XTestStep)parent.CreateXTestStepFromXModule(responseApiModule);
                responseTestStep.Name = wseTestStep.Name + " Response";

                ResponseValueSetters.ForEach(setter => setter.Execute(responseTestStep, wseTestStep));
                requestTestStep.Reorder(responseTestStep, "Yes");

                AbstractSpecializationHandler responseSpecializationHandler =
                        SpecializationFactory.GetResponseSpecializationHandler();
                responseSpecializationHandler.HandleSpecialization(wseTestStep,
                                                                   responseTestStep,
                                                                   payloadParser,
                                                                   payloadSetterFactory);

                //For Payload
                payloadSetterFactory.GetRequestPayloadSetter().Execute(requestTestStep, wseTestStep);
                payloadSetterFactory.GetResponsePayloadSetter().Execute(responseTestStep, wseTestStep);

                CommonParserMethods.FillTransformRequest(requestTestStep, wseTestStep);
                CommonParserMethods.FillTransformResponse(responseTestStep, wseTestStep);

                if (isWseStepDisabled) {
                    requestTestStep.Disable(wseTestStep.DisabledDescription);
                    responseTestStep.Disable(wseTestStep.DisabledDescription);
                    requestTestStep.Disabled = true;
                    responseTestStep.Disabled = true;
                }

                if (!string.IsNullOrEmpty(wseTestStep.Condition)) {
                    requestTestStep.Condition = wseTestStep.Condition;
                    responseTestStep.Condition = wseTestStep.Condition;
                }

                if (!string.IsNullOrEmpty(wseTestStep.Path)) {
                    requestTestStep.Path = wseTestStep.Path;
                    responseTestStep.Path = wseTestStep.Path;
                }

                MoveBodyAttributeAtLast(requestTestStep);
                MoveBodyAttributeAtLast(responseTestStep);
            }
            catch (Exception e) {
                FileLogger.Instance.Error(e);
            }
        }

        #endregion

        #region Methods

        private void MoveBodyAttributeAtLast(XTestStep xTestStep) {
            var bodyAttribute = xTestStep.Module.Attributes.FirstOrDefault(x => x.Name == "Body");
            if (bodyAttribute != null) {
                xTestStep.Module.Move(bodyAttribute);
            }
        }

        #endregion
    }
}