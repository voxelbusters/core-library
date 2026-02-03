using System.Collections.Generic;
using System.IO;
using UnityEditor;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework.Editor
{
    internal static class FeaturePlatformConfigurationUpdateRunner
    {
        internal static void UpdateFromFeatureSettings(PluginProductDescriptor descriptor)
        {
            if (descriptor == null)
            {
                return;
            }

            var settings = PluginProductSettingsUtility.TryLoadProductSettings(descriptor);
            if (settings == null)
            {
                return;
            }

            List<FeatureSettings> features = PluginProductSettingsUtility.GetFeatureSettings(settings);
            for (int i = 0; i < features.Count; i++)
            {
                var featureSettings = features[i];
                if (featureSettings == null)
                {
                    continue;
                }

                UpdateFromFeatureSettings(descriptor, featureSettings);
            }
        }

        internal static void UpdateFromFeatureSettings(PluginProductDescriptor descriptor,
                                                       FeatureSettings featureSettings)
        {
            if (descriptor == null || featureSettings == null)
            {
                return;
            }

            string featureCodeName = featureSettings.FeatureDescriptor?.FeatureCodeName;
            if (string.IsNullOrEmpty(featureCodeName))
            {
                return;
            }

            string templateProductRoot = PluginProductDescriptorUtility.GetProductRootPath(descriptor);
            string assetsProductRoot = PluginProductDescriptorUtility.GetAssetsProductRootPath(descriptor);
            if (string.IsNullOrEmpty(templateProductRoot) && string.IsNullOrEmpty(assetsProductRoot))
            {
                return;
            }

            string templateFeatureRoot = string.IsNullOrEmpty(templateProductRoot)
                ? null
                : Path.Combine(templateProductRoot, "Features", featureCodeName).Replace('\\', '/');
            string assetsFeatureRoot = string.IsNullOrEmpty(assetsProductRoot)
                ? null
                : Path.Combine(assetsProductRoot, "Features", featureCodeName).Replace('\\', '/');

            UpdateFeatureConfigurations(descriptor, featureSettings, templateFeatureRoot, assetsFeatureRoot);
        }

        private static void UpdateFeatureConfigurations(PluginProductDescriptor descriptor,
                                                        FeatureSettings featureSettings,
                                                        string templateFeatureRoot,
                                                        string assetsFeatureRoot)
        {
            if (string.IsNullOrEmpty(assetsFeatureRoot))
            {
                return;
            }

            var configs = PlatformConfigurationUtility.GetOrCreateEditableConfigs<PlatformConfigurationObject>(
                templateFeatureRoot,
                assetsFeatureRoot);
            for (int i = 0; i < configs.Count; i++)
            {
                var config = configs[i];
                if (config == null)
                {
                    continue;
                }

                var updater = config.ConfigurationUpdater as PlatformConfigurationUpdaterBase;
                if (updater == null || updater.Platform != config.Platform)
                {
                    continue;
                }

                updater.UpdateConfiguration(descriptor, featureSettings, config);
                EditorUtility.SetDirty(config);
            }
            AssetDatabase.SaveAssets();
        }
    }
}
