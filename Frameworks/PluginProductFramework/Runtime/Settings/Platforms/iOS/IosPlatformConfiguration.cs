using System;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{

    /// <summary>
    /// Holds iOS-specific configuration for a feature.
    /// </summary>
    public class IosPlatformConfiguration : PlatformConfigurationObject
    {
        #region Fields

        [SerializeField]
        private IosPodDependency[] m_podDependencies = new IosPodDependency[0];

        [SerializeField]
        private IosCapability[] m_capabilities = new IosCapability[0];

        [SerializeField]
        private IosBuildProperty[] m_buildProperties = new IosBuildProperty[0];

        [SerializeField]
        private IosSearchPath[] m_headerSearchPaths = new IosSearchPath[0];

        [SerializeField]
        private IosSearchPath[] m_frameworkSearchPaths = new IosSearchPath[0];

        [SerializeField]
        private IosSearchPath[] m_librarySearchPaths = new IosSearchPath[0];

        [SerializeField]
        private IosMacroDefinition[] m_macroDefinitions = new IosMacroDefinition[0];

        [SerializeField]
        private IosFrameworkReference[] m_frameworks = new IosFrameworkReference[0];

        [SerializeField]
        private IosInfoPlistEntry[] m_infoPlistEntries = new IosInfoPlistEntry[0];

        [SerializeField]
        private IosEntitlementEntry[] m_entitlementEntries = new IosEntitlementEntry[0];

        [SerializeField]
        private IosUrlScheme[] m_urlSchemes = new IosUrlScheme[0];

        [SerializeField]
        private IosAssociatedDomain[] m_associatedDomains = new IosAssociatedDomain[0];

        #endregion

        #region Properties

        /// <summary>
        /// Gets the platform type for this configuration.
        /// </summary>
        public override PlatformType Platform => PlatformType.iOS;

        /// <summary>
        /// Gets the pod dependency list.
        /// </summary>
        public IosPodDependency[] PodDependencies => m_podDependencies;

        /// <summary>
        /// Gets the capability list.
        /// </summary>
        public IosCapability[] Capabilities => m_capabilities;

        /// <summary>
        /// Gets the build property list.
        /// </summary>
        public IosBuildProperty[] BuildProperties => m_buildProperties;

        /// <summary>
        /// Gets the header search path list.
        /// </summary>
        public IosSearchPath[] HeaderSearchPaths => m_headerSearchPaths;

        /// <summary>
        /// Gets the framework search path list.
        /// </summary>
        public IosSearchPath[] FrameworkSearchPaths => m_frameworkSearchPaths;

        /// <summary>
        /// Gets the library search path list.
        /// </summary>
        public IosSearchPath[] LibrarySearchPaths => m_librarySearchPaths;

        /// <summary>
        /// Gets the macro definitions list.
        /// </summary>
        public IosMacroDefinition[] MacroDefinitions => m_macroDefinitions;

        /// <summary>
        /// Gets the framework references list.
        /// </summary>
        public IosFrameworkReference[] Frameworks => m_frameworks;

        /// <summary>
        /// Gets the Info.plist entries list.
        /// </summary>
        public IosInfoPlistEntry[] InfoPlistEntries => m_infoPlistEntries;

        /// <summary>
        /// Gets the entitlements entries list.
        /// </summary>
        public IosEntitlementEntry[] EntitlementEntries => m_entitlementEntries;

        /// <summary>
        /// Gets the URL schemes list.
        /// </summary>
        public IosUrlScheme[] UrlSchemes => m_urlSchemes;

        /// <summary>
        /// Gets the associated domains list.
        /// </summary>
        public IosAssociatedDomain[] AssociatedDomains => m_associatedDomains;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new iOS platform configuration.
        /// </summary>
        public IosPlatformConfiguration(IosPodDependency[] podDependencies = null,
                                        IosCapability[] capabilities = null,
                                        IosBuildProperty[] buildProperties = null,
                                        IosSearchPath[] headerSearchPaths = null,
                                        IosSearchPath[] frameworkSearchPaths = null,
                                        IosSearchPath[] librarySearchPaths = null,
                                        IosMacroDefinition[] macroDefinitions = null,
                                        IosFrameworkReference[] frameworks = null,
                                        IosInfoPlistEntry[] infoPlistEntries = null,
                                        IosEntitlementEntry[] entitlementEntries = null,
                                        IosUrlScheme[] urlSchemes = null,
                                        IosAssociatedDomain[] associatedDomains = null)
        {
            m_podDependencies = podDependencies ?? Array.Empty<IosPodDependency>();
            m_capabilities = capabilities ?? Array.Empty<IosCapability>();
            m_buildProperties = buildProperties ?? Array.Empty<IosBuildProperty>();
            m_headerSearchPaths = headerSearchPaths ?? Array.Empty<IosSearchPath>();
            m_frameworkSearchPaths = frameworkSearchPaths ?? Array.Empty<IosSearchPath>();
            m_librarySearchPaths = librarySearchPaths ?? Array.Empty<IosSearchPath>();
            m_macroDefinitions = macroDefinitions ?? Array.Empty<IosMacroDefinition>();
            m_frameworks = frameworks ?? Array.Empty<IosFrameworkReference>();
            m_infoPlistEntries = infoPlistEntries ?? Array.Empty<IosInfoPlistEntry>();
            m_entitlementEntries = entitlementEntries ?? Array.Empty<IosEntitlementEntry>();
            m_urlSchemes = urlSchemes ?? Array.Empty<IosUrlScheme>();
            m_associatedDomains = associatedDomains ?? Array.Empty<IosAssociatedDomain>();
        }

        #endregion

        #region Setters

        /// <summary>
        /// Sets the pod dependency list.
        /// </summary>
        public void SetPodDependencies(IosPodDependency[] podDependencies)
        {
            m_podDependencies = podDependencies ?? Array.Empty<IosPodDependency>();
        }

        /// <summary>
        /// Sets the capability list.
        /// </summary>
        public void SetCapabilities(IosCapability[] capabilities)
        {
            m_capabilities = capabilities ?? Array.Empty<IosCapability>();
        }

        /// <summary>
        /// Sets the build property list.
        /// </summary>
        public void SetBuildProperties(IosBuildProperty[] buildProperties)
        {
            m_buildProperties = buildProperties ?? Array.Empty<IosBuildProperty>();
        }

        /// <summary>
        /// Sets the header search path list.
        /// </summary>
        public void SetHeaderSearchPaths(IosSearchPath[] headerSearchPaths)
        {
            m_headerSearchPaths = headerSearchPaths ?? Array.Empty<IosSearchPath>();
        }

        /// <summary>
        /// Sets the framework search path list.
        /// </summary>
        public void SetFrameworkSearchPaths(IosSearchPath[] frameworkSearchPaths)
        {
            m_frameworkSearchPaths = frameworkSearchPaths ?? Array.Empty<IosSearchPath>();
        }

        /// <summary>
        /// Sets the library search path list.
        /// </summary>
        public void SetLibrarySearchPaths(IosSearchPath[] librarySearchPaths)
        {
            m_librarySearchPaths = librarySearchPaths ?? Array.Empty<IosSearchPath>();
        }

        /// <summary>
        /// Sets the macro definitions list.
        /// </summary>
        public void SetMacroDefinitions(IosMacroDefinition[] macroDefinitions)
        {
            m_macroDefinitions = macroDefinitions ?? Array.Empty<IosMacroDefinition>();
        }

        /// <summary>
        /// Sets the framework references list.
        /// </summary>
        public void SetFrameworks(IosFrameworkReference[] frameworks)
        {
            m_frameworks = frameworks ?? Array.Empty<IosFrameworkReference>();
        }

        /// <summary>
        /// Sets the Info.plist entries list.
        /// </summary>
        public void SetInfoPlistEntries(IosInfoPlistEntry[] infoPlistEntries)
        {
            m_infoPlistEntries = infoPlistEntries ?? Array.Empty<IosInfoPlistEntry>();
        }

        /// <summary>
        /// Sets the entitlements entries list.
        /// </summary>
        public void SetEntitlementEntries(IosEntitlementEntry[] entitlementEntries)
        {
            m_entitlementEntries = entitlementEntries ?? Array.Empty<IosEntitlementEntry>();
        }

        /// <summary>
        /// Sets the URL schemes list.
        /// </summary>
        public void SetUrlSchemes(IosUrlScheme[] urlSchemes)
        {
            m_urlSchemes = urlSchemes ?? Array.Empty<IosUrlScheme>();
        }

        /// <summary>
        /// Sets the associated domains list.
        /// </summary>
        public void SetAssociatedDomains(IosAssociatedDomain[] associatedDomains)
        {
            m_associatedDomains = associatedDomains ?? Array.Empty<IosAssociatedDomain>();
        }

        #endregion
    }
}
