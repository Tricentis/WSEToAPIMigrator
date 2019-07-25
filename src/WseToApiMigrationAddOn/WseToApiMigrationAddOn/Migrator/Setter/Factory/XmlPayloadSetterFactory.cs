using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Interfaces;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.RequestSetter;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.ResponseSetter;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Factory {
    public class XmlPayloadSetterFactory : IPayloadSetterFactory {
        #region Public Methods and Operators

        public IPayloadSetter GetRequestPayloadSetter() {
            return new ApiRequestXmlPayloadSetter();
        }

        public IPayloadSetter GetResponsePayloadSetter() {
            return new ApiResponseXmlPayloadSetter();
        }

        #endregion
    }
}