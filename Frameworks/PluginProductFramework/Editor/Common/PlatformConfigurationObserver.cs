using System;
using System.Collections.Generic;
using UnityEditor;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework.Editor
{
    /// <summary>
    /// Watches platform configuration asset changes and raises a notification.
    /// </summary>
    internal class PlatformConfigurationObserver : AssetPostprocessor
    {
        private sealed class Registration
        {
            public PluginProductDescriptor Descriptor;
            public Action<PluginProductDescriptor> Callback;
        }

        private static readonly List<Registration> s_registrations = new List<Registration>();

        public static void Register(PluginProductDescriptor descriptor, Action<PluginProductDescriptor> callback)
        {
            if (descriptor == null || callback == null)
            {
                return;
            }

            string productCodeName = descriptor.ProductCodeName;
            if (string.IsNullOrEmpty(productCodeName))
            {
                return;
            }

            RemoveAnyExistingRegistrations(productCodeName);

            s_registrations.Add(new Registration
            {
                Descriptor = descriptor,
                Callback = callback
            });
        }

        public static void Unregister(Action<PluginProductDescriptor> callback)
        {
            if (callback == null)
            {
                return;
            }

            for (int i = s_registrations.Count - 1; i >= 0; i--)
            {
                if (s_registrations[i].Callback == callback)
                {
                    s_registrations.RemoveAt(i);
                }
            }
        }

        private static void OnPostprocessAllAssets(string[] importedAssets,
                                                   string[] deletedAssets,
                                                   string[] movedAssets,
                                                   string[] movedFromAssetPaths)
        {
            var affectedDescriptors = new HashSet<PluginProductDescriptor>();
            AddAffectedDescriptors(importedAssets, affectedDescriptors);
            AddAffectedDescriptors(movedAssets, affectedDescriptors);

            if (affectedDescriptors.Count == 0)
            {
                return;
            }

            NotifyRegistrations(affectedDescriptors);
        }

        private static void NotifyRegistrations(HashSet<PluginProductDescriptor> affectedDescriptors)
        {
            if (affectedDescriptors == null || affectedDescriptors.Count == 0)
            {
                return;
            }

            for (int i = 0; i < s_registrations.Count; i++)
            {
                var registration = s_registrations[i];
                if (registration == null || registration.Callback == null)
                {
                    continue;
                }

                if (registration.Descriptor != null && affectedDescriptors.Contains(registration.Descriptor))
                {
                    registration.Callback.Invoke(registration.Descriptor);
                }
            }
        }

        private static void RemoveAnyExistingRegistrations(string productCodeName)
        {
            for (int i = s_registrations.Count - 1; i >= 0; i--)
            {
                var registration = s_registrations[i];
                if (registration == null)
                {
                    continue;
                }

                if (registration.Descriptor == null)
                {
                    continue;
                }

                if (string.Equals(registration.Descriptor.ProductCodeName, productCodeName, StringComparison.Ordinal))
                {
                    s_registrations.RemoveAt(i);
                }
            }
        }

        private static void AddAffectedDescriptors(string[] paths, HashSet<PluginProductDescriptor> affectedDescriptors)
        {
            if (paths == null || affectedDescriptors == null)
            {
                return;
            }

            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i];
                if (string.IsNullOrEmpty(path) || !path.EndsWith(".asset", System.StringComparison.Ordinal))
                {
                    continue;
                }

                if (!IsPlatformConfigurationAsset(path))
                {
                    continue;
                }

                PluginProductDescriptor descriptor = FindDescriptorForPath(path);
                if (descriptor != null)
                {
                    affectedDescriptors.Add(descriptor);
                }
            }
        }

        private static PluginProductDescriptor FindDescriptorForPath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;
            }

            for (int i = 0; i < s_registrations.Count; i++)
            {
                var registration = s_registrations[i];
                if (registration == null || registration.Descriptor == null)
                {
                    continue;
                }

                string root = GetDescriptorRootPath(registration.Descriptor);
                if (!string.IsNullOrEmpty(root) && IsPathUnderRoot(assetPath, root))
                {
                    return registration.Descriptor;
                }

                string assetsRoot = GetDescriptorAssetsRootPath(registration.Descriptor);
                if (!string.IsNullOrEmpty(assetsRoot) && IsPathUnderRoot(assetPath, assetsRoot))
                {
                    return registration.Descriptor;
                }
            }

            return null;
        }

        private static string GetDescriptorRootPath(PluginProductDescriptor descriptor)
        {
            if (descriptor == null)
            {
                return null;
            }

            string descriptorPath = AssetDatabase.GetAssetPath(descriptor);
            if (string.IsNullOrEmpty(descriptorPath))
            {
                return null;
            }

            string root = System.IO.Path.GetDirectoryName(descriptorPath);
            return string.IsNullOrEmpty(root) ? null : root.Replace('\\', '/');
        }

        private static string GetDescriptorAssetsRootPath(PluginProductDescriptor descriptor)
        {
            return PluginProductDescriptorUtility.GetAssetsProductRootPath(descriptor);
        }

        private static bool IsPathUnderRoot(string assetPath, string root)
        {
            if (string.IsNullOrEmpty(assetPath) || string.IsNullOrEmpty(root))
            {
                return false;
            }

            string normalizedPath = assetPath.Replace('\\', '/');
            string normalizedRoot = root.Replace('\\', '/').TrimEnd('/');
            if (string.Equals(normalizedPath, normalizedRoot, StringComparison.Ordinal))
            {
                return true;
            }

            return normalizedPath.StartsWith(normalizedRoot + "/", StringComparison.Ordinal);
        }

        private static bool IsPlatformConfigurationAsset(string path)
        {
            var assets = AssetDatabase.LoadAllAssetsAtPath(path);
            if (assets == null || assets.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < assets.Length; i++)
            {
                if (assets[i] is PlatformConfigurationObject)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
