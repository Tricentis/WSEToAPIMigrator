using System.Linq;

using Tricentis.TCAPIObjects.Objects;

namespace WseToApiMigrationAddOn.Extensions {
    public static class XModuleAttributeExtensions {
        #region Public Methods and Operators

        public static void AddXParamToModuleAttribute(this XModuleAttribute moduleAttribute,
                                                      string name,
                                                      string value,
                                                      ParamTypeE xParamType) {
            XParam newXParam = moduleAttribute.CreateConfigurationParam();
            newXParam.Name = name;
            newXParam.Value = value;
            newXParam.ParamType = xParamType;
        }

        public static void AddXParamToModuleAttribute(this ApiModule module,
                                                      string name,
                                                      string value,
                                                      ParamTypeE xParamType) {
            var xparam = module.XParams.Where(x => x.Name == name);
            if (xparam == null || !xparam.Any()) {
                XParam newXParam = module.CreateConfigurationParam();
                newXParam.Name = name;
                newXParam.Value = value;
                newXParam.ParamType = xParamType;
                if (name == "Password") newXParam.Visible = false;
            }
        }

        #endregion
    }
}