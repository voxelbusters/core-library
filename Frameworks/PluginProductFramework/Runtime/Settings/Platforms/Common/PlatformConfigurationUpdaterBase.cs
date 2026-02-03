using UnityEngine;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Base class for feature-specific platform configuration updaters.
    /// </summary>
    public abstract class PlatformConfigurationUpdaterBase : ScriptableObject
    {
        /// <summary>
        /// Platform this updater targets.
        /// </summary>
        public abstract PlatformType Platform { get; }

        /// <summary>
        /// Updates the platform configuration from the feature settings.
        /// </summary>
        public abstract void UpdateConfiguration(PluginProductDescriptor descriptor,
                                                 FeatureSettings settings,
                                                 PlatformConfigurationObject configuration);
    }
}
