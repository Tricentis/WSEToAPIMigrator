using System.Linq;
using System.Text;

using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Parser;
using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Helper {
    /// <summary>
    /// Create ScanTag for uniquely identify a API Request/Response Module 
    /// </summary>
    public class ScanTag {
        #region Public Methods and Operators

        public static string GetRequestScanTag(IWseArtifactsParser parser) {
            return Generate(parser, "Request");
        }

        public static string GetResponseScanTag(IWseArtifactsParser parser) {
            return Generate(parser, "Response");
        }

        public static ApiModule SearchModuleByScanTag(TCObject rootComponentFolder, string scanTag) {
            var apiModule = (ApiModule)rootComponentFolder
                                       .Search(
                                               $"=>SUBPARTS:ApiModule[ScanTag==\"{scanTag}\"]")
                                       .FirstOrDefault();
            return apiModule;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create ScanTag for uniquely identify a API Request/Response Module from WSE artifacts 
        /// </summary>
        /// <param name="parserResult">Transport information used to create ScanTag</param>
        /// <param name="prefix">Dilimiter</param>
        /// <returns></returns>
        private static string Generate(IWseArtifactsParser parserResult, string prefix) {
            var scanTag = new StringBuilder();
            scanTag.Append(prefix);
            scanTag.Append("#");
            scanTag.Append(parserResult.Method);
            scanTag.Append("#");
            scanTag.Append(parserResult.Endpoint);
            scanTag.Append("#");
            scanTag.Append(parserResult.Resource);
            scanTag.Append("#");
            scanTag.Append(CommonUtilities.GetSoapAction(parserResult.Headers));
            scanTag.Append("#");
            scanTag.Append(CommonUtilities.GetQueryParamsKeys(parserResult.QueryParams));
            scanTag.Append("#");
            scanTag.Append(parserResult.HashCode);
            return scanTag.ToString();
        }

        #endregion
    }
}