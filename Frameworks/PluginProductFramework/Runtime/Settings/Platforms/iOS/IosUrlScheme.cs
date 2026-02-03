using System;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Represents a URL scheme entry.
    /// </summary>
    [Serializable]
    public class IosUrlScheme
    {
        #region Fields

        [SerializeField]
        private string m_scheme;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the URL scheme.
        /// </summary>
        public string Scheme => m_scheme;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new URL scheme entry.
        /// </summary>
        public IosUrlScheme(string scheme = null)
        {
            m_scheme = scheme;
        }

        #endregion

        #region Setters

        /// <summary>
        /// Sets the URL scheme.
        /// </summary>
        public void SetScheme(string scheme)
        {
            m_scheme = scheme;
        }

        #endregion
    }
}
