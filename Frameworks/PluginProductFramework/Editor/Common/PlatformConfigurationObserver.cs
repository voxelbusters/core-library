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

            public bool IsRelated(string productRoot)
            {
                if (Descriptor == null || string.IsNullOrEmpty(productRoot))
                {
                    return false;
                }

                string productRootPath = PluginProductDescriptorUtility.GetProductRootPath(Descriptor);
                return !string.IsNullOrEmpty(productRootPath) &&
                       string.Equals(productRootPath, productRoot, StringComparison.Ordinal);
            }
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
            var affectedProductRootPaths = new HashSet<string>(StringComparer.Ordinal);
            AddProductRoots(importedAssets, affectedProductRootPaths);
            AddProductRoots(movedAssets, affectedProductRootPaths);

            if (affectedProductRootPaths.Count == 0)
            {
                return;
            }

            NotifyRegistrations(affectedProductRootPaths);
        }

        private static void NotifyRegistrations(HashSet<string> productRoots)
        {
            if (productRoots == null || productRoots.Count == 0)
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

                if (IsRegistrationRelatedToAnyProductRoot(registration, productRoots))
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

        private static bool IsRegistrationRelatedToAnyProductRoot(Registration registration, HashSet<string> productRoots)
        {
            foreach (string root in productRoots)
            {
                if (registration.IsRelated(root))
                {
                    return true;
                }
            }

            return false;
        }

        private static void AddProductRoots(string[] paths, HashSet<string> productRoots)
        {
            if (paths == null || productRoots == null)
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

                string productRoot = PlatformConfigurationUtility.GetProductRootFromPath(path);
                if (!string.IsNullOrEmpty(productRoot))
                {
                    productRoots.Add(productRoot);
                }
            }
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
