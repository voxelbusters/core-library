using UnityEditor;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework.Editor
{
    /// <summary>
    /// Entry points for per-product pre-build processing.
    /// </summary>
    internal static class PlatformPreBuildProcessingRunner
    {
        internal static void ProcessAll(PluginProductDescriptor descriptor)
        {
            ProcessFromConfigurations(descriptor);
        }

        internal static void ProcessForBuildTarget(PluginProductDescriptor descriptor, BuildTarget buildTarget)
        {
#if UNITY_ANDROID
                AndroidPlatformPreBuildProcessor.ProcessAll(descriptor);
            return;
#elif UNITY_IOS || UNITY_TVOS
                IosPlatformPreBuildProcessor.ProcessAll(descriptor);
                return;
#endif
        }

        internal static void ProcessFromConfigurations(PluginProductDescriptor descriptor)
        {
#if UNITY_ANDROID
            AndroidPlatformPreBuildProcessor.ProcessAll(descriptor);
#elif UNITY_IOS || UNITY_TVOS  
            IosPlatformPreBuildProcessor.ProcessAll(descriptor);
#endif
        }

    }
}
