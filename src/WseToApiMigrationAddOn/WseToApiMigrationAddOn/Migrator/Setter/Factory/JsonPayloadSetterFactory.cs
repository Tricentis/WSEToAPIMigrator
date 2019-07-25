using WseToApiMigrationAddOn.Migrator.Setter.Interfaces;
using WseToApiMigrationAddOn.Migrator.Setter.RequestSetter;
using WseToApiMigrationAddOn.Migrator.Setter.ResponseSetter;

namespace WseToApiMigrationAddOn.Migrator.Setter.Factory {
    public class JsonPayloadSetterFactory : IPayloadSetterFactory {
        #region Public Methods and Operators

        public IPayloadSetter GetRequestPayloadSetter() {
            return new ApiRequestJsonPayloadSetter();
        }

        public IPayloadSetter GetResponsePayloadSetter() {
            return new ApiResponseJsonPayloadSetter();
        }

        #endregion
    }
}