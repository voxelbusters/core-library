using System;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Represents an Xcode search path entry.
    /// </summary>
    [Serializable]
    public class IosSearchPath
    {
        #region Fields

        [SerializeField]
        private string m_path;

        [SerializeField]
        private bool m_applyToMainTarget;

        [SerializeField]
        private bool m_applyToFrameworkTarget = true;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the search path.
        /// </summary>
        public string Path => m_path;

        /// <summary>
        /// Gets a value indicating whether to apply to the main target.
        /// </summary>
        public bool ApplyToMainTarget => m_applyToMainTarget;

        /// <summary>
        /// Gets a value indicating whether to apply to the framework target.
        /// </summary>
        public bool ApplyToFrameworkTarget => m_applyToFrameworkTarget;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new search path entry.
        /// </summary>
        public IosSearchPath(string path = null,
                             bool applyToMainTarget = false,
                             bool applyToFrameworkTarget = true)
        {
            m_path = path;
            m_applyToMainTarget = applyToMainTarget;
            m_applyToFrameworkTarget = applyToFrameworkTarget;
        }

        #endregion

        #region Setters

        /// <summary>
        /// Sets the search path.
        /// </summary>
        public void SetPath(string path)
        {
            m_path = path;
        }

        /// <summary>
        /// Sets whether to apply to the main target.
        /// </summary>
        public void SetApplyToMainTarget(bool applyToMainTarget)
        {
            m_applyToMainTarget = applyToMainTarget;
        }

        /// <summary>
        /// Sets whether to apply to the framework target.
        /// </summary>
        public void SetApplyToFrameworkTarget(bool applyToFrameworkTarget)
        {
            m_applyToFrameworkTarget = applyToFrameworkTarget;
        }

        #endregion
    }
}
