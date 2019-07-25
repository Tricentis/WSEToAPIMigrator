namespace WseToApiMigrationAddOn.Migrator.Handler.SpecializationHandlers.Factory {
    /// <summary>
    /// Initializes specialization chain
    /// </summary>
    public static class SpecializationFactory {
        #region Public Methods and Operators
        /// <summary>
        /// Initializes Request specialization chain
        /// </summary>
        /// <returns>Instance of Request specialization</returns>
        public static AbstractSpecializationHandler GetRequestSpecializationHandler() {
            AbstractSpecializationHandler h1 = new EmbeddedRequestSpecializationHandler();
            AbstractSpecializationHandler h2 = new RequestAsResourceSpecializationHandler();
            h1.SetSuccessor(h2);
            return h1;
        }
        /// <summary>
        /// Initializes Response specialization chain
        /// </summary>
        /// <returns>Instance of Response specialization</returns>
        public static AbstractSpecializationHandler GetResponseSpecializationHandler() {
            AbstractSpecializationHandler h1 = new EmbeddedResponseSpecializationHandler();
            AbstractSpecializationHandler h2 = new ResponseAsResourceSpecializationHandler();
            AbstractSpecializationHandler h3 = new ResponseAsPlainTextSpecializationHandler();
            h1.SetSuccessor(h2);
            h2.SetSuccessor(h3);
            return h1;
        }

        #endregion
    }
}