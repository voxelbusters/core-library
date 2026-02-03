using System.Collections.Generic;
#if UNITY_IOS || UNITY_TVOS
using UnityEditor.iOS.Xcode;
using VoxelBusters.CoreLibrary.Editor.NativePlugins.Build.Xcode;
using VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework.Editor
{
    internal static class IosProjectBuildSettingsUpdater
    {
        internal static void Update(string buildPath, List<IosPlatformConfiguration> configs)
        {
            string pbxPath = PBXProject.GetPBXProjectPath(buildPath);
            var project = new PBXProject();
            project.ReadFromFile(pbxPath);

            string mainTargetGuid = IosPostBuildProjectUtility.GetMainTargetGuid(project);
            string frameworkTargetGuid = project.GetUnityFrameworkTargetGuid();

            var macroDefinitions = new HashSet<string>(System.StringComparer.Ordinal);
            foreach (var config in configs)
            {
                var frameworks = config.Frameworks;
                if (frameworks == null)
                {
                    continue;
                }

                foreach (var framework in frameworks)
                {
                    if (framework == null || string.IsNullOrEmpty(framework.Name))
                    {
                        continue;
                    }

                    project.AddFrameworkToProject(mainTargetGuid, framework.Name, framework.IsWeak);
                    if (mainTargetGuid != frameworkTargetGuid)
                    {
                        project.AddFrameworkToProject(frameworkTargetGuid, framework.Name, framework.IsWeak);
                    }
                }

                ApplyBuildProperties(project, mainTargetGuid, frameworkTargetGuid, config.BuildProperties);
                ApplySearchPaths(project, mainTargetGuid, frameworkTargetGuid, config.HeaderSearchPaths,
                    (target, path) => project.AddHeaderSearchPath(target, path));
                ApplySearchPaths(project, mainTargetGuid, frameworkTargetGuid, config.FrameworkSearchPaths,
                    (target, path) => project.AddFrameworkSearchPath(target, path));
                ApplySearchPaths(project, mainTargetGuid, frameworkTargetGuid, config.LibrarySearchPaths,
                    (target, path) => project.AddLibrarySearchPath(target, path));

                CollectMacroDefinitions(config.MacroDefinitions, macroDefinitions);
            }

            ApplyMacroDefinitions(project, mainTargetGuid, frameworkTargetGuid, macroDefinitions);
            project.WriteToFile(pbxPath);
        }

        private static void ApplyBuildProperties(PBXProject project,
                                                 string mainTargetGuid,
                                                 string frameworkTargetGuid,
                                                 IosBuildProperty[] properties)
        {
            if (properties == null)
            {
                return;
            }

            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                if (property == null || string.IsNullOrEmpty(property.Key) || string.IsNullOrEmpty(property.Value))
                {
                    continue;
                }

                if (property.ApplyToMainTarget)
                {
                    project.AddBuildProperty(mainTargetGuid, property.Key, property.Value);
                }

                if (property.ApplyToFrameworkTarget)
                {
                    project.AddBuildProperty(frameworkTargetGuid, property.Key, property.Value);
                }
            }
        }

        private static void ApplySearchPaths(PBXProject project,
                                             string mainTargetGuid,
                                             string frameworkTargetGuid,
                                             IosSearchPath[] searchPaths,
                                             System.Action<string, string> apply)
        {
            if (searchPaths == null || apply == null)
            {
                return;
            }

            for (int i = 0; i < searchPaths.Length; i++)
            {
                var searchPath = searchPaths[i];
                if (searchPath == null || string.IsNullOrEmpty(searchPath.Path))
                {
                    continue;
                }

                if (searchPath.ApplyToMainTarget)
                {
                    apply(mainTargetGuid, searchPath.Path);
                }

                if (searchPath.ApplyToFrameworkTarget)
                {
                    apply(frameworkTargetGuid, searchPath.Path);
                }
            }
        }

        private static void CollectMacroDefinitions(IosMacroDefinition[] macroDefinitions, HashSet<string> results)
        {
            if (macroDefinitions == null || results == null)
            {
                return;
            }

            for (int i = 0; i < macroDefinitions.Length; i++)
            {
                var definition = macroDefinitions[i];
                if (definition == null || string.IsNullOrEmpty(definition.Name))
                {
                    continue;
                }

                string value = definition.Value;
                string entry = string.IsNullOrEmpty(value)
                    ? $"-D{definition.Name}"
                    : $"-D{definition.Name}={value}";
                results.Add(entry);
            }
        }

        private static void ApplyMacroDefinitions(PBXProject project,
                                                  string mainTargetGuid,
                                                  string frameworkTargetGuid,
                                                  HashSet<string> macroDefinitions)
        {
            if (macroDefinitions == null || macroDefinitions.Count == 0)
            {
                return;
            }

            foreach (string macro in macroDefinitions)
            {
                project.AddBuildProperty(frameworkTargetGuid, BuildConfigurationKey.kOtherCFlags, macro);
            }
        }
    }
}
#endif