using System;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Represents an associated domain entry.
    /// </summary>
    [Serializable]
    public class IosAssociatedDomain
    {
        #region Fields

        [SerializeField]
        private string m_serviceType;

        [SerializeField]
        private string m_host;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the associated domain service type.
        /// </summary>
        public string ServiceType => m_serviceType;

        /// <summary>
        /// Gets the associated domain host.
        /// </summary>
        public string Host => m_host;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new associated domain entry.
        /// </summary>
        public IosAssociatedDomain(string serviceType = null, string host = null)
        {
            m_serviceType = serviceType;
            m_host = host;
        }

        #endregion

        #region Setters

        /// <summary>
        /// Sets the associated domain service type.
        /// </summary>
        public void SetServiceType(string serviceType)
        {
            m_serviceType = serviceType;
        }

        /// <summary>
        /// Sets the associated domain host.
        /// </summary>
        public void SetHost(string host)
        {
            m_host = host;
        }

        #endregion
    }
}
