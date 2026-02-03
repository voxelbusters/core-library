using System;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{

    /// <summary>
    /// Holds Android-specific configuration for a feature.
    /// </summary>
    public class AndroidPlatformConfiguration : PlatformConfigurationObject
    {
        #region Fields

        [SerializeField]
        private AndroidManifestConfiguration m_manifestConfiguration = new AndroidManifestConfiguration();

        [SerializeField]
        private AndroidGradleDependency[] m_dependencies = new AndroidGradleDependency[0];

        #endregion

        #region Properties

        /// <summary>
        /// Gets the platform type for this configuration.
        /// </summary>
        public override PlatformType Platform => PlatformType.Android;

        /// <summary>
        /// Gets the Android manifest configuration.
        /// </summary>
        public AndroidManifestConfiguration ManifestConfiguration => m_manifestConfiguration;

        /// <summary>
        /// Gets the Gradle dependency list.
        /// </summary>
        public AndroidGradleDependency[] Dependencies => m_dependencies;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new Android platform configuration.
        /// </summary>
        public AndroidPlatformConfiguration(AndroidManifestConfiguration manifestConfiguration = null,
                                            AndroidGradleDependency[] dependencies = null)
        {
            m_manifestConfiguration = manifestConfiguration ?? new AndroidManifestConfiguration();
            m_dependencies = dependencies ?? Array.Empty<AndroidGradleDependency>();
        }

        #endregion

        #region Setters

        /// <summary>
        /// Sets the manifest configuration.
        /// </summary>
        public void SetManifestConfiguration(AndroidManifestConfiguration manifestConfiguration)
        {
            m_manifestConfiguration = manifestConfiguration ?? new AndroidManifestConfiguration();
        }

        /// <summary>
        /// Sets the Gradle dependency list.
        /// </summary>
        public void SetDependencies(AndroidGradleDependency[] dependencies)
        {
            m_dependencies = dependencies ?? Array.Empty<AndroidGradleDependency>();
        }

        #endregion
    }
}
