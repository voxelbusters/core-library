using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Describes a plugin product and its settings asset.
    /// </summary>
    [CreateAssetMenu(menuName = "Voxel Busters/PluginProduct Descriptor", fileName = "PluginProductDescriptor")]
    public class PluginProductDescriptor : ScriptableObject
    {
        #region Fields

        [FormerlySerializedAs("m_productName")]
        [SerializeField]
        private string m_productCodeName;

        [SerializeField]
        private string m_productDisplayName;

        [SerializeField]
        private string m_settingsAssetPath;

        [SerializeField]
        private PluginProductGeneralSettingsBase m_generalSettingsTemplate;

        [SerializeField]
        private PluginProductResourcesSettingsBase m_resourcesSettingsTemplate;

        [SerializeField]
        private Texture2D m_productIcon;

        [SerializeField]
        private string m_copyright;

        [SerializeField]
        private string m_version;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the product code name used for identifiers and paths.
        /// </summary>
        public string ProductCodeName => m_productCodeName;

        /// <summary>
        /// Gets the human-readable display name for the product.
        /// </summary>
        public string ProductDisplayName => m_productDisplayName;

        /// <summary>
        /// Gets the settings asset path for the product.
        /// </summary>
        public string SettingsAssetPath => m_settingsAssetPath;

        /// <summary>
        /// Gets the template for product-wide general settings.
        /// </summary>
        public PluginProductGeneralSettingsBase GeneralSettingsTemplate => m_generalSettingsTemplate;

        /// <summary>
        /// Gets the template for product-wide resources settings.
        /// </summary>
        public PluginProductResourcesSettingsBase ResourcesSettingsTemplate => m_resourcesSettingsTemplate;


        /// <summary>
        /// Gets the product icon used in settings UI.
        /// </summary>
        public Texture2D ProductIcon => m_productIcon;

        /// <summary>
        /// Gets the copyright text for this product.
        /// </summary>
        public string Copyright => m_copyright;

        /// <summary>
        /// Gets the product version.
        /// </summary>
        public string Version => m_version;

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the product code name.
        /// </summary>
        public void SetProductCodeName(string productCodeName)
        {
            m_productCodeName = productCodeName;
        }

        /// <summary>
        /// Sets the product display name.
        /// </summary>
        public void SetProductDisplayName(string productDisplayName)
        {
            m_productDisplayName = productDisplayName;
        }

        /// <summary>
        /// Sets the settings asset path.
        /// </summary>
        public void SetSettingsAssetPath(string settingsAssetPath)
        {
            m_settingsAssetPath = settingsAssetPath;
        }

        /// <summary>
        /// Sets the general settings template.
        /// </summary>
        public void SetGeneralSettingsTemplate(PluginProductGeneralSettingsBase template)
        {
            m_generalSettingsTemplate = template;
        }

        /// <summary>
        /// Sets the resources settings template.
        /// </summary>
        public void SetResourcesSettingsTemplate(PluginProductResourcesSettingsBase template)
        {
            m_resourcesSettingsTemplate = template;
        }

        /// <summary>
        /// Sets the product icon.
        /// </summary>
        public void SetProductIcon(Texture2D productIcon)
        {
            m_productIcon = productIcon;
        }

        /// <summary>
        /// Sets the copyright text.
        /// </summary>
        public void SetCopyright(string copyright)
        {
            m_copyright = copyright;
        }

        /// <summary>
        /// Sets the product version.
        /// </summary>
        public void SetVersion(string version)
        {
            m_version = version;
        }

        #endregion

        #region Unity Callbacks

        /// <summary>
        /// Ensures default values are populated in the editor.
        /// </summary>
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(m_productCodeName))
            {
                m_productCodeName = name;
            }

            if (string.IsNullOrEmpty(m_productDisplayName) && !string.IsNullOrEmpty(m_productCodeName))
            {
                m_productDisplayName = m_productCodeName;
            }

            if (string.IsNullOrEmpty(m_settingsAssetPath) && !string.IsNullOrEmpty(m_productCodeName))
            {
                m_settingsAssetPath = $"Assets/Resources/{m_productCodeName}Settings.asset";
            }

        }

        #endregion
    }
}
