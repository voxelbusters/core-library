namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Allows a feature settings asset to provide validation feedback.
    /// </summary>
    public interface IFeatureSettingsValidator
    {
        bool TryGetValidationMessage(out string message, out FeatureSettingsValidationStatus status);
    }
}
