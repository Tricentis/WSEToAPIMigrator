using System;
using System.Reflection;

using Tricentis.Automation.WseToApiMigrationAddOn.Extensions;
using Tricentis.Automation.WseToApiMigrationAddOn.Helper;
using Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.Interfaces;
using Tricentis.Automation.WseToApiMigrationAddOn.Shared;
using Tricentis.TCAPIObjects.Objects;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Migrator.Setter.RequestSetter {
    /// <summary>
    /// Sets basic authentication in API module 
    /// </summary>
    public class ApiRequestBasicAuthSetter : IApiRequestValueSetter {
        #region Properties

        protected string TqlToGetWseAuthTestStepValue { get; set; } =
            "=>INTERSECTION(=>SUBPARTS:XTestStepValue[Name=?\"{0}\"], =>return SUBPARTS:XTestStepValue[Name=?\"{0}\"]->ModuleAttribute[Name=?\"{0}\"]->Module[ParameterType=?\"BasicAuthentication\"]->Generalization[ScanTag=?\"Authentication\"])";

        private string AssemblyName { get; } = "Tricentis.Common, Culture=neutral, PublicKeyToken=37ebde39b742d2cc";

        private string ClassName { get; } = "Tricentis.Common.Security.O_CF_NR.CommonCrypto";

        private string MethodName { get; } = "DecryptToString";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Sets basic authentication properties such as type, username, password and pre-authenticate in API module 
        /// </summary>
        /// <param name="apiTestStep">returns module of apiTeststep</param>
        /// <param name="wseTestStep">returns username, password, preauthenticate information of wseTestStep</param>
        public void Execute(XTestStep apiTestStep, XTestStep wseTestStep) {
            try {
                FileLogger.Instance.Info("Inside Basic Auth Credentials Migrator");
                string userName = GetWseUsernameValue(wseTestStep);
                string password = GetWsePasswordValue(wseTestStep);
                string preAuthenticate = GetWsePreAuthenticateValue(wseTestStep);
                if (!string.IsNullOrEmpty(userName) || !string.IsNullOrEmpty(password)
                                                    || !string.IsNullOrEmpty(preAuthenticate)) {
                    ApiModule apiModule = (ApiModule)apiTestStep.Module;
                    apiModule.AddXParamToModuleAttribute("AuthenticationType", "Basic", ParamTypeE.TechnicalID);
                    apiModule.AddXParamToModuleAttribute("Username", userName, ParamTypeE.TechnicalID);
                    apiModule.AddXParamToModuleAttribute("Password", password, ParamTypeE.TechnicalID);
                    apiModule.AddXParamToModuleAttribute("PreAuthenticate",
                                                         preAuthenticate == "Yes" ? "true" : "false",
                                                         ParamTypeE.TechnicalID);
                }

                FileLogger.Instance.Info("Complete Basic Auth Credentials Migrator");
            }
            catch (Exception ex) {
                FileLogger.Instance.Error(
                        $"Error occurred while migration of Auth Credentials for WSE TestStep : 'Name: {wseTestStep?.Name}' NodePath:'{wseTestStep?.NodePath}' Exception:'{ex.ToString()}'");
            }
        }

        #endregion

        #region Methods

        private string GetTestStepValue(XTestStep wseTestStep, string searchString) {
            string value = string.Empty;
            var tcObjects = wseTestStep.Search(String.Format(TqlToGetWseAuthTestStepValue, searchString));
            foreach (var tcObject in tcObjects) {
                if (tcObject is XTestStepValue) {
                    var x = tcObject as XTestStepValue;
                    return x.Value;
                }
            }

            return value;
        }

        private string GetWsePasswordValue(XTestStep wseTestStep) {
            FileLogger.Instance.Info("Inside Basic Auth Credentials Password Migrator");
            Type[] methodParams = new[] { typeof(string) };
            string decrptedPassword = string.Empty;
            string encryptedPassword = string.Empty;
            try {
                encryptedPassword = GetTestStepValue(wseTestStep, "Password");
                if (!string.IsNullOrEmpty(encryptedPassword)) {
                    var (type, methodInfo) =
                            AssemblyHelper.GetClassTypeAndMethodTypeFromAssembly(
                                    AssemblyName,
                                    ClassName,
                                    MethodName,
                                    methodParams);
                    var flags = BindingFlags.Public | BindingFlags.Static;
                    var runnable = type.GetProperty("Instance", flags);
                    var result =
                            methodInfo?.Invoke(runnable?.GetValue(null), new object[] { encryptedPassword });
                    decrptedPassword = result?.ToString();
                }
            }
            catch (Exception ex) {
                FileLogger.Instance.Error(
                        $"Error occurred while migrating of Basic Auth Password for WSE TestStep : 'Name: {wseTestStep?.Name}' NodePath:'{wseTestStep?.NodePath}' Exception:'{ex.ToString()}'");
            }

            FileLogger.Instance.Info("Complete Basic Auth Credentials Password Migrator");

            if (String.IsNullOrEmpty(decrptedPassword))
                return encryptedPassword;

            return decrptedPassword;
        }

        private string GetWsePreAuthenticateValue(XTestStep wseTestStep) {
            return GetTestStepValue(wseTestStep, "PreAuthenticate");
        }

        private string GetWseUsernameValue(XTestStep wseTestStep) {
            return GetTestStepValue(wseTestStep, "Username");
        }

        #endregion
    }
}