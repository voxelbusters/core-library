using System;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Represents an iOS CocoaPods dependency entry.
    /// </summary>
    [Serializable]
    public class IosPodDependency
    {
        #region Fields

        [SerializeField]
        private string m_name;

        [SerializeField]
        private string m_version;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the pod name.
        /// </summary>
        public string Name => m_name;

        /// <summary>
        /// Gets the pod version string.
        /// </summary>
        public string Version => m_version;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new pod dependency entry.
        /// </summary>
        public IosPodDependency(string name = null, string version = null)
        {
            m_name = name;
            m_version = version;
        }

        #endregion

        #region Setters

        /// <summary>
        /// Sets the pod name.
        /// </summary>
        public void SetName(string name)
        {
            m_name = name;
        }

        /// <summary>
        /// Sets the pod version string.
        /// </summary>
        public void SetVersion(string version)
        {
            m_version = version;
        }

        #endregion
    }
}
