using UnityEditor;
using VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework.Editor
{
    internal sealed class FeatureDescriptorCleanupProcessor : AssetModificationProcessor
    {
        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            TryCleanupForDescriptor(assetPath);
            return AssetDeleteResult.DidNotDelete;
        }

        private static AssetMoveResult OnWillMoveAsset(string oldPath, string newPath)
        {
            TryCleanupForDescriptor(oldPath);
            return AssetMoveResult.DidNotMove;
        }

        private static void TryCleanupForDescriptor(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return;
            }

            var descriptor = AssetDatabase.LoadAssetAtPath<FeatureDescriptor>(assetPath);
            if (descriptor == null || descriptor.ProductDescriptor == null)
            {
                return;
            }

            PluginProductSettingsUtility.CleanupMissingFeatures(descriptor.ProductDescriptor);
        }
    }
}
