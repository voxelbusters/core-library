using UnityEngine;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Base class for per-feature settings stored as sub-assets.
    /// </summary>
    public abstract class FeatureSettings : ScriptableObject, IFeatureSettings, IFeatureSettingsValidator
    {
        #region Fields

        [SerializeField]
        [HideInInspector]
        private bool m_isEnabled = true;

        [SerializeField]
        [HideInInspector]
        private FeatureDescriptor m_featureDescriptor;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the feature descriptor associated with these settings.
        /// </summary>
        public FeatureDescriptor FeatureDescriptor => m_featureDescriptor;

        /// <summary>
        /// Gets a value indicating whether the feature is enabled.
        /// </summary>
        public bool IsEnabled => m_isEnabled;

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes this settings asset with a feature descriptor.
        /// </summary>
        public void Initialize(FeatureDescriptor descriptor)
        {
            m_featureDescriptor = descriptor;
        }

        /// <summary>
        /// Provides a validation message for this settings asset.
        /// </summary>
        public virtual bool TryGetValidationMessage(out string message, out FeatureSettingsValidationStatus status)
        {
            message = null;
            status = FeatureSettingsValidationStatus.None;
            return false;
        }

        #endregion
    }
}
