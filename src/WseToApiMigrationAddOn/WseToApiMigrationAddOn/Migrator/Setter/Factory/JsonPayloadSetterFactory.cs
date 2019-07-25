using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Interfaces;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.RequestSetter;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.ResponseSetter;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Factory {
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