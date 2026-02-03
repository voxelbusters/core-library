using System;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Represents a manifest attribute name/value pair.
    /// </summary>
    [Serializable]
    public class AndroidManifestAttribute
    {
        #region Fields

        [SerializeField]
        private string m_name;

        [SerializeField]
        private string m_value;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the attribute name.
        /// </summary>
        public string Name => m_name;

        /// <summary>
        /// Gets the attribute value.
        /// </summary>
        public string Value => m_value;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new manifest attribute.
        /// </summary>
        public AndroidManifestAttribute(string name = null, string value = null)
        {
            m_name = name;
            m_value = value;
        }

        #endregion

        #region Setters

        /// <summary>
        /// Sets the attribute name.
        /// </summary>
        public void SetName(string name)
        {
            m_name = name;
        }

        /// <summary>
        /// Sets the attribute value.
        /// </summary>
        public void SetValue(string value)
        {
            m_value = value;
        }

        #endregion
    }
}
