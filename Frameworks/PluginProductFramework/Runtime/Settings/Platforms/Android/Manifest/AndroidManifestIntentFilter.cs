using System;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Represents an intent filter entry.
    /// </summary>
    [Serializable]
    public class AndroidManifestIntentFilter
    {
        #region Fields

        [SerializeField]
        private string m_label;

        [SerializeField]
        private bool m_autoVerify;

        [SerializeField]
        private string[] m_actions = new string[0];

        [SerializeField]
        private string[] m_categories = new string[0];

        [SerializeField]
        private AndroidManifestData[] m_data = new AndroidManifestData[0];

        #endregion

        #region Properties

        /// <summary>
        /// Gets the intent filter label.
        /// </summary>
        public string Label => m_label;

        /// <summary>
        /// Gets a value indicating whether autoVerify is enabled.
        /// </summary>
        public bool AutoVerify => m_autoVerify;

        /// <summary>
        /// Gets the action list.
        /// </summary>
        public string[] Actions => m_actions;

        /// <summary>
        /// Gets the category list.
        /// </summary>
        public string[] Categories => m_categories;

        /// <summary>
        /// Gets the data list.
        /// </summary>
        public AndroidManifestData[] Data => m_data;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new intent filter entry.
        /// </summary>
        public AndroidManifestIntentFilter(string label = null,
                                           bool autoVerify = false,
                                           string[] actions = null,
                                           string[] categories = null,
                                           AndroidManifestData[] data = null)
        {
            m_label = label;
            m_autoVerify = autoVerify;
            m_actions = actions ?? Array.Empty<string>();
            m_categories = categories ?? Array.Empty<string>();
            m_data = data ?? Array.Empty<AndroidManifestData>();
        }

        #endregion

        #region Setters

        /// <summary>
        /// Sets the intent filter label.
        /// </summary>
        public void SetLabel(string label)
        {
            m_label = label;
        }

        /// <summary>
        /// Sets whether autoVerify is enabled.
        /// </summary>
        public void SetAutoVerify(bool autoVerify)
        {
            m_autoVerify = autoVerify;
        }

        /// <summary>
        /// Sets the action list.
        /// </summary>
        public void SetActions(string[] actions)
        {
            m_actions = actions ?? Array.Empty<string>();
        }

        /// <summary>
        /// Sets the category list.
        /// </summary>
        public void SetCategories(string[] categories)
        {
            m_categories = categories ?? Array.Empty<string>();
        }

        /// <summary>
        /// Sets the data list.
        /// </summary>
        public void SetData(AndroidManifestData[] data)
        {
            m_data = data ?? Array.Empty<AndroidManifestData>();
        }

        #endregion
    }
}
