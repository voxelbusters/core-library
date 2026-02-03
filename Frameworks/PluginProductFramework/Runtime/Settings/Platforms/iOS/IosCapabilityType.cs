namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Supported iOS capability types.
    /// </summary>
    public enum IosCapabilityType
    {
        /// <summary>
        /// Game Center capability.
        /// </summary>
        GameCenter = 1,
        /// <summary>
        /// In-app purchase capability.
        /// </summary>
        InAppPurchase,
        /// <summary>
        /// iCloud capability.
        /// </summary>
        iCloud,
        /// <summary>
        /// Push notifications capability.
        /// </summary>
        PushNotifications,
        /// <summary>
        /// Associated domains capability.
        /// </summary>
        AssociatedDomains
    }
}
