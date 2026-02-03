using System;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Represents a system framework reference.
    /// </summary>
    [Serializable]
    public class IosFrameworkReference
    {
        #region Fields

        [SerializeField]
        private string m_name;

        [SerializeField]
        private bool m_isWeak;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the framework name.
        /// </summary>
        public string Name => m_name;

        /// <summary>
        /// Gets a value indicating whether the framework is weak-linked.
        /// </summary>
        public bool IsWeak => m_isWeak;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new framework reference entry.
        /// </summary>
        public IosFrameworkReference(string name = null, bool isWeak = false)
        {
            m_name = name;
            m_isWeak = isWeak;
        }

        #endregion

        #region Setters

        /// <summary>
        /// Sets the framework name.
        /// </summary>
        public void SetName(string name)
        {
            m_name = name;
        }

        /// <summary>
        /// Sets whether the framework is weak-linked.
        /// </summary>
        public void SetIsWeak(bool isWeak)
        {
            m_isWeak = isWeak;
        }

        #endregion
    }
}
