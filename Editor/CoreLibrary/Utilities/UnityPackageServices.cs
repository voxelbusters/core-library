using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.Editor
{
    public static class UnityPackageServices
    {
        #region Static methods

        public static void InstallToUpm(this UnityPackageDefinition package)
        {
            try
            {
                // confirm that package exists in default install path
                if (!package.IsInstalledWithinAssets()) return;

                // move directory to new path
                IOServices.MoveDirectory(package.DefaultInstallPath, package.UpmInstallPath);
            }
            finally
            {
                AssetDatabase.Refresh();
            }
        }

        #endregion
    }
}