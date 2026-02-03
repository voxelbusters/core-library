#if UNITY_IOS || UNITY_TVOS
using System.Collections.Generic;
using System.IO;
using UnityEditor.iOS.Xcode;
using VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework.Editor
{
    internal static class IosEntitlementsUpdater
    {
        internal static void Update(string buildPath, List<IosPlatformConfiguration> configs)
        {
            string pbxPath = PBXProject.GetPBXProjectPath(buildPath);
            var project = new PBXProject();
            project.ReadFromFile(pbxPath);

            string mainTargetGuid = IosPostBuildProjectUtility.GetMainTargetGuid(project);
            string entitlementsRelPath = project.GetBuildPropertyForAnyConfig(mainTargetGuid, IosPostBuildConstants.CodeSignEntitlementsKey);
            string entitlementsPath = ResolveEntitlementsPath(project, pbxPath, buildPath, mainTargetGuid, entitlementsRelPath, out _);
            if (string.IsNullOrEmpty(entitlementsPath))
            {
                return;
            }

            var entitlements = new PlistDocument();
            if (File.Exists(entitlementsPath))
            {
                entitlements.ReadFromFile(entitlementsPath);
            }

            var root = entitlements.root;
            foreach (var config in configs)
            {
                var entries = config.EntitlementEntries;
                if (entries != null)
                {
                    foreach (var entry in entries)
                    {
                        if (entry == null || string.IsNullOrEmpty(entry.Key))
                        {
                            continue;
                        }

                        root.SetString(entry.Key, entry.Value ?? string.Empty);
                    }
                }

                var domains = config.AssociatedDomains;
                if (domains != null && domains.Length > 0)
                {
                    var domainsArray = root.CreateArray("com.apple.developer.associated-domains");
                    foreach (var domain in domains)
                    {
                        if (domain == null || string.IsNullOrEmpty(domain.Host))
                        {
                            continue;
                        }

                        string serviceType = string.IsNullOrEmpty(domain.ServiceType)
                            ? IosPostBuildConstants.DefaultAssociatedDomainsServiceType
                            : domain.ServiceType;
                        domainsArray.AddString(serviceType + ":" + domain.Host);
                    }
                }
            }

            entitlements.WriteToFile(entitlementsPath);
        }

        internal static string ResolveEntitlementsPath(PBXProject project,
                                                      string pbxPath,
                                                      string buildPath,
                                                      string mainTargetGuid,
                                                      string entitlementsRelPath,
                                                      out string resolvedRelativePath)
        {
            resolvedRelativePath = entitlementsRelPath;
            if (!string.IsNullOrEmpty(entitlementsRelPath))
            {
                string configuredPath = Path.Combine(buildPath, entitlementsRelPath);
                if (File.Exists(configuredPath))
                {
                    return configuredPath;
                }

                EnsureEntitlementsFileExists(configuredPath);
                return configuredPath;
            }

            if (TryFindExistingEntitlements(buildPath, out string existingRelPath, out string existingPath))
            {
                resolvedRelativePath = existingRelPath;
                project.SetBuildProperty(mainTargetGuid, IosPostBuildConstants.CodeSignEntitlementsKey, existingRelPath);
                project.WriteToFile(pbxPath);
                return existingPath;
            }

            string defaultRelPath = IosPostBuildConstants.DefaultEntitlementsFileName;
            string defaultPath = Path.Combine(buildPath, defaultRelPath);
            EnsureEntitlementsFileExists(defaultPath);
            project.SetBuildProperty(mainTargetGuid, IosPostBuildConstants.CodeSignEntitlementsKey, defaultRelPath);
            project.WriteToFile(pbxPath);
            resolvedRelativePath = defaultRelPath;
            return defaultPath;
        }

        private static bool TryFindExistingEntitlements(string buildPath, out string relativePath, out string fullPath)
        {
            string defaultPath = Path.Combine(buildPath, IosPostBuildConstants.DefaultEntitlementsFileName);
            if (File.Exists(defaultPath))
            {
                relativePath = IosPostBuildConstants.DefaultEntitlementsFileName;
                fullPath = defaultPath;
                return true;
            }

            string[] entitlements = Directory.GetFiles(buildPath, "*.entitlements", SearchOption.AllDirectories);
            if (entitlements.Length == 0)
            {
                relativePath = null;
                fullPath = null;
                return false;
            }

            fullPath = entitlements[0];
            relativePath = GetRelativePath(buildPath, fullPath);
            return !string.IsNullOrEmpty(relativePath);
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

        private static void EnsureEntitlementsFileExists(string path)
        {
            if (File.Exists(path))
            {
                return;
            }

            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var plist = new PlistDocument();
            plist.WriteToFile(path);
        }
    }
}
#endif