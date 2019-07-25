using System.Collections.Generic;
using System.Linq;

using Tricentis.Automation.Resources;

namespace WseToApiMigrationAddOn.Migrator.Handler.SpecializationHandlers.Model {
    public class TestCaseResourceResult : IResource {
        #region Public Properties

        public List<TestCaseResource> TestCaseResources { get; set; }

        #endregion

        #region Public Methods and Operators

        public void AddResult(string testCaseName, string resourceName) {
            if (TestCaseResources != null && TestCaseResources.Any()) {
                foreach (var testCaseRes in TestCaseResources) {
                    if (testCaseRes.TestCaseName == testCaseName) {
                        testCaseRes.Add(resourceName);
                        return;
                    }
                }

                TestCaseResource testCaseResource = FillTestCaseResource(testCaseName, resourceName);
                TestCaseResources.Add(testCaseResource);
            }
            else {
                TestCaseResource testCaseResource = FillTestCaseResource(testCaseName, resourceName);
                TestCaseResources = new List<TestCaseResource> {
                        testCaseResource
                };
            }
        }

        public void Dispose() {
            //ignored
        }

        #endregion

        #region Methods

        private TestCaseResource FillTestCaseResource(string testCaseName, string resourceName) {
            TestCaseResource testCaseResource = new TestCaseResource {
                    TestCaseName = testCaseName
            };
            testCaseResource.Add(resourceName);
            return testCaseResource;
        }

        #endregion
    }
}