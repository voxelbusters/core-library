using UnityEngine;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Base class for product-wide resources settings.
    /// </summary>
    public abstract class PluginProductResourcesSettingsBase : ScriptableObject
    {
        #region Fields

        [SerializeField]
        private string m_documentation;

        [SerializeField]
        private string m_tutorials;

        [SerializeField]
        private string m_forum;

        [SerializeField]
        private string m_support;

        [SerializeField]
        private string m_writeReview;

        #endregion

        #region Properties

        public string Documentation => m_documentation;

        public string Tutorials => m_tutorials;

        public string Forum => m_forum;

        public string Support => m_support;

        public string WriteReview => m_writeReview;

        #endregion
    }
}
