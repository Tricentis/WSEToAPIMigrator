using WseToApiMigrationAddOn.Migrator.Setter.Interfaces;
using WseToApiMigrationAddOn.Migrator.Setter.RequestSetter;
using WseToApiMigrationAddOn.Migrator.Setter.ResponseSetter;

namespace WseToApiMigrationAddOn.Migrator.Setter.Factory {
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