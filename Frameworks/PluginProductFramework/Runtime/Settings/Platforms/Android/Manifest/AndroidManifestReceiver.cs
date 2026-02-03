using System;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Represents a receiver entry.
    /// </summary>
    [Serializable]
    public class AndroidManifestReceiver
    {
        #region Fields

        [SerializeField]
        private string m_name;

        [SerializeField]
        private AndroidManifestAttribute[] m_attributes = new AndroidManifestAttribute[0];

        [SerializeField]
        private AndroidManifestIntentFilter[] m_intentFilters = new AndroidManifestIntentFilter[0];

        #endregion

        #region Properties

        /// <summary>
        /// Gets the receiver name.
        /// </summary>
        public string Name => m_name;

        /// <summary>
        /// Gets the receiver attributes.
        /// </summary>
        public AndroidManifestAttribute[] Attributes => m_attributes;

        /// <summary>
        /// Gets the receiver intent filters.
        /// </summary>
        public AndroidManifestIntentFilter[] IntentFilters => m_intentFilters;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new receiver entry.
        /// </summary>
        public AndroidManifestReceiver(string name = null,
                                       AndroidManifestAttribute[] attributes = null,
                                       AndroidManifestIntentFilter[] intentFilters = null)
        {
            m_name = name;
            m_attributes = attributes ?? Array.Empty<AndroidManifestAttribute>();
            m_intentFilters = intentFilters ?? Array.Empty<AndroidManifestIntentFilter>();
        }

        #endregion

        #region Setters

        /// <summary>
        /// Sets the receiver name.
        /// </summary>
        public void SetName(string name)
        {
            m_name = name;
        }

        /// <summary>
        /// Sets the receiver attributes.
        /// </summary>
        public void SetAttributes(AndroidManifestAttribute[] attributes)
        {
            m_attributes = attributes ?? Array.Empty<AndroidManifestAttribute>();
        }

        /// <summary>
        /// Sets the receiver intent filters.
        /// </summary>
        public void SetIntentFilters(AndroidManifestIntentFilter[] intentFilters)
        {
            m_intentFilters = intentFilters ?? Array.Empty<AndroidManifestIntentFilter>();
        }

        #endregion
    }
}
