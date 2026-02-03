using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework.Editor
{
    /// <summary>
    /// Helpers for locating platform configuration assets.
    /// </summary>
    internal static class PlatformConfigurationUtility
    {
        private const string kTemplateSuffix = "Template.asset";

        public static List<T> FindAll<T>() where T : PlatformConfigurationObject
        {
            var results = new List<T>();
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset != null)
                {
                    results.Add(asset);
                }
            }
            return results;
        }

        public static List<T> FindAll<T>(string searchFolder) where T : PlatformConfigurationObject
        {
            if (string.IsNullOrEmpty(searchFolder))
            {
                return FindAll<T>();
            }

            var results = new List<T>();
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { searchFolder });
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset != null)
                {
                    results.Add(asset);
                }
            }
            return results;
        }

        public static List<T> GetOrCreateEditableConfigs<T>(string templateRoot, string assetsRoot)
            where T : PlatformConfigurationObject
        {
            if (string.IsNullOrEmpty(assetsRoot))
            {
                return new List<T>();
            }

            if (!string.IsNullOrEmpty(templateRoot))
            {
                EnsureEditableCopies<T>(templateRoot, assetsRoot);
            }

            var configs = FindAll<T>(assetsRoot);
            configs.RemoveAll(config => IsTemplateAssetPath(AssetDatabase.GetAssetPath(config)));
            return configs;
        }

        private static void EnsureEditableCopies<T>(string templateRoot, string assetsRoot)
            where T : PlatformConfigurationObject
        {
            if (string.IsNullOrEmpty(templateRoot) || string.IsNullOrEmpty(assetsRoot))
            {
                return;
            }

            string normalizedTemplateRoot = NormalizePath(templateRoot);
            string normalizedAssetsRoot = NormalizePath(assetsRoot);
            if (string.IsNullOrEmpty(normalizedTemplateRoot) || string.IsNullOrEmpty(normalizedAssetsRoot))
            {
                return;
            }

            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { normalizedTemplateRoot });
            for (int i = 0; i < guids.Length; i++)
            {
                string templatePath = NormalizePath(AssetDatabase.GUIDToAssetPath(guids[i]));
                if (string.IsNullOrEmpty(templatePath))
                {
                    continue;
                }

                string targetPath = GetEditableAssetPath(templatePath, normalizedTemplateRoot, normalizedAssetsRoot);
                if (string.IsNullOrEmpty(targetPath))
                {
                    continue;
                }

                if (string.Equals(templatePath, targetPath, StringComparison.Ordinal))
                {
                    continue;
                }

                if (File.Exists(targetPath))
                {
                    continue;
                }

                string targetDirectory = Path.GetDirectoryName(targetPath);
                if (!string.IsNullOrEmpty(targetDirectory) && !Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                AssetDatabase.CopyAsset(templatePath, targetPath);
                AssetDatabase.ImportAsset(targetPath, ImportAssetOptions.ForceUpdate);
            }
        }

        private static string GetEditableAssetPath(string templatePath, string templateRoot, string assetsRoot)
        {
            if (!IsPathUnderRoot(templatePath, templateRoot))
            {
                return null;
            }

            string relativePath = templatePath.Substring(templateRoot.Length).TrimStart('/');
            string fileName = Path.GetFileName(relativePath);
            if (fileName.EndsWith(kTemplateSuffix, StringComparison.OrdinalIgnoreCase))
            {
                fileName = fileName.Substring(0, fileName.Length - kTemplateSuffix.Length) + ".asset";
            }

            string relativeDir = Path.GetDirectoryName(relativePath);
            string targetPath = string.IsNullOrEmpty(relativeDir)
                ? Path.Combine(assetsRoot, fileName)
                : Path.Combine(assetsRoot, relativeDir, fileName);
            return NormalizePath(targetPath);
        }

        private static bool IsTemplateAssetPath(string assetPath)
        {
            return !string.IsNullOrEmpty(assetPath) &&
                   assetPath.EndsWith(kTemplateSuffix, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsPathUnderRoot(string assetPath, string rootPath)
        {
            if (string.IsNullOrEmpty(assetPath) || string.IsNullOrEmpty(rootPath))
            {
                return false;
            }

            string normalizedPath = NormalizePath(assetPath);
            string normalizedRoot = NormalizePath(rootPath).TrimEnd('/');
            if (string.Equals(normalizedPath, normalizedRoot, StringComparison.Ordinal))
            {
                return true;
            }

            return normalizedPath.StartsWith(normalizedRoot + "/", StringComparison.Ordinal);
        }

        private static string NormalizePath(string path)
        {
            return string.IsNullOrEmpty(path) ? null : path.Replace('\\', '/');
        }
    }
}
