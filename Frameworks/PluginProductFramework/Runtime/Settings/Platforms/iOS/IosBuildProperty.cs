using System;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Represents a build property entry for Xcode.
    /// </summary>
    [Serializable]
    public class IosBuildProperty
    {
        #region Fields

        [SerializeField]
        private string m_key;

        [SerializeField]
        private string m_value;

        [SerializeField]
        private bool m_applyToMainTarget;

        [SerializeField]
        private bool m_applyToFrameworkTarget = true;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the build setting key.
        /// </summary>
        public string Key => m_key;

        /// <summary>
        /// Gets the build setting value.
        /// </summary>
        public string Value => m_value;

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
        /// Creates a new build property entry.
        /// </summary>
        public IosBuildProperty(string key = null,
                                string value = null,
                                bool applyToMainTarget = false,
                                bool applyToFrameworkTarget = true)
        {
            m_key = key;
            m_value = value;
            m_applyToMainTarget = applyToMainTarget;
            m_applyToFrameworkTarget = applyToFrameworkTarget;
        }

        #endregion

        #region Setters

        /// <summary>
        /// Sets the build setting key.
        /// </summary>
        public void SetKey(string key)
        {
            m_key = key;
        }

        /// <summary>
        /// Sets the build setting value.
        /// </summary>
        public void SetValue(string value)
        {
            m_value = value;
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
