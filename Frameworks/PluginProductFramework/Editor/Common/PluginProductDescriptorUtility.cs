using System;
using System.Collections.Generic;
using UnityEditor;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework.Editor
{
    public static class PluginProductDescriptorUtility
    {
        private const string kAllProductsRootSearchPath = "Assets/Plugins/VoxelBusters";

        public static PluginProductDescriptor FindDescriptorByCodeName(string productCodeName)
        {
            if (string.IsNullOrEmpty(productCodeName))
            {
                return null;
            }

            var descriptors = FindAllPluginProductDescriptors();
            for (int i = 0; i < descriptors.Count; i++)
            {
                var descriptor = descriptors[i];
                if (descriptor != null && string.Equals(descriptor.ProductCodeName, productCodeName, StringComparison.Ordinal))
                {
                    return descriptor;
                }
            }

            return null;
        }

        public static string GetProductRootPath(PluginProductDescriptor descriptor)
        {
            if (descriptor == null)
            {
                return null;
            }

            string assetPath = AssetDatabase.GetAssetPath(descriptor);
            return PlatformConfigurationUtility.GetProductRootFromPath(assetPath);
        }

        public static PluginProductDescriptor FindPluginProductDescriptorForSettingsAsset(string settingsAssetPath)
        {
            if (string.IsNullOrEmpty(settingsAssetPath))
            {
                return null;
            }

            string normalizedSettingsPath = NormalizeAssetPath(settingsAssetPath);
            foreach (var descriptor in FindAllPluginProductDescriptors())
            {
                if (descriptor == null)
                {
                    continue;
                }

                string settingsAssetPathNormalized = NormalizeAssetPath(descriptor.SettingsAssetPath);
                if (string.IsNullOrEmpty(settingsAssetPathNormalized) && !string.IsNullOrEmpty(descriptor.ProductCodeName))
                {
                    settingsAssetPathNormalized = $"Assets/Resources/{descriptor.ProductCodeName}Settings.asset";
                }

                if (string.Equals(settingsAssetPathNormalized, normalizedSettingsPath, StringComparison.Ordinal))
                {
                    return descriptor;
                }
            }

            return null;
        }

        public static List<PluginProductDescriptor> FindAllPluginProductDescriptors()
        {
            var results = new List<PluginProductDescriptor>();
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(PluginProductDescriptor)}", new[] { kAllProductsRootSearchPath });
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = AssetDatabase.LoadAssetAtPath<PluginProductDescriptor>(path);
                if (asset != null)
                {
                    results.Add(asset);
                }
            }
            return results;
        }

        public static string NormalizeAssetPath(string assetPath)
        {
            return string.IsNullOrEmpty(assetPath) ? null : assetPath.Replace('\\', '/');
        }
    }
}
