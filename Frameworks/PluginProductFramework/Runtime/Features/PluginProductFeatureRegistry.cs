using System;
using System.Collections.Generic;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Provides a bootstrap hook for feature registration without hard references.
    /// </summary>
    public interface IPluginProductSettingsSummary
    {
        /// <summary>
        /// Gets the product descriptor.
        /// </summary>
        PluginProductDescriptor Descriptor { get; }
    }

    /// <summary>
    /// Provides a bootstrap hook for feature registration without hard references.
    /// </summary>
    public interface IPluginProductFeatureBootstrap<TSettings>
        where TSettings : FeatureSettings
    {
        /// <summary>
        /// Registers the feature with the product using provided settings.
        /// </summary>
        void Register(IPluginProductSettingsSummary settingsSummary, TSettings featureSettings);
    }

    /// <summary>
    /// Registry for feature bootstraps.
    /// </summary>
    public static class PluginProductFeatureRegistry
    {
        private interface IFeatureBootstrapEntry
        {
            Type SettingsType { get; }
            void Register(IPluginProductSettingsSummary settingsSummary, FeatureSettings featureSettings);
        }

        private sealed class FeatureBootstrapEntry<TSettings> : IFeatureBootstrapEntry
            where TSettings : FeatureSettings
        {
            private readonly IPluginProductFeatureBootstrap<TSettings> m_bootstrap;

            public FeatureBootstrapEntry(IPluginProductFeatureBootstrap<TSettings> bootstrap)
            {
                m_bootstrap = bootstrap;
            }

            public Type SettingsType => typeof(TSettings);

            public void Register(IPluginProductSettingsSummary settingsSummary, FeatureSettings featureSettings)
            {
                if (featureSettings is TSettings typedSettings)
                {
                    m_bootstrap.Register(settingsSummary, typedSettings);
                }
            }
        }

        private static readonly List<IFeatureBootstrapEntry> s_bootstraps =
            new List<IFeatureBootstrapEntry>();

        public static int Count => s_bootstraps.Count;

        public static void Register<TSettings>(IPluginProductFeatureBootstrap<TSettings> bootstrap)
            where TSettings : FeatureSettings
        {
            if (bootstrap == null)
            {
                return;
            }

            for (int i = s_bootstraps.Count - 1; i >= 0; i--)
            {
                if (s_bootstraps[i].SettingsType == typeof(TSettings))
                {
                    s_bootstraps.RemoveAt(i);
                }
            }

            s_bootstraps.Add(new FeatureBootstrapEntry<TSettings>(bootstrap));
        }

        public static void Apply(PluginProductSettings settings)
        {
            if (settings == null)
            {
                return;
            }

            for (int i = 0; i < s_bootstraps.Count; i++)
            {
                IFeatureBootstrapEntry entry = s_bootstraps[i];
                if (entry == null)
                {
                    continue;
                }

                FeatureSettings featureSettings = settings.GetFeatureSettings(entry.SettingsType);
                entry.Register(settings, featureSettings);
            }
        }

        public static void Clear()
        {
            s_bootstraps.Clear();
        }
    }
}
