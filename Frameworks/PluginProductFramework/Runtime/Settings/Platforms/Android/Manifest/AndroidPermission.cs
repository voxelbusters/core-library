using System;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Represents an Android permission entry.
    /// </summary>
    [Serializable]
    public class AndroidPermission
    {
        #region Fields

        [SerializeField]
        private string m_name;

        [SerializeField]
        private string m_maxSdkVersion;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the permission name.
        /// </summary>
        public string Name => m_name;

        /// <summary>
        /// Gets the max SDK version constraint.
        /// </summary>
        public string MaxSdkVersion => m_maxSdkVersion;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new permission entry.
        /// </summary>
        public AndroidPermission(string name = null, string maxSdkVersion = null)
        {
            m_name = name;
            m_maxSdkVersion = maxSdkVersion;
        }

        #endregion

        #region Setters

        /// <summary>
        /// Sets the permission name.
        /// </summary>
        public void SetName(string name)
        {
            m_name = name;
        }

        /// <summary>
        /// Sets the max SDK version constraint.
        /// </summary>
        public void SetMaxSdkVersion(string maxSdkVersion)
        {
            m_maxSdkVersion = maxSdkVersion;
        }

        #endregion
    }
}
