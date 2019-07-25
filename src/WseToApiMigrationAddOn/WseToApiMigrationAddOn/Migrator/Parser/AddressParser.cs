using System;
using System.Linq;

using Tricentis.TCAPIObjects.Objects;

using WseToApiMigrationAddOn.Extensions;
using WseToApiMigrationAddOn.Helper;
using WseToApiMigrationAddOn.Migrator.Parser.Interfaces;
using WseToApiMigrationAddOn.Migrator.Parser.Model;
using WseToApiMigrationAddOn.Shared;

namespace WseToApiMigrationAddOn.Migrator.Parser {
    /// <summary>
    /// Extracts Endpoint, Resource and Query Params from Wse Artifacts
    /// </summary>
    public class AddressParser : IWseValueParser {
        #region Public Methods and Operators

        /// <summary>
        /// Get Endpoint, Resource and Query Params from WSE Module
        /// </summary>
        /// <param name="wseModule"></param>
        /// <returns>AddressParserResult object which contains Endpoint, Resource and Query Params</returns>
        public AddressParserResult Parse(XModule wseModule) {
            var addressParserResult = new AddressParserResult();
            try {
                TCObject addressValueTql =
                        wseModule.Search(AddOnConstants.AddressValueTql).FirstOrDefault();
                if (addressValueTql == null) return addressParserResult;

                XModuleAttribute addressValue = addressValueTql as XModuleAttribute;

                if (string.IsNullOrEmpty(addressValue?.DefaultValue)) return addressParserResult;
                ParseAddressInternal(addressParserResult, addressValue.DefaultValue);
            }
            catch (Exception ex) {
                FileLogger.Instance.Error(ex);
            }

            return addressParserResult;
        }

        /// <summary>
        /// Get Endpoint, Resource and Query Params from WSE XTestStep
        /// </summary>
        /// <param name="xTestStep"></param>
        /// <returns>AddressParserResult object which contains Endpoint, Resource and Query Params</returns>
        public AddressParserResult Parse(XTestStep xTestStep) {
            var addressParserResult = new AddressParserResult();
            try {
                XTestStepValue address = (XTestStepValue)
                        xTestStep.Search(AddOnConstants.TestStepAddressValueTql).FirstOrDefault();
                if (address == null) return addressParserResult;
                if (string.IsNullOrEmpty(address.Value)) return addressParserResult;
                ParseAddressInternal(addressParserResult, address.Value);
            }
            catch (Exception ex) {
                FileLogger.Instance.Error(ex);
            }

            return addressParserResult;
        }

        #endregion

        #region Methods

        private static void ParseAddressInternal(AddressParserResult addressParserResult, string addressValue) {
            addressParserResult.Endpoint = UriHelper.GetEndPoint(addressValue);
            addressParserResult.Resource = UriHelper.GetResource(addressValue);
            addressParserResult.QueryParams = UriHelper.DecodeQueryParameters(addressValue);
        }

        #endregion
    }
}