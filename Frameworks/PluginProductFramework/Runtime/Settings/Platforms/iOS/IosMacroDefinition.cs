using System;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Represents a macro definition entry.
    /// </summary>
    [Serializable]
    public class IosMacroDefinition
    {
        #region Fields

        [SerializeField]
        private string m_name;

        [SerializeField]
        private string m_value;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the macro name.
        /// </summary>
        public string Name => m_name;

        /// <summary>
        /// Gets the macro value.
        /// </summary>
        public string Value => m_value;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new macro definition entry.
        /// </summary>
        public IosMacroDefinition(string name = null, string value = null)
        {
            m_name = name;
            m_value = value;
        }

        #endregion

        #region Setters

        /// <summary>
        /// Sets the macro name.
        /// </summary>
        public void SetName(string name)
        {
            m_name = name;
        }

        /// <summary>
        /// Sets the macro value.
        /// </summary>
        public void SetValue(string value)
        {
            m_value = value;
        }

        #endregion
    }
}
