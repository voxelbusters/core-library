#if UNITY_IOS || UNITY_TVOS
using UnityEditor.iOS.Xcode;
using VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework.Editor
{
    internal static class IosPostBuildProjectUtility
    {
        internal static string GetMainTargetName(PBXProject project)
        {
#if UNITY_CHINA
            return IosPostBuildConstants.UnityChinaTargetName;
#else
            return IosPostBuildConstants.UnityIphoneTargetName;
#endif

        }

        internal static string GetMainTargetGuid(PBXProject project)
        {
#if UNITY_CHINA
            return project.TargetGuidByName(GetMainTargetName(project));
#else
            return project.GetUnityMainTargetGuid();
#endif
        }
    }
}
#endif