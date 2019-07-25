using System.Collections.Generic;
using System.Linq;

namespace WseToApiMigrationAddOn.Migrator.Handler.SpecializationHandlers.Model {
    public class TestCaseResource {
        #region Public Properties

        public List<string> ResourceNameList { get; set; }

        public string TestCaseName { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Add(string resourceName) {
            if (ResourceNameList != null && ResourceNameList.Any() && !ResourceNameList.Contains(resourceName)) {
                ResourceNameList.Add(resourceName);
            }
            else {
                ResourceNameList = new List<string> {
                        resourceName
                };
            }
        }

        #endregion
    }
}