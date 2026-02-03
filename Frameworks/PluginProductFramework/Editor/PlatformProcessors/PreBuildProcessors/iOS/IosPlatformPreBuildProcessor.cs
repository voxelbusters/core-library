#if UNITY_IOS || UNITY_TVOS
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework.Editor
{
    /// <summary>
    /// Generates iOS pre-build dependency XML from product-scoped platform configurations.
    /// </summary>
    internal static class IosPlatformPreBuildProcessor
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
            ProcessAll(templateRootPath, assetsRootPath);
        }

        public static void ProcessAll(string templateRootPath, string assetsRootPath)
        {
            UpdateDependenciesXml(templateRootPath, assetsRootPath);
        }

        private static void UpdateDependenciesXml(string templateRootPath, string assetsRootPath)
        {
            if (string.IsNullOrEmpty(assetsRootPath))
            {
                return;
            }

            var configs = PlatformConfigurationUtility.GetOrCreateEditableConfigs<IosPlatformConfiguration>(
                templateRootPath,
                assetsRootPath);
            var dependencies = CollectDependencies(configs);
            string resolvedPath = ResolveOutputPath(assetsRootPath);
            WriteDependenciesXml(resolvedPath, dependencies);
        }

        private static List<IosPodDependency> CollectDependencies(List<IosPlatformConfiguration> configs)
        {
            var map = new Dictionary<string, IosPodDependency>(System.StringComparer.Ordinal);
            for (int i = 0; i < configs.Count; i++)
            {
                var config = configs[i];
                if (config == null)
                {
                    continue;
                }

                var deps = config.PodDependencies;
                if (deps == null)
                {
                    continue;
                }

                for (int j = 0; j < deps.Length; j++)
                {
                    var dep = deps[j];
                    if (dep == null || string.IsNullOrEmpty(dep.Name))
                    {
                        continue;
                    }

                    map[dep.Name] = dep;
                }
            }

            var list = new List<IosPodDependency>(map.Values);
            list.Sort((a, b) => string.CompareOrdinal(a?.Name, b?.Name));
            return list;
        }

        private static void WriteDependenciesXml(string path, List<IosPodDependency> dependencies)
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
                writer.WriteComment("DONT MODIFY HERE. AUTO GENERATED DEPENDENCIES FROM IosPlatformPreBuildProcessor.cs");

                writer.WriteStartElement("dependencies");
                writer.WriteStartElement("iosPods");
                for (int i = 0; i < dependencies.Count; i++)
                {
                    IosPodDependency dependency = dependencies[i];
                    writer.WriteStartElement("iosPod");
                    writer.WriteAttributeString("name", dependency.Name);
                    if (!string.IsNullOrEmpty(dependency.Version))
                    {
                        writer.WriteAttributeString("version", dependency.Version);
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private static string ResolveOutputPath(string assetsRootPath)
        {
            return Path.Combine(assetsRootPath, PluginProductFrameworkConstants.IosDependenciesOutputRelativePath);
        }
    }
}
#endif
