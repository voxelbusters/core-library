using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelBusters.CoreLibrary
{
    [System.Serializable]
    public class UnityPackageDefinition
    {
        #region Properties

        public string Name { get; private set; }

        public string DisplayName { get; private set; }

        public string Version { get; private set; }

        public string DefaultInstallPath { get; private set; }

        public string MutableResourcePath { get; private set; }

        public string MutableResourceRelativePath { get; private set; }

        #endregion

        #region Constructors

        public UnityPackageDefinition(string name, string displayName,
            string version, string defaultInstallPath = null,
            string mutableResourcePath = "Assets/Resources", string mutableResourceRelativePath = "")
        {
            // set properties
            Name                        = name;
            DisplayName                 = displayName;
            Version                     = version;
            DefaultInstallPath          = defaultInstallPath ?? $"Assets/{Name}";
            MutableResourcePath         = mutableResourcePath;
            MutableResourceRelativePath = mutableResourceRelativePath;
        }

        #endregion
    }
}