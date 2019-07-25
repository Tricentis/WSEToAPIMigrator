using System;

using Tricentis.TCAddOns;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Extensions {
    /// <summary>
    /// All constants used through-out the project
    /// </summary>
    public static class AddOnConstants {
        #region Static Fields

        public static readonly string AddressValueJsonTql = "=>SUBPARTS: XTestStepValue[Name==\"Address\"]";

        public static readonly string AddressValueTql =
                "=>SUBPARTS:XModuleAttribute[Name==\"Communicate\"]=>SubAttributes:XModuleAttribute[Name==\"Address\"]";

        public static readonly string HeadersJsonTql =
                "=>SUBPARTS: XTestStepValue[Name==\"Send\"]=>SUBPARTS: XTestStepValue[Name==\"Headers\"]=>SUBPARTS";

        public static readonly string MethodTql =
                "=>SUBPARTS:XModuleAttribute[Name==\"Communicate\"]=>SubAttributes:XModuleAttribute[Name==\"Send\"]=>SubAttributes[Name==\"Method\"]";

        public static readonly string RequestHeadersTql =
                "=>SUBPARTS:XModuleAttribute[Name==\"Communicate\"]=>SubAttributes:XModuleAttribute[Name==\"Send\"]=>SubAttributes[Name==\"Headers\"]=>SubAttributes";

        public static readonly string RequestMethodJsonTql =
                "=>SUBPARTS:XTestStepValue[Name==\"Communicate\"]=>SUBPARTS:XTestStepValue[Name==\"Send\"]=>SUBPARTS:XTestStepValue[Name==\"Method\"]";

        public static readonly string RequestPayloadTql =
                "=>SUBPARTS:XModuleAttribute[Name==\"Request\"]=>SubAttributes:XModuleAttribute[BusinessType==\"XmlElement\"]";

        public static readonly string ResponseHeadersJsonTql =
                "=>SUBPARTS:XTestStepValue[Name==\"Receive\"]=>SUBPARTS:XTestStepValue[Name==\"Headers\"]=>SUBPARTS";

        public static readonly string ResponseHeadersTql =
                "=>SUBPARTS:XModuleAttribute[Name ==\"Communicate\"]=>SubAttributes:XModuleAttribute[Name==\"Receive\"]=>SubAttributes[Name==\"Headers\"]=>SubAttributes";

        public static readonly string ResponsePayloadTql =
                "=>SUBPARTS:XModuleAttribute[Name==\"Response\"]=>SubAttributes:XModuleAttribute[BusinessType==\"XmlElement\"]";

        public static readonly string ResponseStatusJsonTql =
                "=>SUBPARTS:XTestStepValue[Name==\"Receive\"]=>SUBPARTS:XTestStepValue[Name==\"Status code name\"]";

        public static readonly string ResponseStatusTql =
                "=>SUBPARTS:XModuleAttribute[Name==\"Communicate\"]=>SubAttributes:XModuleAttribute[Name==\"Receive\"]=>SubAttributes[Name==\"Status code name\"]";

        public static readonly string TestStepAddressValueTql =
                "=>SUBPARTS:XTestStepValue[Name ==\"Communicate\"]=>SubValues:XTestStepValue[Name==\"Address\"]";

        public static readonly string TestStepMethodTql =
                "=>SUBPARTS:XTestStepValue[Name==\"Communicate\"]=>SUBPARTS:XTestStepValue[Name==\"Send\"]=>SUBPARTS:XTestStepValue[Name==\"Method\"]";

        public static readonly string TestStepRequestHeadersTql =
                "=>SUBPARTS: XTestStepValue[Name==\"Send\"]=>SUBPARTS: XTestStepValue[Name==\"Headers\"]=>SUBPARTS";

        public static readonly string TestStepResponseHeadersTql =
                "=>SUBPARTS:XTestStepValue[Name==\"Receive\"]=>SUBPARTS:XTestStepValue[Name==\"Headers\"]=>SUBPARTS";

        public static readonly string TestStepResponseStatusTql =
                "=>SUBPARTS:XTestStepValue[Name==\"Receive\"]=>SUBPARTS:XTestStepValue[Name==\"Status code name\"]";

        public static readonly string TestStepXmlPayloadTql =
                "=>SUBPARTS:XModuleAttribute[BusinessType==\"XmlElement\"]";

        private static readonly Version TcVersion = new Version(TCAddOn.ActiveWorkspace.GetVersionInfo());

        public static readonly string ToscaVersion = $"{TcVersion.Major}.{TcVersion.Minor}";

        #endregion
    }
}