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

            string productRootPath = PluginProductDescriptorUtility.GetProductRootPath(descriptor);
            if (string.IsNullOrEmpty(productRootPath))
            {
                return;
            }

            string featureRoot = Path.Combine(productRootPath, "Features", featureCodeName).Replace('\\', '/');
            UpdateFeatureConfigurations(descriptor, featureSettings, featureRoot);
        }

        private static void UpdateFeatureConfigurations(PluginProductDescriptor descriptor,
                                                        FeatureSettings featureSettings,
                                                        string featureRoot)
        {
            if (string.IsNullOrEmpty(featureRoot))
            {
                return;
            }

            string[] guids = AssetDatabase.FindAssets($"t:{nameof(PlatformConfigurationObject)}", new[] { featureRoot });
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var config = AssetDatabase.LoadAssetAtPath<PlatformConfigurationObject>(path);
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
