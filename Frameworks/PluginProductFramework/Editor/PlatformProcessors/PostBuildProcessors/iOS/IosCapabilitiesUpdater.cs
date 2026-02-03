#if UNITY_IOS || UNITY_TVOS
using System.Collections.Generic;
using UnityEditor.iOS.Xcode;
using UnityEngine;
using VoxelBusters.CoreLibrary.Editor.NativePlugins.Build.Xcode;
using VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework.Editor
{
    internal static class IosCapabilitiesUpdater
    {
        internal static void Update(string buildPath, List<IosPlatformConfiguration> configs)
        {
            string pbxPath = PBXProject.GetPBXProjectPath(buildPath);
            var project = new PBXProject();
            project.ReadFromFile(pbxPath);

            string mainTargetGuid = IosPostBuildProjectUtility.GetMainTargetGuid(project);
            string entitlementsRelPath = project.GetBuildPropertyForAnyConfig(mainTargetGuid, IosPostBuildConstants.CodeSignEntitlementsKey);
            string entitlementsPath = IosEntitlementsUpdater.ResolveEntitlementsPath(project, pbxPath, buildPath, mainTargetGuid, entitlementsRelPath, out string resolvedRelPath);
            if (string.IsNullOrEmpty(entitlementsPath))
            {
                return;
            }

            string capabilityEntitlementsPath = string.IsNullOrEmpty(resolvedRelPath)
                ? GetRelativePath(buildPath, entitlementsPath)
                : resolvedRelPath;
            if (string.IsNullOrEmpty(capabilityEntitlementsPath))
            {
                return;
            }

            var capabilityManager = new ProjectCapabilityManager(
                pbxPath,
                capabilityEntitlementsPath,
                IosPostBuildProjectUtility.GetMainTargetName(project),
                mainTargetGuid);

            var capabilityTypes = new HashSet<IosCapabilityType>();
            var associatedDomains = new HashSet<string>(System.StringComparer.Ordinal);

            for (int i = 0; i < configs.Count; i++)
            {
                var config = configs[i];
                if (config == null)
                {
                    continue;
                }

                var capabilities = config.Capabilities;
                if (capabilities != null)
                {
                    for (int j = 0; j < capabilities.Length; j++)
                    {
                        var capability = capabilities[j];
                        if (capability == null)
                        {
                            continue;
                        }

                        capabilityTypes.Add(capability.Type);
                        if (capability.Type == IosCapabilityType.AssociatedDomains)
                        {
                            AppendAssociatedDomains(capability.AssociatedDomains, associatedDomains);
                        }
                    }
                }

                AppendAssociatedDomains(config.AssociatedDomains, associatedDomains);
            }

            if (associatedDomains.Count > 0)
            {
                capabilityTypes.Add(IosCapabilityType.AssociatedDomains);
            }

            foreach (var capabilityType in capabilityTypes)
            {
                switch (capabilityType)
                {
                    case IosCapabilityType.GameCenter:
                        capabilityManager.AddGameCenter();
                        break;
                    case IosCapabilityType.iCloud:
                        capabilityManager.AddiCloud(enableKeyValueStorage: true, enableiCloudDocument: false, enablecloudKit: false, addDefaultContainers: false, customContainers: null);
                        break;
                    case IosCapabilityType.InAppPurchase:
                        capabilityManager.AddInAppPurchase();
                        break;
                    case IosCapabilityType.PushNotifications:
                        capabilityManager.AddPushNotifications(Debug.isDebugBuild);
                        capabilityManager.AddBackgroundModes(BackgroundModesOptions.RemoteNotifications);
                        break;
                    case IosCapabilityType.AssociatedDomains:
                        if (associatedDomains.Count > 0)
                        {
                            capabilityManager.AddAssociatedDomains(domains: new List<string>(associatedDomains).ToArray());
                        }
                        break;
                }
            }

            capabilityManager.WriteToFile();
        }

        private static void AppendAssociatedDomains(IosAssociatedDomain[] domains, HashSet<string> results)
        {
            if (domains == null || results == null)
            {
                return;
            }

            for (int i = 0; i < domains.Length; i++)
            {
                var domain = domains[i];
                if (domain == null || string.IsNullOrEmpty(domain.Host))
                {
                    continue;
                }

                string serviceType = string.IsNullOrEmpty(domain.ServiceType)
                    ? IosPostBuildConstants.DefaultAssociatedDomainsServiceType
                    : domain.ServiceType;
                results.Add(serviceType + ":" + domain.Host);
            }
        }

        private static string GetRelativePath(string rootPath, string fullPath)
        {
            if (string.IsNullOrEmpty(rootPath) || string.IsNullOrEmpty(fullPath))
            {
                return null;
            }

            string normalizedRoot = rootPath.Replace('\\', '/').TrimEnd('/') + "/";
            string normalizedFull = fullPath.Replace('\\', '/');
            if (!normalizedFull.StartsWith(normalizedRoot, System.StringComparison.Ordinal))
            {
                return null;
            }

            return normalizedFull.Substring(normalizedRoot.Length);
        }

    }
}
#endif