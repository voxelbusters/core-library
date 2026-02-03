using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework.Editor
{
    /// <summary>
    /// Base class for product bootstrappers that register observers and processing callbacks.
    /// </summary>
    internal abstract class PluginProductBootstrapperBase
    {
        /// <summary>
        /// Registers platform and feature observers for this product.
        /// </summary>
        protected void Register(string productCodeName)
        {
            PluginProductDescriptor descriptor = PluginProductDescriptorUtility.FindDescriptorByCodeName(productCodeName);
            if (descriptor == null)
            {
                return;
            }

            FeatureSettingsObserver.Register(descriptor, OnFeatureSettingsChanged);
            PlatformConfigurationObserver.Register(descriptor, OnPlatformConfigurationChanged);
        }

        /// <summary>
        /// Handles feature settings changes.
        /// </summary>
        protected virtual void OnFeatureSettingsChanged(PluginProductDescriptor descriptor,
                                                        FeatureSettings featureSettings)
        {
            FeatureActivationUtility.UpdateImporters(descriptor, featureSettings);
            FeaturePlatformConfigurationUpdateRunner.UpdateFromFeatureSettings(descriptor, featureSettings);
        }

        /// <summary>
        /// Handles platform configuration changes.
        /// </summary>
        protected virtual void OnPlatformConfigurationChanged(PluginProductDescriptor descriptor)
        {
            PlatformPreBuildProcessingRunner.ProcessFromConfigurations(descriptor);
        }        
    }
}
