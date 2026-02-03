using UnityEngine;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{

    /// <summary>
    /// Base class for per-platform configuration assets.
    /// </summary>
    public abstract class PlatformConfigurationObject : ScriptableObject
    {
        #region Fields

        [SerializeField]
        private PlatformConfigurationUpdaterBase m_configurationUpdater;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the platform type for this configuration asset.
        /// </summary>
        public abstract PlatformType Platform { get; }

        /// <summary>
        /// Gets the updater asset used to sync this configuration.
        /// </summary>
        public PlatformConfigurationUpdaterBase ConfigurationUpdater => m_configurationUpdater;

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the updater asset used to sync this configuration.
        /// </summary>
        public void SetConfigurationUpdater(PlatformConfigurationUpdaterBase configurationUpdater)
        {
            m_configurationUpdater = configurationUpdater;
        }

        #endregion
    }
}
