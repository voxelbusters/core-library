using System.Collections.Generic;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Base settings asset for a plugin product.
    /// </summary>
    public abstract class PluginProductSettings : SettingsObject, IPluginProductSettingsSummary
    {
        #region Fields

        [SerializeField]
        private List<FeatureSettings> m_featureSettings = new List<FeatureSettings>();

        [SerializeField]
        private PluginProductGeneralSettingsBase m_generalSettings;

        [SerializeField]
        private PluginProductResourcesSettingsBase m_resourcesSettings;

        [SerializeField]
        private PluginProductDescriptor m_descriptor;

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the cached feature settings list.
        /// </summary>
        public void SetFeatures(List<FeatureSettings> featureSettings)
        {
            m_featureSettings = featureSettings ?? new List<FeatureSettings>();
        }

        public PluginProductGeneralSettingsBase GeneralSettings => m_generalSettings;

        public PluginProductResourcesSettingsBase ResourcesSettings => m_resourcesSettings;

        public PluginProductDescriptor Descriptor => m_descriptor;

        public FeatureSettings GetFeatureSettings(string featureCodeName)
        {
            if (string.IsNullOrEmpty(featureCodeName))
            {
                return null;
            }

            for (int i = 0; i < m_featureSettings.Count; i++)
            {
                FeatureSettings feature = m_featureSettings[i];
                if (feature != null && feature.FeatureDescriptor != null &&
                    string.Equals(feature.FeatureDescriptor.FeatureCodeName, featureCodeName, System.StringComparison.Ordinal))
                {
                    return feature;
                }
            }

            return null;
        }

        public FeatureSettings GetFeatureSettings(System.Type settingsType)
        {
            if (settingsType == null)
            {
                return null;
            }

            for (int i = 0; i < m_featureSettings.Count; i++)
            {
                FeatureSettings feature = m_featureSettings[i];
                if (feature != null && settingsType.IsInstanceOfType(feature))
                {
                    return feature;
                }
            }

            return null;
        }

        public TSettings GetFeatureSettings<TSettings>() where TSettings : FeatureSettings
        {
            return GetFeatureSettings(typeof(TSettings)) as TSettings;
        }

        public void SetGeneralSettings(PluginProductGeneralSettingsBase generalSettings)
        {
            m_generalSettings = generalSettings;
        }

        public void SetResourcesSettings(PluginProductResourcesSettingsBase resourcesSettings)
        {
            m_resourcesSettings = resourcesSettings;
        }

        public void SetDescriptor(PluginProductDescriptor descriptor)
        {
            m_descriptor = descriptor;
        }

        protected abstract PluginProductGeneralSettingsBase CreateGeneralSettingsInstance();

        protected abstract PluginProductResourcesSettingsBase CreateResourcesSettingsInstance();

        public PluginProductGeneralSettingsBase CreateGeneralSettings()
        {
            return CreateGeneralSettingsInstance();
        }

        public PluginProductResourcesSettingsBase CreateResourcesSettings()
        {
            return CreateResourcesSettingsInstance();
        }

        #endregion
    }
}
