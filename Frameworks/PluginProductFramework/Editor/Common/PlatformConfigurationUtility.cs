using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework.Editor
{
    /// <summary>
    /// Helpers for locating platform configuration assets.
    /// </summary>
    internal static class PlatformConfigurationUtility
    {
        private const string kVoxelBustersRoot = "Assets/Plugins/VoxelBusters";

        public static string GetProductRootPath(string productRootRelativePath)
        {
            if (string.IsNullOrEmpty(productRootRelativePath))
            {
                return kVoxelBustersRoot;
            }

            string trimmed = productRootRelativePath.Trim().Replace('\\', '/').Trim('/');
            if (trimmed.StartsWith("Assets/", StringComparison.Ordinal))
            {
                return trimmed;
            }

            return string.Concat(kVoxelBustersRoot, "/", trimmed);
        }

        public static List<T> FindAll<T>() where T : PlatformConfigurationObject
        {
            var results = new List<T>();
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset != null)
                {
                    results.Add(asset);
                }
            }
            return results;
        }

        public static List<T> FindAll<T>(string searchFolder) where T : PlatformConfigurationObject
        {
            if (string.IsNullOrEmpty(searchFolder))
            {
                return FindAll<T>();
            }

            var results = new List<T>();
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { searchFolder });
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset != null)
                {
                    results.Add(asset);
                }
            }
            return results;
        }

        public static string GetProductRootFromPath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;
            }

            const string kRootMarker = kVoxelBustersRoot + "/";
            int index = assetPath.IndexOf(kRootMarker, StringComparison.Ordinal);
            if (index < 0)
            {
                return null;
            }

            int start = index + kRootMarker.Length;
            int slashIndex = assetPath.IndexOf('/', start);
            if (slashIndex < 0)
            {
                return assetPath;
            }

            return assetPath.Substring(0, slashIndex);
        }
    }
}
