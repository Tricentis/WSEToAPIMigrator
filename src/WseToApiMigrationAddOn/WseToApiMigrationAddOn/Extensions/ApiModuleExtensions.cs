using System.Collections.Generic;

using Tricentis.TCAPIObjects.Objects;

namespace WseToApiMigrationAddOn.Extensions {
    /// <summary>
    /// Extension Methods for API XModule
    /// </summary>
    public static class ApiModuleExtensions {
        #region Public Methods and Operators
        /// <summary>
        /// Creates configuration param
        /// </summary>
        /// <param name="module">API Module</param>
        /// <param name="name">Name of the configuration parameter</param>
        /// <param name="value">Value of the configuration parameter</param>
        public static void AddConfigurationParam(this ApiModule module, string name, string value) {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value))
                return;
            XParam newXParam = module.CreateConfigurationParam();
            newXParam.Name = name;
            newXParam.Value = value;
            newXParam.ParamType = ParamTypeE.Configuration;
            if (name == "Password") newXParam.Visible = false;
        }
        /// <summary>
        /// Add HTTP Headers to API Module
        /// </summary>
        /// <param name="apiModule">API Module</param>
        /// <param name="headers">Dictionary of headers to be added</param>
        public static void AddHeaders(this ApiModule apiModule, Dictionary<string, string> headers) {
            foreach (KeyValuePair<string, string> header in headers)
                apiModule.APIAddMessageHeader(header.Key, header.Value);
        }

        /// <summary>
        /// Add params of type Path to the Params section in tosca
        /// </summary>
        /// <param name="apiModule">API Module</param>
        /// <param name="pathParams">Dictionary of Path params to be added</param>
        public static void AddPathParams(this ApiModule apiModule, Dictionary<string, string> pathParams) {
            foreach (KeyValuePair<string, string> pathParam in pathParams)
                apiModule.APIAddMessageParameter(pathParam.Key, pathParam.Value, "Path");
        }

        /// <summary>
        /// Add params of type Query to the Params section in tosca
        /// </summary>
        /// <param name="apiModule">API Module</param>
        /// <param name="pathParams">Dictionary of query params to be added</param>
        public static void AddQueryParams(this ApiModule apiModule, Dictionary<string, string> queryParams) {
            foreach (KeyValuePair<string, string> queryParam in queryParams)
                apiModule.APIAddMessageParameter(queryParam.Key, queryParam.Value, "Query");
            apiModule.AddTechnicalIdParam("QueryParams", string.Join(";", queryParams));
        }

        /// <summary>
        /// Creates technical param in the property section of module
        /// </summary>
        /// <param name="module">API Module</param>
        /// <param name="name">Name of the Technical parameter</param>
        /// <param name="value">Value of the Technical parameter</param>
        /// <param name="visibility">if the param will be visible in the property section</param>
        public static void AddTechnicalIdParam(this ApiModule module,
                                               string name,
                                               string value,
                                               bool visibility = true) {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value))
                return;
            XParam newXParam = module.CreateConfigurationParam();
            newXParam.Name = name;
            newXParam.Value = value;
            newXParam.ParamType = ParamTypeE.TechnicalID;
            if (!visibility) newXParam.Visible = false;
            if (name == "Password") newXParam.Visible = false;
        }

        /// <summary>
        /// Creates Module Attribute
        /// </summary>
        /// <param name="apiModule">ApiModule</param>
        /// <param name="name"> Name of the Module Attribute</param>
        /// <param name="valueRange">ValueRange of the Module Attribute</param>
        /// <param name="value">Default value of the Module Attribute</param>
        /// <param name="pathType">pathType of the Module Attribute</param>
        /// <param name="path">path of the Module Attribute</param>
        /// <param name="actionMode">Default ActionMode of the Module Attribute</param>
        /// <param name="dataType">Data Type of the Module Attribute</param>
        /// <returns>Module Attribute</returns>
        public static XModuleAttribute CreateModuleAttribute(this ApiModule apiModule,
                                                             string name,
                                                             string valueRange,
                                                             string value,
                                                             string pathType,
                                                             string path,
                                                             XTestStepActionMode actionMode =
                                                                     XTestStepActionMode.Verify,
                                                             ModuleAttributeDataType dataType =
                                                                     ModuleAttributeDataType.String) {
            XModuleAttribute xModuleAttribute =
                    apiModule.CreateModuleAttribute();
            xModuleAttribute.Name = name;
            xModuleAttribute.DefaultValue = value;
            if (!string.IsNullOrEmpty(valueRange))
                xModuleAttribute.ValueRange = valueRange;
            xModuleAttribute.EnsureUniqueName();
            xModuleAttribute.AddXParamToModuleAttribute("PathType", pathType, ParamTypeE.TechnicalID);
            xModuleAttribute.AddXParamToModuleAttribute("Path", path, ParamTypeE.TechnicalID);
            xModuleAttribute.AddXParamToModuleAttribute("ExplicitName", "True", ParamTypeE.Configuration);
            xModuleAttribute.DefaultActionMode = actionMode;
            xModuleAttribute.DefaultDataType = dataType;
            return xModuleAttribute;
        }

        /// <summary>
        /// Creates Module Attribute
        /// </summary>
        /// <param name="apiModule">ApiModule</param>
        /// <param name="name"> Name of the Module Attribute</param>
        /// <param name="value">Default value of the Module Attribute which is also used as value range</param>
        /// <param name="pathType">pathType of the Module Attribute</param>
        /// <param name="path">path of the Module Attribute</param>
        /// <param name="actionMode">Default ActionMode of the Module Attribute</param>
        /// <param name="dataType">Data Type of the Module Attribute</param>
        /// <returns>Module Attribute</returns>
        public static XModuleAttribute CreateModuleAttribute(this ApiModule apiModule,
                                                             string name,
                                                             string value,
                                                             string pathType,
                                                             string path,
                                                             XTestStepActionMode actionMode =
                                                                     XTestStepActionMode.Verify,
                                                             ModuleAttributeDataType dataType =
                                                                     ModuleAttributeDataType.String) {
            XModuleAttribute xModuleAttribute =
                    apiModule.CreateModuleAttribute();
            xModuleAttribute.Name = name;
            xModuleAttribute.DefaultValue = value;
            xModuleAttribute.ValueRange = value;
            xModuleAttribute.EnsureUniqueName();
            xModuleAttribute.AddXParamToModuleAttribute("PathType", pathType, ParamTypeE.TechnicalID);
            xModuleAttribute.AddXParamToModuleAttribute("Path", path, ParamTypeE.TechnicalID);
            xModuleAttribute.AddXParamToModuleAttribute("ExplicitName", "True", ParamTypeE.Configuration);
            xModuleAttribute.DefaultActionMode = actionMode;
            xModuleAttribute.DefaultDataType = dataType;
            return xModuleAttribute;
        }

        #endregion
    }
}