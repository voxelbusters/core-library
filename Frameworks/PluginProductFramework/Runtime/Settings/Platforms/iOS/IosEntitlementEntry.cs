using System;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Represents an entitlements entry.
    /// </summary>
    [Serializable]
    public class IosEntitlementEntry
    {
        #region Fields

        [SerializeField]
        private string m_key;

        [SerializeField]
        private string m_value;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the entry key.
        /// </summary>
        public string Key => m_key;

        /// <summary>
        /// Gets the entry value.
        /// </summary>
        public string Value => m_value;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new entitlements entry.
        /// </summary>
        public IosEntitlementEntry(string key = null, string value = null)
        {
            m_key = key;
            m_value = value;
        }

        #endregion

        #region Setters

        /// <summary>
        /// Sets the entry key.
        /// </summary>
        public void SetKey(string key)
        {
            m_key = key;
        }

        /// <summary>
        /// Sets the entry value.
        /// </summary>
        public void SetValue(string value)
        {
            m_value = value;
        }

        #endregion
    }
}
