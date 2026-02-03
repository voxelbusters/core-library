using UnityEngine;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Base class for product-wide general settings.
    /// </summary>
    public abstract class PluginProductGeneralSettingsBase : ScriptableObject
    {
        #region Fields

        [SerializeField]
        private DebugLogger.LogLevel m_logLevel = DebugLogger.LogLevel.Critical;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the log level used by the product.
        /// </summary>
        public DebugLogger.LogLevel LogLevel => m_logLevel;

        #endregion
    }
}
