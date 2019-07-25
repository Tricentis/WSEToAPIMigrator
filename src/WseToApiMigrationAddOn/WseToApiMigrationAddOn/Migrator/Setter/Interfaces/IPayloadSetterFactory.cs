namespace WseToApiMigrationAddOn.Migrator.Setter.Interfaces {
    public interface IPayloadSetterFactory {
        #region Public Methods and Operators

        IPayloadSetter GetRequestPayloadSetter();

        IPayloadSetter GetResponsePayloadSetter();

        #endregion
    }
}