using System;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{
    /// <summary>
    /// Represents a Gradle dependency entry.
    /// </summary>
    [Serializable]
    public class AndroidGradleDependency
    {
        #region Fields

        [SerializeField]
        private string m_group;

        [SerializeField]
        private string m_artifact;

        [SerializeField]
        private string m_version;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the dependency group identifier.
        /// </summary>
        public string Group => m_group;

        /// <summary>
        /// Gets the dependency artifact identifier.
        /// </summary>
        public string Artifact => m_artifact;

        /// <summary>
        /// Gets the dependency version string.
        /// </summary>
        public string Version => m_version;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new Gradle dependency entry.
        /// </summary>
        public AndroidGradleDependency(string group = null, string artifact = null, string version = null)
        {
            m_group = group;
            m_artifact = artifact;
            m_version = version;
        }

        #endregion

        #region Setters

        /// <summary>
        /// Sets the dependency group identifier.
        /// </summary>
        public void SetGroup(string group)
        {
            m_group = group;
        }

        /// <summary>
        /// Sets the dependency artifact identifier.
        /// </summary>
        public void SetArtifact(string artifact)
        {
            m_artifact = artifact;
        }

        /// <summary>
        /// Sets the dependency version string.
        /// </summary>
        public void SetVersion(string version)
        {
            m_version = version;
        }

        #endregion
    }
}
