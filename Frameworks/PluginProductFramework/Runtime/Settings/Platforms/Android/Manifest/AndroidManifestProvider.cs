using System;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Represents a provider entry.
    /// </summary>
    [Serializable]
    public class AndroidManifestProvider
    {
        #region Fields

        [SerializeField]
        private string m_name;

        [SerializeField]
        private AndroidManifestAttribute[] m_attributes = new AndroidManifestAttribute[0];

        #endregion

        #region Properties

        /// <summary>
        /// Gets the provider name.
        /// </summary>
        public string Name => m_name;

        /// <summary>
        /// Gets the provider attributes.
        /// </summary>
        public AndroidManifestAttribute[] Attributes => m_attributes;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new provider entry.
        /// </summary>
        public AndroidManifestProvider(string name = null, AndroidManifestAttribute[] attributes = null)
        {
            m_name = name;
            m_attributes = attributes ?? Array.Empty<AndroidManifestAttribute>();
        }

        #endregion

        #region Setters

        /// <summary>
        /// Sets the provider name.
        /// </summary>
        public void SetName(string name)
        {
            m_name = name;
        }

        public void SetAttributes(AndroidManifestAttribute[] attributes)
        {
            m_attributes = attributes ?? Array.Empty<AndroidManifestAttribute>();
        }

        #endregion
    }
}
