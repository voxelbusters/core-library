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
    /// Stores feature settings loaded from plugin product settings and exposes typed lookup APIs.
    /// </summary>
    public static class PluginProductFeatureSettingsStore
    {
        private static readonly Dictionary<Type, FeatureSettings> s_featureSettingsByType =
            new Dictionary<Type, FeatureSettings>();

        private static readonly Dictionary<Type, IPluginProductSettingsSummary> s_settingsSummaryByType =
            new Dictionary<Type, IPluginProductSettingsSummary>();

        public static int Count => s_featureSettingsByType.Count;

        public static void RegisterPluginProductSettings(PluginProductSettings settings)
        {
            if (settings == null)
            {
                return;
            }

            IReadOnlyList<FeatureSettings> allFeatureSettings = settings.GetAllFeatureSettings();
            for (int i = 0; i < allFeatureSettings.Count; i++)
            {
                FeatureSettings featureSettings = allFeatureSettings[i];
                if (featureSettings == null)
                {
                    continue;
                }

                Type settingsType = featureSettings.GetType();
                s_featureSettingsByType[settingsType] = featureSettings;
                s_settingsSummaryByType[settingsType] = settings;
            }
        }

        public static bool TryGetFeatureSettingsAndProductSummary<TSettings>(out TSettings featureSettings, out IPluginProductSettingsSummary productSummary)
            where TSettings : FeatureSettings
        {
            Type settingsType = typeof(TSettings);
            if (s_featureSettingsByType.TryGetValue(settingsType, out FeatureSettings rawSettings) &&
                rawSettings is TSettings typedSettings &&
                s_settingsSummaryByType.TryGetValue(settingsType, out IPluginProductSettingsSummary summary))
            {
                featureSettings = typedSettings;
                productSummary = summary;
                return true;
            }

            featureSettings = null;
            productSummary = null;
            return false;
        }

        public static void Clear()
        {
            s_featureSettingsByType.Clear();
            s_settingsSummaryByType.Clear();
        }
    }
}
