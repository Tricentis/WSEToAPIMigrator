using Tricentis.Automation.Resources;

using WseToApiMigrationAddOn.Migrator.Handler.SpecializationHandlers.Model;

namespace WseToApiMigrationAddOn.Migrator.Handler {
    /// <summary>
    /// Add and Remove resources in ResourceManager for a ResourceId
    /// </summary>
    public static class ResourceManagerHandler {
        #region Public Properties

        public static string ResourceId { get; set; } = "WseToApiMigration_TestCaseResourceResult";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Add resources in ResourceManager for a ResourceId
        /// </summary>
        /// <param name="testCaseName">Name of TestCase</param>
        /// <param name="resourceName">Name of a Resource needs to be added for above TestCase</param>
        public static void AddResourceToResourceId(string testCaseName, string resourceName) {
            TestCaseResourceResult testCaseResourceResult = GetResourceFromResourceId(ResourceId).Item2;
            if (testCaseResourceResult != null) {
                testCaseResourceResult.AddResult(testCaseName, resourceName);
            }
            else {
                testCaseResourceResult = new TestCaseResourceResult();
                testCaseResourceResult.AddResult(testCaseName, resourceName);
            }

            ResourceManager.Instance.AddOrReplace(
                    ResourceId,
                    testCaseResourceResult);
        }

        /// <summary>
        /// Remove ResourceId from ResourceManager
        /// </summary>
        /// <param name="v">ResourceId</param>
        /// <returns>true if ResourceId is deleted from Resource Manager,resource object from resource manager for that ResourceID</returns>
        public static (bool, TestCaseResourceResult) DeleteResource(string v) {
            var boolGet =
                    ResourceManager.Instance.TryRemove<TestCaseResourceResult>(
                            v,
                            out TestCaseResourceResult testCaseResourceResult);
            return (boolGet, testCaseResourceResult);
        }

        /// <summary>
        /// Get resource object from resource manager for that ResourceID
        /// </summary>
        /// <param name="v">ResourceId</param>
        /// <returns>true if ResourceId is exists in Resource Manager,resource object in resource manager for that ResourceID</returns>
        public static (bool, TestCaseResourceResult) GetResourceFromResourceId(string v) {
            var boolGet =
                    ResourceManager.Instance.TryGet<TestCaseResourceResult>(
                            v,
                            out TestCaseResourceResult testCaseResourceResult);
            return (boolGet, testCaseResourceResult);
        }

        #endregion
    }
}