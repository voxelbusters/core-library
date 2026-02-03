#if UNITY_IOS || UNITY_TVOS
using System.Collections.Generic;
using UnityEditor;
using VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework.Editor
{
    /// <summary>
    /// Applies iOS plist, entitlements, and framework updates at build time.
    /// </summary>
    internal sealed class IosPlatformPostBuildProcessor : IPostprocessBuildWithReport
    {
        int IOrderedCallback.callbackOrder => 500;

        void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
        {
            if (report == null)
            {
                return;
            }

            ProcessAll(report.summary.platform, report.summary.outputPath);
        }

        internal static void ProcessAll(BuildTarget target, string pathToBuiltProject)
        {
            if (target != BuildTarget.iOS && target != BuildTarget.tvOS)
            {
                return;
            }

            var configs = new List<IosPlatformConfiguration>();
            var descriptors = PluginProductDescriptorUtility.FindAllPluginProductDescriptors();
            PluginProductSettingsUtility.LogMissingSettingsWarnings(descriptors, nameof(IosPlatformPostBuildProcessor));
            for (int i = 0; i < descriptors.Count; i++)
            {
                var descriptor = descriptors[i];
                if (descriptor == null)
                {
                    continue;
                }

                string assetsRootPath = PluginProductDescriptorUtility.GetAssetsProductRootPath(descriptor);
                if (string.IsNullOrEmpty(assetsRootPath))
                {
                    continue;
                }

                string templateRootPath = PluginProductDescriptorUtility.GetProductRootPath(descriptor);
                configs.AddRange(PlatformConfigurationUtility.GetOrCreateEditableConfigs<IosPlatformConfiguration>(
                    templateRootPath,
                    assetsRootPath));
            }

            if (configs.Count == 0)
            {
                return;
            }

            IosInfoPlistUpdater.Update(pathToBuiltProject, configs);
            IosEntitlementsUpdater.Update(pathToBuiltProject, configs);
            IosCapabilitiesUpdater.Update(pathToBuiltProject, configs);
            IosProjectBuildSettingsUpdater.Update(pathToBuiltProject, configs);
        }
    }
}
#endif
