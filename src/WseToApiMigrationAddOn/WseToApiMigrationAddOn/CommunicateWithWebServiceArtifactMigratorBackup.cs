using System;
using System.Collections.Generic;
using System.Linq;

using Tricentis.TCAPIObjects.Objects;

using WseToApiMigrationAddOn.Helper;
using WseToApiMigrationAddOn.Parser;
using WseToApiMigrationAddOn.Shared;

namespace WseToApiMigrationAddOn.Importer {
    public class CommunicateWithWebServiceArtifactMigratorBack : IMigrator {
        #region Delegates

        delegate ApiModule Search(TCObject rootComponentFolder, WseTestStepParser testStepParser);

        #endregion

        #region Public Methods and Operators

        public void CreateApiModulesAndTestSteps(TCObject rootComponentFolder,
                                                 XModule wseModule,
                                                 ModuleType moduleType) {
            foreach (var teststep in CommonUtilities.GetFilteredWseTestSteps(rootComponentFolder, wseModule.TestSteps)) {
                try {


                    var requestSpecializationModule = teststep.TestStepValues
                                                              .FirstOrDefault(
                                                                      x => x.Name == "Request"
                                                                           && (x.SpecializationModule.Name
                                                                               != "Web service request data in JSON Resource"
                                                                               && x.SpecializationModule.Name
                                                                               != "Web service request data in XML Resource"
                                                                              ))
                                                              ?.SpecializationModule;

                    var responseSpecializationModule = teststep.TestStepValues
                                                               .FirstOrDefault(
                                                                       x => x.Name == "Response"
                                                                            && (x.SpecializationModule.Name
                                                                                != "Web service response data in JSON Resource"
                                                                                && x.SpecializationModule.Name
                                                                                != "Web service response data in XML Resource"
                                                                               ))
                                                               ?.SpecializationModule;

                    if (responseSpecializationModule == null && requestSpecializationModule == null) return;
                    string correlationId = Guid.NewGuid().ToString();
                    var testStepParser = new WseTestStepParser(moduleType);
                    testStepParser.Parse(teststep, requestSpecializationModule, responseSpecializationModule);
                    ApiModule requestApiModule;
                    ApiModule responseApiModule;
                    ApiModule apiModule = null;
                    switch (moduleType) {
                        case ModuleType.CommunicatewithWebserviceRestJson
                                when !string.IsNullOrEmpty(testStepParser.Endpoint):
                        case ModuleType.CommunicatewithWebservice
                                when !string.IsNullOrEmpty(CommonUtilities.GetSoapAction(testStepParser.Headers)):
                            apiModule = SearchApiModuleByScanTag(rootComponentFolder,
                                                                 testStepParser,
                                                                 requestSpecializationModule,
                                                                 responseSpecializationModule,
                                                                 SearchRequestModule);
                            break;
                    }

                    if (apiModule == null) {
                        var apiModuleFolder =
                                new FolderStructureHandler().CreateFolderForWseModules(
                                        rootComponentFolder,
                                        requestSpecializationModule ?? responseSpecializationModule);
                        requestApiModule =
                                ApiModuleCreator.CreateRequestModule(apiModuleFolder,
                                                                     requestSpecializationModule == null
                                                                             ? responseSpecializationModule.Name
                                                                             : requestSpecializationModule.Name,
                                                                     testStepParser,
                                                                     correlationId,
                                                                     ScanTag.GetRequestScanTag(testStepParser));
                        string responseModuleName = requestSpecializationModule == null
                                                            ? responseSpecializationModule.Name
                                                            : requestSpecializationModule.Name;
                        responseApiModule =
                                ApiModuleCreator.CreateResponseModule(apiModuleFolder,
                                                                      $"{responseModuleName} Response",
                                                                      testStepParser,
                                                                      correlationId,
                                                                      ScanTag.GetResponseScanTag(testStepParser));
                    }
                    else {
                        requestApiModule = apiModule;
                        responseApiModule = SearchApiModuleByScanTag(rootComponentFolder,
                                                                     testStepParser,
                                                                     requestSpecializationModule,
                                                                     responseSpecializationModule,
                                                                     SearchResponseModule);
                    }

                    FileLogger.Instance.Debug(
                            $"Completed migration for WSE Module : '{wseModule.Name}' NodePath:'{wseModule.NodePath}'");

                    WseTestStepImporter wseTestStepMigrator = new WseTestStepImporter();
                    wseTestStepMigrator.MigrateTestSteps(rootComponentFolder,
                                                         requestApiModule,
                                                         responseApiModule,
                                                         new List<XTestStep>() { teststep },
                                                         moduleType);
                }
                catch (Exception e) {
                    FileLogger.Instance.Error(e);
                }
            }
        }

        #endregion

        #region Methods

        private static ApiModule SearchApiModuleByScanTag(TCObject rootComponentFolder,
                                                          WseTestStepParser testStepParser,
                                                          XModule requestSpecializationModule,
                                                          XModule responseSpecializationModule,
                                                          Search search) {
            ApiModule apiModule;
            var folderToSearchIn =
                    requestSpecializationModule?.ParentFolder ?? responseSpecializationModule?.ParentFolder;
            if (folderToSearchIn != null) {
                apiModule = search(folderToSearchIn, testStepParser);
                if (apiModule != null) return apiModule;
            }

            apiModule = search(rootComponentFolder, testStepParser);
            return apiModule;
        }

        private static ApiModule SearchRequestModule(TCObject rootComponentFolder, WseTestStepParser testStepParser) {
            return ScanTag.SearchModuleByScanTag(rootComponentFolder, ScanTag.GetRequestScanTag(testStepParser));
        }

        private ApiModule SearchResponseModule(TCObject rootComponentFolder, WseTestStepParser testStepParser) {
            return ScanTag.SearchModuleByScanTag(rootComponentFolder, ScanTag.GetResponseScanTag(testStepParser));
        }

        #endregion
    }
}