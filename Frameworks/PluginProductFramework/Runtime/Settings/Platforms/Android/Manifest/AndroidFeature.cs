using System;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Represents an Android uses-feature entry.
    /// </summary>
    [Serializable]
    public class AndroidFeature
    {
        #region Fields

        [SerializeField]
        private string m_name;

        [SerializeField]
        private bool m_required = true;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the feature name.
        /// </summary>
        public string Name => m_name;

        /// <summary>
        /// Gets a value indicating whether the feature is required.
        /// </summary>
        public bool Required => m_required;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new feature entry.
        /// </summary>
        public AndroidFeature(string name = null, bool required = true)
        {
            m_name = name;
            m_required = required;
        }

        #endregion

        #region Setters

        /// <summary>
        /// Sets the feature name.
        /// </summary>
        public void SetName(string name)
        {
            m_name = name;
        }

        /// <summary>
        /// Sets whether the feature is required.
        /// </summary>
        public void SetRequired(bool required)
        {
            m_required = required;
        }

        #endregion
    }
}
