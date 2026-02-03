using System;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Represents a data element for an intent filter.
    /// </summary>
    [Serializable]
    public class AndroidManifestData
    {
        #region Fields

        [SerializeField]
        private string m_scheme;

        [SerializeField]
        private string m_host;

        [SerializeField]
        private string m_path;

        [SerializeField]
        private string m_pathPrefix;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the URI scheme.
        /// </summary>
        public string Scheme => m_scheme;

        /// <summary>
        /// Gets the host.
        /// </summary>
        public string Host => m_host;

        /// <summary>
        /// Gets the path.
        /// </summary>
        public string Path => m_path;

        /// <summary>
        /// Gets the path prefix.
        /// </summary>
        public string PathPrefix => m_pathPrefix;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new data entry.
        /// </summary>
        public AndroidManifestData(string scheme = null, string host = null, string path = null, string pathPrefix = null)
        {
            m_scheme = scheme;
            m_host = host;
            m_path = path;
            m_pathPrefix = pathPrefix;
        }

        #endregion

        #region Setters

        /// <summary>
        /// Sets the URI scheme.
        /// </summary>
        public void SetScheme(string scheme)
        {
            m_scheme = scheme;
        }

        /// <summary>
        /// Sets the host.
        /// </summary>
        public void SetHost(string host)
        {
            m_host = host;
        }

        /// <summary>
        /// Sets the path.
        /// </summary>
        public void SetPath(string path)
        {
            m_path = path;
        }

        /// <summary>
        /// Sets the path prefix.
        /// </summary>
        public void SetPathPrefix(string pathPrefix)
        {
            m_pathPrefix = pathPrefix;
        }

        #endregion
    }
}
