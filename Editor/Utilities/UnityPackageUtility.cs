using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VoxelBusters.CoreLibrary.Editor
{
    public static class UnityPackageUtility
    {
        #region Static methods

        private static string GetPackageFullPathInternal(string package, string customInstallPath)
        {
            string  installPathInAssets = GetDefaultOrCustomInstallPathInAssets(package: package, customInstallPath: customInstallPath);
            if (IOServices.DirectoryExists(installPathInAssets))
            {
                return installPathInAssets;
            }
            return "Packages/" + package;
        }

        private static string GetDefaultOrCustomInstallPathInAssets(string package, string customInstallPath)
        {
            return customInstallPath ?? ("Assets/" + package);
        }

        #endregion

        #region Public methods

        public static bool IsInstalledInAssets(string package, string customInstallPath = null)
        {
            return IOServices.DirectoryExists(GetDefaultOrCustomInstallPathInAssets(package: package, customInstallPath: customInstallPath));
        }

        public static string GetPackagePath(string package, string customInstallPath = null)
        {
            return GetPackageFullPathInternal(package: package, customInstallPath: customInstallPath);
        }

        public static string GetEditorResourcesPath(string package, string customInstallPath = null)
        {
            return GetPackagePath(package: package, customInstallPath: customInstallPath) + "/EditorResources";
        }

        public static string GetResourcesPath(string package, string customInstallPath = null)
        {
            return GetPackagePath(package: package, customInstallPath: customInstallPath) + "/Resources";
        }

        public static string GetEditorScriptsPath(string package, string customInstallPath = null)
        {
            return GetPackagePath(package: package, customInstallPath: customInstallPath) + "/Editor";
        }

        public static string GetRuntimeScriptsPath(string package, string customInstallPath = null)
        {
            return GetPackagePath(package: package, customInstallPath: customInstallPath) + "/Runtime";
        }

        #endregion
    }
}