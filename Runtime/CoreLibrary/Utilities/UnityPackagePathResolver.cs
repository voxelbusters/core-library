using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VoxelBusters.CoreLibrary
{
    public static class UnityPackagePathResolver
    {
        #region Static methods

        public static bool IsInstalledWithinAssets(this UnityPackageDefinition package)
        {
            return IOServices.DirectoryExists(package.DefaultInstallPath);
        }

        public static string GetInstallPath(this UnityPackageDefinition package)
        {
            if (IsSupported())
            {
                return  IsInstalledWithinAssets(package) ? package.DefaultInstallPath : $"package/{package.Name}";
            }
            return null;
        }

        public static string GetRuntimeScriptsPath(this UnityPackageDefinition package)
        {
            return CombinePath(pathA: GetInstallPath(package), pathB: "Runtime");
        }

        public static string GetEditorScriptsPath(this UnityPackageDefinition package)
        {
            return CombinePath(pathA: GetInstallPath(package), pathB: "Editor");
        }

        public static string GetEditorResourcesPath(this UnityPackageDefinition package)
        {
            return CombinePath(pathA: GetInstallPath(package), pathB: "EditorResources");
        }

        public static string GetImmutableResourcesPath(this UnityPackageDefinition package)
        {
            return CombinePath(pathA: GetInstallPath(package), pathB: "Resources");
        }

        public static string GetFullPath(this UnityPackageDefinition package, string relativePath)
        {
            return CombinePath(pathA: GetInstallPath(package), pathB: relativePath);
        }

        public static string GetMutableResourceRelativePath(this UnityPackageDefinition package, string name)
        {
            return CombinePath(pathA: package.MutableResourceRelativePath, pathB: name);
        }

        private static bool IsSupported() => Application.isEditor;

        private static string CombinePath(string pathA, string pathB)
        {
            if (pathA == null)
            {
                return null;
            }
            else if (pathA == "")
            {
                return pathB;
            }
            else
            {
                return $"{pathA}/{pathB}";
            }
        }

        #endregion
    }
}