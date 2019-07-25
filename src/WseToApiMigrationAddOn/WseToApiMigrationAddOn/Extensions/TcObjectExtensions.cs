using System;
using System.Collections.Generic;
using System.Linq;

using Tricentis.TCAddOns;
using Tricentis.TCAPIObjects.Objects;

using WseToApiMigrationAddOn.Helper;
using WseToApiMigrationAddOn.Shared;

namespace WseToApiMigrationAddOn.Extensions {
    public static class TcObjectExtensions {
        #region Public Methods and Operators

        public static string GetEscapedNodePath(this TCObject tcObject) {
            return CommonUtilities.GetEscapedNodePath(tcObject.NodePath);
        }

        public static List<XModule> GetWseModules(this TCObject tcObject) {
            try {
                return string.IsNullOrEmpty(tcObject.NodePath)
                               ? tcObject.Search(
                                                 $"=>SUBPARTS:XModule[(Engine=i=\"Webservice\") AND (TestAction=i=\"CommunicateWithWebservice\")]")
                                         .Cast<XModule>().ToList()
                               : TCAddOn.ActiveWorkspace.GetTCProject()
                                        .Search(
                                                $"=>UNION(=>SUBPARTS:TCObject[UniqueId==\"{tcObject.UniqueId}\"]=>SUBPARTS:XModule[(Engine=i=\"Webservice\") AND (TestAction=i=\"CommunicateWithWebservice\")],=>SUBPARTS:TCObject[UniqueId==\"{tcObject.UniqueId}\"]=>UNION( =>SUBPARTS:XTestStep,=>SUBPARTS:TestStepFolderReference=>ReusedItem=>SUBPARTS:XTestStep)->AllReferences:XModule[(Engine=i=\"Webservice\") AND (TestAction=i=\"CommunicateWithWebservice\")])")
                                        .Cast<XModule>().ToList();
            }
            catch (Exception e) {
                FileLogger.Instance.Error("Failed to find WSE Modules", e);
            }

            return new List<XModule>();
        }

        #endregion
    }
}