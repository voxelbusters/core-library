#if UNITY_IOS || UNITY_TVOS
namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework.Editor
{
    internal static class IosPostBuildConstants
    {
        internal const string InfoPlistFileName = "Info.plist";
        internal const string PlistUrlTypesKey = "CFBundleURLTypes";
        internal const string PlistUrlSchemesKey = "CFBundleURLSchemes";
        internal const string CodeSignEntitlementsKey = "CODE_SIGN_ENTITLEMENTS";
        internal const string DefaultEntitlementsFileName = "Unity-iPhone.entitlements";
        internal const string DefaultAssociatedDomainsServiceType = "applinks";
        internal const string UnityIphoneTargetName = "Unity-iPhone";
        internal const string UnityChinaTargetName = "Tuanjie-iPhone";
    }
}
#endif