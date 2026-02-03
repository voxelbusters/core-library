namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Exposes feature settings shared by feature-specific settings assets.
    /// </summary>
    public interface IFeatureSettings
    {
        #region Properties

        /// <summary>
        /// Gets the feature descriptor associated with these settings.
        /// </summary>
        FeatureDescriptor FeatureDescriptor { get; }

        /// <summary>
        /// Gets a value indicating whether the feature is enabled.
        /// </summary>
        bool IsEnabled { get; }

        #endregion
    }
}
