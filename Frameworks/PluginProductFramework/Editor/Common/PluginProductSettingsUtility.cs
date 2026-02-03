using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework.Editor
{
    public static class PluginProductSettingsUtility
    {
        public static TSettings LoadOrCreateProductSettings<TSettings>(PluginProductDescriptor descriptor)
            where TSettings : PluginProductSettings
        {
            if (descriptor == null)
            {
                DebugLogger.LogWarning(CoreLibraryDomain.Default,
                    "[PluginProductSettingsUtility] Descriptor is null; cannot load settings.");
                return null;
            }

            string assetPath = ResolveSettingsAssetPath(descriptor);
            if (string.IsNullOrEmpty(assetPath))
            {
                DebugLogger.LogWarning(CoreLibraryDomain.Default,
                    $"[PluginProductSettingsUtility] Invalid settings asset path for '{descriptor.ProductCodeName}'.");
                return null;
            }

            var settings = AssetDatabase.LoadAssetAtPath<TSettings>(assetPath);
            if (settings == null)
            {
                settings = CreateSettingsAsset<TSettings>(descriptor, assetPath);
            }

            if (settings != null)
            {
                EnsureDescriptorLink(settings, descriptor);
                EnsureGeneralAndResourcesSettings(settings, descriptor);
                EnsureFeatureSettings(descriptor, settings);
            }

            return settings;
        }

        public static PluginProductSettings TryLoadProductSettings(PluginProductDescriptor descriptor)
        {
            if (descriptor == null)
            {
                return null;
            }

            string assetPath = ResolveSettingsAssetPath(descriptor);
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;
            }

            var settings = AssetDatabase.LoadAssetAtPath<PluginProductSettings>(assetPath);
            if (settings != null)
            {
                EnsureDescriptorLink(settings, descriptor);
            }

            return settings;
        }

        public static List<FeatureSettings> GetFeatureSettings(PluginProductSettings settings)
        {
            if (settings == null)
            {
                return new List<FeatureSettings>();
            }

            string assetPath = AssetDatabase.GetAssetPath(settings);
            var assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            var results = new List<FeatureSettings>();
            for (int i = 0; i < assets.Length; i++)
            {
                if (assets[i] is FeatureSettings featureSettings)
                {
                    results.Add(featureSettings);
                }
            }

            results.Sort((a, b) =>
            {
                string nameA = a?.FeatureDescriptor?.FeatureCodeName;
                string nameB = b?.FeatureDescriptor?.FeatureCodeName;
                return string.CompareOrdinal(nameA, nameB);
            });
            return results;
        }

        public static void EnsureFeatureSettings(PluginProductDescriptor descriptor, PluginProductSettings settings)
        {
            if (descriptor == null || settings == null)
            {
                return;
            }

            if (!TryGetProductRootPath(descriptor, out string productRootPath))
            {
                return;
            }

            List<FeatureDescriptor> featureDescriptors = GetSortedFeatureDescriptors(productRootPath);

            string settingsAssetPath = AssetDatabase.GetAssetPath(settings);
            if (string.IsNullOrEmpty(settingsAssetPath))
            {
                return;
            }

            var validFeatureCodes = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < featureDescriptors.Count; i++)
            {
                string featureCode = featureDescriptors[i]?.FeatureCodeName;
                if (!string.IsNullOrEmpty(featureCode))
                {
                    validFeatureCodes.Add(featureCode);
                }
            }

            bool removed = RemoveObsoleteFeatureSettings(settings, settingsAssetPath, validFeatureCodes);
            var existing = BuildExistingFeatureLookup(settings);
            List<FeatureSettings> orderedFeatures = BuildOrderedFeatures(featureDescriptors, existing, settingsAssetPath);

            settings.SetFeatures(orderedFeatures);
            if (removed)
            {
                EditorUtility.SetDirty(settings);
            }

            AssetDatabase.SaveAssets();
        }

        public static void CleanupMissingFeatures(string productRootPath)
        {
            if (string.IsNullOrEmpty(productRootPath))
            {
                return;
            }

            PluginProductDescriptor descriptor = FindDescriptorInProductRoot(productRootPath);
            if (descriptor == null)
            {
                return;
            }

            PluginProductSettings settings = TryLoadProductSettings(descriptor);
            if (settings == null)
            {
                return;
            }

            EnsureFeatureSettings(descriptor, settings);
        }

        public static void CleanupMissingFeatures(PluginProductDescriptor descriptor)
        {
            if (descriptor == null)
            {
                return;
            }

            PluginProductSettings settings = TryLoadProductSettings(descriptor);
            if (settings == null)
            {
                return;
            }

            EnsureFeatureSettings(descriptor, settings);
        }

        public static void LogMissingSettingsWarnings(IEnumerable<PluginProductDescriptor> descriptors, string context)
        {
            if (descriptors == null)
            {
                return;
            }

            foreach (PluginProductDescriptor descriptor in descriptors)
            {
                if (descriptor == null)
                {
                    continue;
                }

                if (TryLoadProductSettings(descriptor) != null)
                {
                    continue;
                }

                string displayName = string.IsNullOrEmpty(descriptor.ProductDisplayName)
                    ? descriptor.ProductCodeName
                    : descriptor.ProductDisplayName;
                string contextPrefix = string.IsNullOrEmpty(context) ? "PluginProductSettings" : context;
                DebugLogger.LogWarning(CoreLibraryDomain.Default,
                    $"[{contextPrefix}] Settings not found for '{displayName}'. Use Window → Voxel Busters → {displayName} → Open Settings to create and configure them.");
            }
        }

        private static string ResolveSettingsAssetPath(PluginProductDescriptor descriptor)
        {
            if (descriptor == null)
            {
                DebugLogger.LogWarning(CoreLibraryDomain.Default,
                    "[PluginProductSettingsUtility] Cannot resolve settings path; descriptor is null.");
                return null;
            }

            string settingsAssetPath = PluginProductDescriptorUtility.NormalizeAssetPath(descriptor.SettingsAssetPath);
            if (!string.IsNullOrEmpty(settingsAssetPath) &&
                settingsAssetPath.StartsWith("Assets/Resources/", StringComparison.Ordinal))
            {
                return settingsAssetPath;
            }

            string productName = string.IsNullOrEmpty(descriptor.ProductCodeName) ? descriptor.name : descriptor.ProductCodeName;
            if (string.IsNullOrEmpty(productName))
            {
                DebugLogger.LogWarning(CoreLibraryDomain.Default,
                    "[PluginProductSettingsUtility] Cannot resolve settings path; product code name is empty.");
                return null;
            }

            settingsAssetPath = $"Assets/Resources/{productName}Settings.asset";
            return settingsAssetPath;
        }

        private static TSettings CreateSettingsAsset<TSettings>(PluginProductDescriptor descriptor, string assetPath)
            where TSettings : PluginProductSettings
        {
            string directory = Path.GetDirectoryName(assetPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var settings = ScriptableObject.CreateInstance<TSettings>();
            if (settings == null)
            {
                DebugLogger.LogError(CoreLibraryDomain.Default,
                    $"[PluginProductSettingsUtility] Failed to create settings instance for '{descriptor.ProductCodeName}'.");
                return null;
            }

            AssetDatabase.CreateAsset(settings, assetPath);
            AssetDatabase.SaveAssets();
            return settings;
        }

        private static void EnsureDescriptorLink(PluginProductSettings settings,
                                                 PluginProductDescriptor descriptor)
        {
            if (settings == null)
            {
                return;
            }

            if (settings.Descriptor != descriptor)
            {
                settings.SetDescriptor(descriptor);
                EditorUtility.SetDirty(settings);
            }
        }

        private static void EnsureGeneralAndResourcesSettings(PluginProductSettings settings,
                                                              PluginProductDescriptor descriptor)
        {
            if (settings == null)
            {
                return;
            }

            bool changed = false;
            if (settings.GeneralSettings == null)
            {
                var general = CreateGeneralSettingsInstance(settings, descriptor);
                if (general != null)
                {
                    general.name = $"{settings.GetType().Name}_GeneralSettings";
                    AssetDatabase.AddObjectToAsset(general, settings);
                    settings.SetGeneralSettings(general);
                    changed = true;
                }
            }

            if (settings.ResourcesSettings == null)
            {
                var resources = CreateResourcesSettingsInstance(settings, descriptor);
                if (resources != null)
                {
                    resources.name = $"{settings.GetType().Name}_ResourcesSettings";
                    AssetDatabase.AddObjectToAsset(resources, settings);
                    settings.SetResourcesSettings(resources);
                    changed = true;
                }
            }

            if (changed)
            {
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
            }
        }

        private static PluginProductGeneralSettingsBase CreateGeneralSettingsInstance(PluginProductSettings settings,
                                                                                      PluginProductDescriptor descriptor)
        {
            if (descriptor != null && descriptor.GeneralSettingsTemplate != null)
            {
                return UnityEngine.Object.Instantiate(descriptor.GeneralSettingsTemplate);
            }

            return settings.CreateGeneralSettings();
        }

        private static PluginProductResourcesSettingsBase CreateResourcesSettingsInstance(PluginProductSettings settings,
                                                                                          PluginProductDescriptor descriptor)
        {
            if (descriptor != null && descriptor.ResourcesSettingsTemplate != null)
            {
                return UnityEngine.Object.Instantiate(descriptor.ResourcesSettingsTemplate);
            }

            return settings.CreateResourcesSettings();
        }


        private static List<FeatureDescriptor> FindFeatureDescriptors(string productRootPath)
        {
            var results = new List<FeatureDescriptor>();
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(FeatureDescriptor)}", new[] { productRootPath });
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = AssetDatabase.LoadAssetAtPath<FeatureDescriptor>(path);
                if (asset != null)
                {
                    results.Add(asset);
                }
            }
            return results;
        }

        private static PluginProductDescriptor FindDescriptorInProductRoot(string productRootPath)
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(PluginProductDescriptor)}",
                new[] { productRootPath });
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = AssetDatabase.LoadAssetAtPath<PluginProductDescriptor>(path);
                if (asset != null)
                {
                    return asset;
                }
            }

            return null;
        }

        private static bool TryGetProductRootPath(PluginProductDescriptor descriptor, out string productRootPath)
        {
            productRootPath = PluginProductDescriptorUtility.GetProductRootPath(descriptor);
            return !string.IsNullOrEmpty(productRootPath);
        }

        private static List<FeatureDescriptor> GetSortedFeatureDescriptors(string productRootPath)
        {
            var descriptors = FindFeatureDescriptors(productRootPath);
            descriptors.Sort((a, b) => string.CompareOrdinal(a?.FeatureCodeName, b?.FeatureCodeName));
            return descriptors;
        }

        private static Dictionary<string, FeatureSettings> BuildExistingFeatureLookup(PluginProductSettings settings)
        {
            var existing = new Dictionary<string, FeatureSettings>(StringComparer.Ordinal);
            foreach (var feature in GetFeatureSettings(settings))
            {
                string featureCodeName = feature?.FeatureDescriptor?.FeatureCodeName;
                if (string.IsNullOrEmpty(featureCodeName))
                {
                    continue;
                }

                if (!existing.ContainsKey(featureCodeName))
                {
                    existing.Add(featureCodeName, feature);
                }
            }

            return existing;
        }

        private static bool RemoveObsoleteFeatureSettings(PluginProductSettings settings,
                                                         string settingsAssetPath,
                                                         HashSet<string> validFeatureCodes)
        {
            bool removed = false;
            var seenFeatureCodes = new HashSet<string>(StringComparer.Ordinal);
            List<FeatureSettings> features = GetFeatureSettings(settings);
            for (int i = 0; i < features.Count; i++)
            {
                FeatureSettings feature = features[i];
                if (feature == null)
                {
                    continue;
                }

                string featureCodeName = feature.FeatureDescriptor != null
                    ? feature.FeatureDescriptor.FeatureCodeName
                    : null;

                if (string.IsNullOrEmpty(featureCodeName) || !validFeatureCodes.Contains(featureCodeName))
                {
                    if (AssetDatabase.GetAssetPath(feature) == settingsAssetPath)
                    {
                        AssetDatabase.RemoveObjectFromAsset(feature);
                        UnityEngine.Object.DestroyImmediate(feature, true);
                        removed = true;
                    }
                    continue;
                }

                if (!seenFeatureCodes.Add(featureCodeName))
                {
                    if (AssetDatabase.GetAssetPath(feature) == settingsAssetPath)
                    {
                        AssetDatabase.RemoveObjectFromAsset(feature);
                        UnityEngine.Object.DestroyImmediate(feature, true);
                        removed = true;
                    }
                }
            }

            return removed;
        }

        private static List<FeatureSettings> BuildOrderedFeatures(List<FeatureDescriptor> descriptors,
                                                                  Dictionary<string, FeatureSettings> existing,
                                                                  string settingsAssetPath)
        {
            var orderedFeatures = new List<FeatureSettings>();
            for (int i = 0; i < descriptors.Count; i++)
            {
                var descriptorAsset = descriptors[i];
                if (descriptorAsset == null || string.IsNullOrEmpty(descriptorAsset.FeatureCodeName))
                {
                    continue;
                }

                if (!existing.TryGetValue(descriptorAsset.FeatureCodeName, out FeatureSettings featureSettings))
                {
                    featureSettings = descriptorAsset.CreateSettingsInstance();
                    if (featureSettings == null)
                    {
                        continue;
                    }

                    featureSettings.name = $"{descriptorAsset.FeatureCodeName}Settings";
                    featureSettings.Initialize(descriptorAsset);
                    AssetDatabase.AddObjectToAsset(featureSettings, settingsAssetPath);
                }
                else if (featureSettings.FeatureDescriptor != descriptorAsset)
                {
                    featureSettings.Initialize(descriptorAsset);
                    EditorUtility.SetDirty(featureSettings);
                }

                orderedFeatures.Add(featureSettings);
            }

            return orderedFeatures;
        }
    }
}
