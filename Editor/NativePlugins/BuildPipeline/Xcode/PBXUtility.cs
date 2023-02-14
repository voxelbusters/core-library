#if UNITY_IOS || UNITY_TVOS
using System.Collections;
using System.Collections.Generic;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.Editor.NativePlugins.Build.Xcode
{
    public static class PBXProjectUtility
    {
        public static ProjectCapabilityManager CreateDefaultProjectCapabilityManager(PBXProject project, string projectPath, string targetGuid)
        {
            string  fileName    = $"{Application.productName}.entitlements";
#if UNITY_2019_3_OR_NEWER
            return new ProjectCapabilityManager(projectPath, fileName, null, targetGuid);
#else
            return new ProjectCapabilityManager(projectPath, fileName, targetGuid);
#endif
        }
    }
}
#endif