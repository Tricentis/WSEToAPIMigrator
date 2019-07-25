using System;
using System.Reflection;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Helper {
    /// <summary>
    /// Load a assembly at run-time and get its class and Method
    /// </summary>
    public static class AssemblyHelper {
        #region Public Methods and Operators

        /// <summary>
        /// Load a assembly at run-time and get its class and Method
        /// </summary>
        /// <param name="assemblyName">Name of assembly to load</param>
        /// <param name="className">Name of class</param>
        /// <param name="methodName">Name of method</param>
        /// <param name="methodParams">Paameters of method</param>
        /// <returns>return class and method</returns>
        public static (Type type, MethodInfo methodInfo) GetClassTypeAndMethodTypeFromAssembly(
                string assemblyName,
                string className,
                string methodName,
                Type[] methodParams) {
            var asm = Assembly.Load(assemblyName);
            if (asm == null)
                throw new Exception(string.Format("Unable to load assembly {0}", assemblyName));
            var type = asm.GetType(className);
            if (type == null)
                throw new Exception(string.Format("Class not found {0}", className));
            var methodInfo = type.GetMethod(methodName, methodParams);
            if (methodInfo == null)
                throw new Exception(string.Format("Method not found {0}", methodName));
            return (type, methodInfo);
        }

        #endregion
    }
}