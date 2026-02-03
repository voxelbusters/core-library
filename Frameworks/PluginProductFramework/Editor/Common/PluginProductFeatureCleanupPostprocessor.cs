using System.Collections.Generic;
using UnityEditor;
using VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework.Editor
{
    internal sealed class PluginProductFeatureCleanupPostprocessor : AssetPostprocessor
    {
        private const string kVoxelBustersRoot = "Assets/Plugins/VoxelBusters/";
        private const string kFeaturesSegment = "/Features/";

        private static void OnPostprocessAllAssets(string[] importedAssets,
                                                   string[] deletedAssets,
                                                   string[] movedAssets,
                                                   string[] movedFromAssetPaths)
        {
            if ((deletedAssets == null || deletedAssets.Length == 0) &&
                (movedFromAssetPaths == null || movedFromAssetPaths.Length == 0))
            {
                return;
            }

            var affectedProductRoots = new HashSet<string>();
            CollectAffectedRoots(deletedAssets, affectedProductRoots);
            CollectAffectedRoots(movedFromAssetPaths, affectedProductRoots);

            if (affectedProductRoots.Count == 0)
            {
                return;
            }

            foreach (string productRoot in affectedProductRoots)
            {
                PluginProductSettingsUtility.CleanupMissingFeatures(productRoot);
            }
        }

        private static void CollectAffectedRoots(string[] assetPaths, HashSet<string> roots)
        {
            if (assetPaths == null || roots == null)
            {
                return;
            }

            for (int i = 0; i < assetPaths.Length; i++)
            {
                string assetPath = assetPaths[i];
                if (string.IsNullOrEmpty(assetPath) ||
                    !assetPath.StartsWith(kVoxelBustersRoot, System.StringComparison.Ordinal) ||
                    assetPath.IndexOf(kFeaturesSegment, System.StringComparison.Ordinal) < 0)
                {
                    continue;
                }

                if (TryGetProductRoot(assetPath, out string productRoot))
                {
                    roots.Add(productRoot);
                }
            }
        }

        private static bool TryGetProductRoot(string assetPath, out string productRoot)
        {
            productRoot = null;
            if (string.IsNullOrEmpty(assetPath) ||
                !assetPath.StartsWith(kVoxelBustersRoot, System.StringComparison.Ordinal))
            {
                return false;
            }

            string remainder = assetPath.Substring(kVoxelBustersRoot.Length);
            int slashIndex = remainder.IndexOf('/');
            if (slashIndex <= 0)
            {
                return false;
            }

            productRoot = $"{kVoxelBustersRoot}{remainder.Substring(0, slashIndex)}";
            return true;
        }
    }
}
