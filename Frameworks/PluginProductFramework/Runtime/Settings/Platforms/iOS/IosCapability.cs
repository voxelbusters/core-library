using System;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Represents an iOS capability entry.
    /// </summary>
    [Serializable]
    public class IosCapability
    {
        #region Fields

        [SerializeField]
        private IosCapabilityType m_type;

        [SerializeField]
        private IosAssociatedDomain[] m_associatedDomains = new IosAssociatedDomain[0];

        #endregion

        #region Properties

        /// <summary>
        /// Gets the capability type.
        /// </summary>
        public IosCapabilityType Type => m_type;

        /// <summary>
        /// Gets the associated domains list.
        /// </summary>
        public IosAssociatedDomain[] AssociatedDomains => m_associatedDomains;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new capability entry.
        /// </summary>
        public IosCapability(IosCapabilityType type = IosCapabilityType.GameCenter,
                             IosAssociatedDomain[] associatedDomains = null)
        {
            m_type = type;
            m_associatedDomains = associatedDomains ?? Array.Empty<IosAssociatedDomain>();
        }

        #endregion

        #region Setters

        /// <summary>
        /// Sets the capability type.
        /// </summary>
        public void SetType(IosCapabilityType type)
        {
            m_type = type;
        }

        /// <summary>
        /// Sets the associated domains list.
        /// </summary>
        public void SetAssociatedDomains(IosAssociatedDomain[] associatedDomains)
        {
            m_associatedDomains = associatedDomains ?? Array.Empty<IosAssociatedDomain>();
        }

        #endregion
    }
}
