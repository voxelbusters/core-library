using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework.Editor
{
    public static class PluginProductDescriptorUtility
    {
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
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;
            }

            string rootPath = Path.GetDirectoryName(assetPath);
            return NormalizeAssetPath(rootPath);
        }

        public static string GetAssetsProductRootPath(PluginProductDescriptor descriptor)
        {
            if (descriptor == null || string.IsNullOrEmpty(descriptor.ProductCodeName))
            {
                return null;
            }

            string rootPath = Path.Combine(PluginProductFrameworkConstants.AssetsProductsRootPath,
                descriptor.ProductCodeName);
            return NormalizeAssetPath(rootPath);
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
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(PluginProductDescriptor)}");
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
