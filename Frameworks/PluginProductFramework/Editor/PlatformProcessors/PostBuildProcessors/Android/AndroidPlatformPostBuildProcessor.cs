#if UNITY_EDITOR && UNITY_ANDROID
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Android;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework.Editor
{
    /// <summary>
    /// Android post-build processor for all VoxelBusters products.
    /// </summary>
    internal class AndroidPlatformPostBuildProcessor : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder => 0;

        public void OnPostGenerateGradleAndroidProject(string basePath)
        {
            PluginProductSettingsUtility.LogMissingSettingsWarnings(
                PluginProductDescriptorUtility.FindAllPluginProductDescriptors(),
                nameof(AndroidPlatformPostBuildProcessor));
            ApplyGradlePatches(basePath);
        }

        protected virtual void ApplyGradlePatches(string basePath)
        {
            // Placeholder for future gradle patching driven by configuration.
        }

        protected void PatchGradleForPackingOptions(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            string gradleFilePath = Path.Combine(path, "build.gradle");
            if (!File.Exists(gradleFilePath))
            {
                DebugLogger.LogWarning(CoreLibraryDomain.Default,
                    $"[AndroidPlatformPostBuildProcessor] build.gradle not found at '{gradleFilePath}'.");
                return;
            }

            List<string> lines = File.ReadAllLines(gradleFilePath).ToList();
            if (lines.Any(line => line.Contains("packagingOptions", StringComparison.Ordinal)))
            {
                return;
            }

            int index = -1;
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                if (line.Contains("apply plugin", StringComparison.Ordinal) &&
                    (line.Contains("com.android.application", StringComparison.Ordinal) ||
                     line.Contains("com.android.library", StringComparison.Ordinal)))
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                return;
            }

            string textToInsert = @"android {
                packagingOptions {
                    exclude 'META-INF/proguard/androidx-annotations.pro'
                }
            }";

            lines.Insert(index + 1, textToInsert);
            File.WriteAllLines(gradleFilePath, lines);
        }
    }
}
#endif
