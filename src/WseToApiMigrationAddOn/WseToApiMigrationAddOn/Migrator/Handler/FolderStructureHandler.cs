using System.Linq;

using Tricentis.TCAPIObjects.Objects;

using WseToApiMigrationAddOn.Helper;
using WseToApiMigrationAddOn.Migrator.Parser;

namespace WseToApiMigrationAddOn.Migrator.Handler {
    /// <summary>
    ///  Identify location of existing WSE Modules and create generic folder for storing API modules
    /// </summary>
    public class FolderStructureHandler {
        #region Public Methods and Operators

        /// <summary>
        /// Identify location of existing WSE Modules and create generic folder for storing API modules
        /// </summary>
        /// <param name="wseModule">Xmodule of WSE Engine</param>
        /// <returns></returns>
        public TCFolder CreateFolderForApiModules(XModule wseModule) {
            TCFolder parentFolder = wseModule.ParentFolder as TCFolder;
            TCFolder apiFolder = parentFolder?.CreateFolder();
            if (apiFolder != null) {
                apiFolder.Name = $"API_{wseModule.Name}";
                apiFolder.EnsureUniqueName();
            }

            return apiFolder;
        }

        /// <summary>
        /// Identify location of existing WSE Specialized Modules and create generic folder for storing API modules
        /// </summary>
        /// <param name="rootComponentFolder"> Component Folder at which migration is executing </param>
        /// <param name="xTestStep">TestStep for WseArtifacts</param>
        /// <param name="parserResult">Transport information used to fill in Request and Response Module</param>
        /// <returns></returns>
        public (TCFolder, string) CreateFolderForApiModules(TCObject rootComponentFolder,
                                                            XTestStep xTestStep,
                                                            IWseArtifactsParser parserResult) {
            if (SpecializationHelper.IsRequestIsUsingEmbeddedModule(xTestStep, out XModule requestSpecializationModule)
            ) {
                return (CreateFolderForApiModules(requestSpecializationModule), requestSpecializationModule.Name);
            }

            if (SpecializationHelper.IsResponseIsUsingEmbeddedModule(xTestStep,
                                                                     out XModule responseSpecializationModule)) {
                return (CreateFolderForApiModules(responseSpecializationModule), responseSpecializationModule.Name);
            }

            var apiModulesFolder = GetOrCreateApiModulesFolder(rootComponentFolder);
            var moduleName = CommonUtilities.CreateModuleFolderName(xTestStep, parserResult);
            var folder = apiModulesFolder.CreateFolder();
            folder.Name = $"API_{moduleName}";
            folder.EnsureUniqueName();
            return (folder, moduleName);
        }

        #endregion

        #region Methods

        private static TCFolder GetOrCreateApiModulesFolder(TCObject rootComponentFolder) {
            TCFolder apiModulesFolder = null;
            switch (rootComponentFolder) {
                case TCComponentFolder folder:
                    apiModulesFolder =
                            (TCFolder)folder.Search("->SUBPARTS:TCFolder[Name == \"API Modules\"]").FirstOrDefault();
                    if (apiModulesFolder != null) return apiModulesFolder;
                    apiModulesFolder = folder.CreateModulesFolder();
                    apiModulesFolder.Name = "API Modules";
                    break;
                case TCProject tcProject:
                    var modulesFolder =
                            tcProject.Search("->SUBPARTS:TCFolder[NodePath==\"/Modules\"]")
                                     .Cast<TCFolder>().FirstOrDefault();
                    apiModulesFolder = (TCFolder)modulesFolder
                                                 ?.Search("->SUBPARTS:TCFolder[Name == \"API Modules\"]")
                                                 .FirstOrDefault();
                    if (apiModulesFolder != null) return apiModulesFolder;
                    if (modulesFolder != null) {
                        apiModulesFolder = modulesFolder.CreateFolder();
                        apiModulesFolder.Name = "API Modules";
                    }

                    break;
            }

            return apiModulesFolder;
        }

        #endregion
    }
}