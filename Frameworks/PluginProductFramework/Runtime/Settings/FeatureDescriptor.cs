using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Describes a feature and its settings template.
    /// </summary>
    [CreateAssetMenu(menuName = "Voxel Busters/Feature Descriptor", fileName = "FeatureDescriptor")]
    public class FeatureDescriptor : ScriptableObject
    {
        #region Fields

        [FormerlySerializedAs("m_featureName")]
        [SerializeField]
        private string m_featureCodeName;

        [SerializeField]
        private string m_featureDisplayName;

        [SerializeField]
        private string m_featureDescription;

        // NOTE: This links the feature back to its owning product so editor cleanup
        // can remain root-path independent when features are moved or deleted.
        [SerializeField]
        private PluginProductDescriptor m_productDescriptor;

        [SerializeField]
        private FeatureSettings m_settingsTemplate;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the feature code name used for identifiers and paths.
        /// </summary>
        public string FeatureCodeName => m_featureCodeName;

        /// <summary>
        /// Gets the human-readable display name for the feature.
        /// </summary>
        public string FeatureDisplayName => m_featureDisplayName;

        /// <summary>
        /// Gets the short description for the feature.
        /// </summary>
        public string FeatureDescription => m_featureDescription;

        /// <summary>
        /// Gets the owning product descriptor.
        /// </summary>
        public PluginProductDescriptor ProductDescriptor => m_productDescriptor;

        /// <summary>
        /// Gets the template settings asset used to create new settings instances.
        /// </summary>
        public FeatureSettings SettingsTemplate => m_settingsTemplate;

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new settings instance using the template.
        /// </summary>
        public FeatureSettings CreateSettingsInstance()
        {
            if (m_settingsTemplate != null)
            {
                var instance = Instantiate(m_settingsTemplate);
                instance.Initialize(this);
                return instance;
            }

            Debug.LogWarning(
                $"[FeatureDescriptor] Settings template is missing for feature '{m_featureCodeName}'.",
                this);
            return null;
        }

        /// <summary>
        /// Sets the feature code name.
        /// </summary>
        public void SetFeatureCodeName(string featureCodeName)
        {
            m_featureCodeName = featureCodeName;
        }

        /// <summary>
        /// Sets the feature display name.
        /// </summary>
        public void SetFeatureDisplayName(string featureDisplayName)
        {
            m_featureDisplayName = featureDisplayName;
        }

        /// <summary>
        /// Sets the feature description.
        /// </summary>
        public void SetFeatureDescription(string featureDescription)
        {
            m_featureDescription = featureDescription;
        }

        /// <summary>
        /// Sets the product descriptor.
        /// </summary>
        public void SetProductDescriptor(PluginProductDescriptor productDescriptor)
        {
            m_productDescriptor = productDescriptor;
        }

        /// <summary>
        /// Sets the settings template.
        /// </summary>
        public void SetSettingsTemplate(FeatureSettings settingsTemplate)
        {
            m_settingsTemplate = settingsTemplate;
        }

        #endregion

        #region Unity Callbacks

        /// <summary>
        /// Ensures default values are populated in the editor.
        /// </summary>
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(m_featureCodeName))
            {
                m_featureCodeName = name;
            }

            if (string.IsNullOrEmpty(m_featureDisplayName) && !string.IsNullOrEmpty(m_featureCodeName))
            {
                m_featureDisplayName = m_featureCodeName;
            }

            if (string.IsNullOrEmpty(m_featureDescription))
            {
                m_featureDescription = string.Empty;
            }
        }

        #endregion
    }
}
