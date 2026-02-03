using System;
using System.Collections.Generic;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework.Editor
{
    internal static class FeatureSettingsObserver
    {
        private sealed class Registration
        {
            public PluginProductDescriptor Descriptor;
            public Action<PluginProductDescriptor, FeatureSettings> Callback;
        }

        private static readonly List<Registration> s_registrations = new List<Registration>();

        public static void Register(PluginProductDescriptor descriptor,
                                    Action<PluginProductDescriptor, FeatureSettings> callback)
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

        public static void Unregister(Action<PluginProductDescriptor, FeatureSettings> callback)
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

        public static void NotifyFeatureChanged(PluginProductDescriptor descriptor, FeatureSettings featureSettings)
        {
            if (descriptor == null || featureSettings == null)
            {
                return;
            }

            string productCodeName = descriptor.ProductCodeName;
            if (string.IsNullOrEmpty(productCodeName))
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

                if (registration.Descriptor == null)
                {
                    continue;
                }

                if (!string.Equals(registration.Descriptor.ProductCodeName, productCodeName, StringComparison.Ordinal))
                {
                    continue;
                }

                registration.Callback.Invoke(descriptor, featureSettings);
            }
        }
    }
}
