using System;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Represents an Android query intent entry.
    /// </summary>
    [Serializable]
    public class AndroidQueryIntent
    {
        #region Fields

        [SerializeField]
        private string m_action;

        [SerializeField]
        private string m_scheme;

        [SerializeField]
        private string m_host;

        [SerializeField]
        private string m_path;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the intent action.
        /// </summary>
        public string Action => m_action;

        /// <summary>
        /// Gets the intent URI scheme.
        /// </summary>
        public string Scheme => m_scheme;

        /// <summary>
        /// Gets the intent host.
        /// </summary>
        public string Host => m_host;

        /// <summary>
        /// Gets the intent path.
        /// </summary>
        public string Path => m_path;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new query intent entry.
        /// </summary>
        public AndroidQueryIntent(string action = null, string scheme = null, string host = null, string path = null)
        {
            m_action = action;
            m_scheme = scheme;
            m_host = host;
            m_path = path;
        }

        #endregion

        #region Setters

        /// <summary>
        /// Sets the intent action.
        /// </summary>
        public void SetAction(string action)
        {
            m_action = action;
        }

        /// <summary>
        /// Sets the intent URI scheme.
        /// </summary>
        public void SetScheme(string scheme)
        {
            m_scheme = scheme;
        }

        /// <summary>
        /// Sets the intent host.
        /// </summary>
        public void SetHost(string host)
        {
            m_host = host;
        }

        /// <summary>
        /// Sets the intent path.
        /// </summary>
        public void SetPath(string path)
        {
            m_path = path;
        }

        #endregion
    }
}
