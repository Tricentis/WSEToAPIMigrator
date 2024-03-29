﻿using System.Linq;

using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Interfaces;
using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Factory {
    public static class MigratorFactory {
        #region Public Methods and Operators

        /// <summary>
        /// Return appropriate migrator instance depending upon type of WSE Module
        /// </summary>
        public static IMigrator GetMigrator(XModule wseModule) {
            if (wseModule.Search("=>SELF[ScanTag==\"Communicate with Web service (REST/JSON)\"]").Any()) {
                return new CommunicateWithWebServiceJsonArtifactMigrator(wseModule);
            }

            if (wseModule.Search("=>SELF[ScanTag==\"Communicate with Web service\"]").Any()) {
                return new CommunicateWithWebServiceXmlArtifactMigrator(wseModule);
            }

            return new ScannedWebserviceArtifactMigrator(wseModule);
        }

        #endregion
    }
}