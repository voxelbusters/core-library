using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework.Editor
{
    /// <summary>
    /// Generates Android pre-build artifacts from product-scoped platform configurations.
    /// </summary>
    internal static class AndroidPlatformPreBuildProcessor
    {
        public static void ProcessAll(PluginProductDescriptor descriptor)
        {
            if (descriptor == null)
            {
                return;
            }

            string assetsRootPath = PluginProductDescriptorUtility.GetAssetsProductRootPath(descriptor);
            if (string.IsNullOrEmpty(assetsRootPath))
            {
                return;
            }

            string templateRootPath = PluginProductDescriptorUtility.GetProductRootPath(descriptor);
            ProcessAll(templateRootPath, assetsRootPath, descriptor.ProductCodeName);
        }

        public static void ProcessAll(string templateRootPath, string assetsRootPath, string productCodeName)
        {
            if (string.IsNullOrEmpty(assetsRootPath))
            {
                return;
            }

            var configs = PlatformConfigurationUtility.GetOrCreateEditableConfigs<AndroidPlatformConfiguration>(
                templateRootPath,
                assetsRootPath);
            UpdateDependenciesXml(assetsRootPath, configs);
            UpdateManifestFile(assetsRootPath, productCodeName, configs);
        }

        private static void UpdateDependenciesXml(string assetsRootPath, List<AndroidPlatformConfiguration> configs)
        {
            var dependencies = CollectDependencies(configs);
            string resolvedPath = ResolveDependenciesPath(assetsRootPath);
            WriteDependenciesXml(resolvedPath, dependencies);
        }

        private static void UpdateManifestFile(string assetsRootPath,
                                               string productCodeName,
                                               List<AndroidPlatformConfiguration> configs)
        {
            if (string.IsNullOrEmpty(productCodeName) || configs == null || configs.Count == 0)
            {
                return;
            }

            string packageName = BuildAndroidLibraryPackageName(productCodeName);
            string manifestPath = ResolveManifestPath(assetsRootPath, packageName);
            var manifest = new AndroidManifestDocument(manifestPath, packageName);

            for (int i = 0; i < configs.Count; i++)
            {
                var config = configs[i];
                if (config == null)
                {
                    continue;
                }

                ApplyManifestConfiguration(manifest, config);
                ApplyPermissions(manifest, config);
                ApplyUsesFeatures(manifest, config);
                ApplyMetaData(manifest, config);
                ApplyQueries(manifest, config);
            }

            manifest.Save();
        }

        private static void ApplyPermissions(AndroidManifestDocument manifest, AndroidPlatformConfiguration config)
        {
            var permissions = config.ManifestConfiguration?.Permissions;
            if (permissions == null)
            {
                return;
            }

            foreach (var permission in permissions)
            {
                if (permission == null || string.IsNullOrEmpty(permission.Name))
                {
                    continue;
                }

                manifest.AddUsesPermission(permission.Name, permission.MaxSdkVersion);
            }
        }

        private static void ApplyManifestConfiguration(AndroidManifestDocument manifest, AndroidPlatformConfiguration config)
        {
            var manifestConfig = config.ManifestConfiguration;
            if (manifestConfig == null)
            {
                return;
            }

            manifest.AddManifestAttributes(manifestConfig.ManifestAttributes);
            manifest.AddApplicationAttributes(manifestConfig.ApplicationAttributes);
            manifest.AddActivities(manifestConfig.Activities);
            manifest.AddProviders(manifestConfig.Providers);
            manifest.AddServices(manifestConfig.Services);
            manifest.AddReceivers(manifestConfig.Receivers);
        }

        private static void ApplyUsesFeatures(AndroidManifestDocument manifest, AndroidPlatformConfiguration config)
        {
            var features = config.ManifestConfiguration?.Features;
            if (features == null)
            {
                return;
            }

            foreach (var feature in features)
            {
                if (feature == null || string.IsNullOrEmpty(feature.Name))
                {
                    continue;
                }

                manifest.AddUsesFeature(feature.Name, feature.Required);
            }
        }

        private static void ApplyMetaData(AndroidManifestDocument manifest, AndroidPlatformConfiguration config)
        {
            var entries = config.ManifestConfiguration?.MetaData;
            if (entries == null)
            {
                return;
            }

            foreach (var entry in entries)
            {
                if (entry == null || string.IsNullOrEmpty(entry.Name))
                {
                    continue;
                }

                manifest.AddMetaData(entry.Name, entry.Value);
            }
        }

        private static void ApplyQueries(AndroidManifestDocument manifest, AndroidPlatformConfiguration config)
        {
            var queries = config.ManifestConfiguration?.Queries;
            if (queries == null)
            {
                return;
            }

            foreach (var query in queries)
            {
                if (query == null || string.IsNullOrEmpty(query.Action))
                {
                    continue;
                }

                manifest.AddQuery(query.Action, query.Scheme, query.Host, query.Path);
            }
        }

        private static List<AndroidGradleDependency> CollectDependencies(List<AndroidPlatformConfiguration> configs)
        {
            var map = new Dictionary<string, AndroidGradleDependency>(StringComparer.Ordinal);
            for (int i = 0; i < configs.Count; i++)
            {
                var config = configs[i];
                if (config == null)
                {
                    continue;
                }

                var deps = config.Dependencies;
                if (deps == null)
                {
                    continue;
                }

                for (int j = 0; j < deps.Length; j++)
                {
                    var dep = deps[j];
                    if (dep == null)
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(dep.Group) ||
                        string.IsNullOrEmpty(dep.Artifact) ||
                        string.IsNullOrEmpty(dep.Version))
                    {
                        continue;
                    }

                    string key = dep.Group + ":" + dep.Artifact;
                    map[key] = dep;
                }
            }

            var list = new List<AndroidGradleDependency>(map.Values);
            return list;
        }

        private static void WriteDependenciesXml(string path, List<AndroidGradleDependency> dependencies)
        {
            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(true),
                ConformanceLevel = ConformanceLevel.Document,
                Indent = true,
                NewLineOnAttributes = true,
                IndentChars = "\t"
            };

            using (XmlWriter writer = XmlWriter.Create(path, settings))
            {
                writer.WriteStartDocument();
                writer.WriteComment("DONT MODIFY HERE. AUTO GENERATED DEPENDENCIES FROM AndroidPlatformPreBuildProcessor.cs");

                writer.WriteStartElement("dependencies");
                writer.WriteStartElement("androidPackages");
                for (int i = 0; i < dependencies.Count; i++)
                {
                    AndroidGradleDependency dependency = dependencies[i];
                    writer.WriteStartElement("androidPackage");
                    writer.WriteAttributeString("spec", BuildSpec(dependency));
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private static string BuildSpec(AndroidGradleDependency dependency)
        {
            return dependency.Group + ":" + dependency.Artifact + ":" + dependency.Version;
        }

        private static string ResolveDependenciesPath(string assetsRootPath)
        {
            return Path.Combine(assetsRootPath, PluginProductFrameworkConstants.AndroidDependenciesOutputRelativePath);
        }

        private static string ResolveManifestPath(string assetsRootPath, string packageName)
        {
            return Path.Combine(assetsRootPath, PluginProductFrameworkConstants.AndroidPluginsFolder, packageName, "AndroidManifest.xml");
        }

        private static string BuildAndroidLibraryPackageName(string productCodeName)
        {
            return PluginProductFrameworkConstants.AndroidLibraryPrefix + ToKebabCase(productCodeName) + PluginProductFrameworkConstants.AndroidLibrarySuffix;
        }

        private static string ToKebabCase(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var builder = new StringBuilder(value.Length + 4);
            for (int i = 0; i < value.Length; i++)
            {
                char current = value[i];
                if (char.IsUpper(current) && i > 0)
                {
                    builder.Append('-');
                }
                builder.Append(char.ToLowerInvariant(current));
            }

            return builder.ToString();
        }
    }
}
