using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using VoxelBusters.CoreLibrary.Editor;
using VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework.Editor
{
    internal static class FeatureActivationUtility
    {
        public static void UpdateImporters(PluginProductDescriptor descriptor)
        {
            UpdateImporters(descriptor, null);
        }

        public static void UpdateImporters(PluginProductDescriptor descriptor, FeatureSettings featureSettings)
        {
            if (descriptor == null)
            {
                return;
            }

            string productRootPath = PluginProductDescriptorUtility.GetProductRootPath(descriptor);
            if (string.IsNullOrEmpty(productRootPath))
            {
                return;
            }

            var updatedAssets = new List<string>();
            if (featureSettings != null)
            {
                UpdateImportersForFeature(productRootPath, featureSettings, updatedAssets);
            }
            else
            {
                var settings = PluginProductSettingsUtility.TryLoadProductSettings(descriptor);
                if (settings == null)
                {
                    return;
                }

                var features = PluginProductSettingsUtility.GetFeatureSettings(settings);
                if (features.Count == 0)
                {
                    return;
                }

                for (int i = 0; i < features.Count; i++)
                {
                    UpdateImportersForFeature(productRootPath, features[i], updatedAssets);
                }
            }

            if (updatedAssets.Count > 0)
            {
                DebugLogger.Log(CoreLibraryDomain.Default,
                    $"[FeatureActivationUtility] Updated plugin importers:\n{string.Join("\n", updatedAssets)}");
            }
        }

        public static string GetFeatureNativePluginsRootPath(string productRootPath, string featureName)
        {
            if (string.IsNullOrEmpty(productRootPath) || string.IsNullOrEmpty(featureName))
            {
                return null;
            }

            return CombineAssetPath(productRootPath, "Features", featureName, "Plugins");
        }

        /// <summary>
        /// We need to disable the asmdefs supported platforms as well as any native files for the related feature.
        /// </summary>
        /// <param name="productRootPath"></param>
        /// <param name="feature"></param>
        /// <param name="updatedAssets"></param>
        private static void UpdateImportersForFeature(string productRootPath,
                                                      FeatureSettings feature,
                                                      List<string> updatedAssets)
        {
            if (feature == null)
            {
                return;
            }

            string featureCodeName = feature?.FeatureDescriptor?.FeatureCodeName;
            if (string.IsNullOrEmpty(featureCodeName))
            {
                return;
            }

            string featureRoot = CombineAssetPath(productRootPath, "Features", featureCodeName);
            UpdateAssemblyDefinitionsForFeature(featureRoot, feature.IsEnabled, updatedAssets);

            string pluginsRoot = GetFeatureNativePluginsRootPath(productRootPath, featureCodeName);
            if (string.IsNullOrEmpty(pluginsRoot))
            {
                return;
            }

            UpdateImportersForPlatform(CombineAssetPath(pluginsRoot, "iOS"), new[] { BuildTarget.iOS, BuildTarget.tvOS }, feature.IsEnabled, updatedAssets);
            UpdateImportersForPlatform(CombineAssetPath(pluginsRoot, "Android"), new[] { BuildTarget.Android }, feature.IsEnabled, updatedAssets);
        }

        private static void UpdateImportersForPlatform(string folderPath,
                                                       BuildTarget[] platforms,
                                                       bool isEnabled,
                                                       List<string> updatedAssets)
        {
            if (string.IsNullOrEmpty(folderPath) || !AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            if (platforms == null || platforms.Length == 0)
            {
                return;
            }

            string[] guids = AssetDatabase.FindAssets(string.Empty, new[] { folderPath });
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                var importer = AssetImporter.GetAtPath(assetPath) as PluginImporter;
                if (importer == null)
                {
                    continue;
                }

                bool changed = false;
                if (importer.GetCompatibleWithAnyPlatform())
                {
                    importer.SetCompatibleWithAnyPlatform(false);
                    changed = true;
                }

                if (importer.GetCompatibleWithEditor())
                {
                    importer.SetCompatibleWithEditor(false);
                    changed = true;
                }

                for (int platformIndex = 0; platformIndex < platforms.Length; platformIndex++)
                {
                    BuildTarget platform = platforms[platformIndex];
                    if (importer.GetCompatibleWithPlatform(platform) != isEnabled)
                    {
                        importer.SetCompatibleWithPlatform(platform, isEnabled);
                        changed = true;
                    }
                }

                if (changed)
                {
                    importer.SaveAndReimport();
                    updatedAssets?.Add(assetPath);
                }
            }
        }

        private static void UpdateAssemblyDefinitionsForFeature(string featureRootPath, bool isEnabled, List<string> updatedAssets)
        {
            if (string.IsNullOrEmpty(featureRootPath) || !AssetDatabase.IsValidFolder(featureRootPath))
            {
                return;
            }

            string[] guids = AssetDatabase.FindAssets("t:AssemblyDefinitionAsset", new[] { featureRootPath });
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (IsEditorAssemblyDefinition(assetPath))
                {
                    continue;
                }

                if (TryUpdateAssemblyDefinition(assetPath, isEnabled))
                {
                    updatedAssets?.Add(assetPath);
                }
            }
        }

        private static bool IsEditorAssemblyDefinition(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return false;
            }

            string normalizedPath = assetPath.Replace('\\', '/');
            if (normalizedPath.IndexOf("/Editor/", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            return normalizedPath.EndsWith(".Editor.asmdef", StringComparison.OrdinalIgnoreCase);
        }

        private static bool TryUpdateAssemblyDefinition(string asmdefPath, bool isEnabled)
        {
            if (string.IsNullOrEmpty(asmdefPath) || !File.Exists(asmdefPath))
            {
                return false;
            }

            string directory = Path.GetDirectoryName(asmdefPath);
            if (string.IsNullOrEmpty(directory))
            {
                return false;
            }

            string[] asmdefFiles = Directory.GetFiles(directory, "*.asmdef");
            if (asmdefFiles.Length != 1)
            {
                DebugLogger.LogWarning(CoreLibraryDomain.Default,
                    $"[FeatureActivationUtility] Skipped asmdef update due to multiple asmdefs in '{directory}'.");
                return false;
            }

            var proxy = new AssemblyDefinitionProxy(directory);
            if (isEnabled)
            {
                proxy.IncludeAllPlatforms();
            }
            else
            {
                proxy.ExcludeAllPlatforms();
            }

            proxy.Save();
            AssetDatabase.ImportAsset(asmdefPath);
            return true;
        }

        private static string CombineAssetPath(params string[] parts)
        {
            return Path.Combine(parts).Replace('\\', '/');
        }
    }
}
