using System;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Represents an Android manifest meta-data entry.
    /// </summary>
    [Serializable]
    public class AndroidMetaData
    {
        #region Fields

        [SerializeField]
        private string m_name;

        [SerializeField]
        private string m_value;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the meta-data name.
        /// </summary>
        public string Name => m_name;

        /// <summary>
        /// Gets the meta-data value.
        /// </summary>
        public string Value => m_value;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new meta-data entry.
        /// </summary>
        public AndroidMetaData(string name = null, string value = null)
        {
            m_name = name;
            m_value = value;
        }

        #endregion

        #region Setters

        /// <summary>
        /// Sets the meta-data name.
        /// </summary>
        public void SetName(string name)
        {
            m_name = name;
        }

        /// <summary>
        /// Sets the meta-data value.
        /// </summary>
        public void SetValue(string value)
        {
            m_value = value;
        }

        #endregion
    }
}
