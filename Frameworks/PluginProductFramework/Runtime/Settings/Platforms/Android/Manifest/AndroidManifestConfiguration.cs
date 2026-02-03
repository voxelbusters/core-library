using System;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework
{

    /// <summary>
    /// Holds Android manifest configuration entries.
    /// </summary>
    [Serializable]
    public class AndroidManifestConfiguration
    {
        #region Fields

        [SerializeField]
        private AndroidManifestAttribute[] m_manifestAttributes = new AndroidManifestAttribute[0];

        [SerializeField]
        private AndroidManifestAttribute[] m_applicationAttributes = new AndroidManifestAttribute[0];

        [SerializeField]
        private AndroidManifestActivity[] m_activities = new AndroidManifestActivity[0];

        [SerializeField]
        private AndroidManifestProvider[] m_providers = new AndroidManifestProvider[0];

        [SerializeField]
        private AndroidManifestService[] m_services = new AndroidManifestService[0];

        [SerializeField]
        private AndroidManifestReceiver[] m_receivers = new AndroidManifestReceiver[0];

        [SerializeField]
        private AndroidPermission[] m_permissions = new AndroidPermission[0];

        [SerializeField]
        private AndroidFeature[] m_features = new AndroidFeature[0];

        [SerializeField]
        private AndroidMetaData[] m_metaData = new AndroidMetaData[0];

        [SerializeField]
        private AndroidQueryIntent[] m_queries = new AndroidQueryIntent[0];

        #endregion

        #region Properties

        /// <summary>
        /// Gets the manifest attributes.
        /// </summary>
        public AndroidManifestAttribute[] ManifestAttributes => m_manifestAttributes;

        /// <summary>
        /// Gets the application attributes.
        /// </summary>
        public AndroidManifestAttribute[] ApplicationAttributes => m_applicationAttributes;

        /// <summary>
        /// Gets the manifest activities.
        /// </summary>
        public AndroidManifestActivity[] Activities => m_activities;

        /// <summary>
        /// Gets the manifest providers.
        /// </summary>
        public AndroidManifestProvider[] Providers => m_providers;

        /// <summary>
        /// Gets the manifest services.
        /// </summary>
        public AndroidManifestService[] Services => m_services;

        /// <summary>
        /// Gets the manifest receivers.
        /// </summary>
        public AndroidManifestReceiver[] Receivers => m_receivers;

        /// <summary>
        /// Gets the manifest permission list.
        /// </summary>
        public AndroidPermission[] Permissions => m_permissions;

        /// <summary>
        /// Gets the manifest uses-feature list.
        /// </summary>
        public AndroidFeature[] Features => m_features;

        /// <summary>
        /// Gets the manifest meta-data list.
        /// </summary>
        public AndroidMetaData[] MetaData => m_metaData;

        /// <summary>
        /// Gets the manifest query intent list.
        /// </summary>
        public AndroidQueryIntent[] Queries => m_queries;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new manifest configuration.
        /// </summary>
        public AndroidManifestConfiguration(AndroidManifestAttribute[] manifestAttributes = null,
                                            AndroidManifestAttribute[] applicationAttributes = null,
                                            AndroidManifestActivity[] activities = null,
                                            AndroidManifestProvider[] providers = null,
                                            AndroidManifestService[] services = null,
                                            AndroidManifestReceiver[] receivers = null,
                                            AndroidPermission[] permissions = null,
                                            AndroidFeature[] features = null,
                                            AndroidMetaData[] metaData = null,
                                            AndroidQueryIntent[] queries = null)
        {
            m_manifestAttributes = manifestAttributes ?? Array.Empty<AndroidManifestAttribute>();
            m_applicationAttributes = applicationAttributes ?? Array.Empty<AndroidManifestAttribute>();
            m_activities = activities ?? Array.Empty<AndroidManifestActivity>();
            m_providers = providers ?? Array.Empty<AndroidManifestProvider>();
            m_services = services ?? Array.Empty<AndroidManifestService>();
            m_receivers = receivers ?? Array.Empty<AndroidManifestReceiver>();
            m_permissions = permissions ?? Array.Empty<AndroidPermission>();
            m_features = features ?? Array.Empty<AndroidFeature>();
            m_metaData = metaData ?? Array.Empty<AndroidMetaData>();
            m_queries = queries ?? Array.Empty<AndroidQueryIntent>();
        }

        #endregion

        #region Setters

        /// <summary>
        /// Sets the manifest attributes.
        /// </summary>
        public void SetManifestAttributes(AndroidManifestAttribute[] manifestAttributes)
        {
            m_manifestAttributes = manifestAttributes ?? Array.Empty<AndroidManifestAttribute>();
        }

        /// <summary>
        /// Sets the application attributes.
        /// </summary>
        public void SetApplicationAttributes(AndroidManifestAttribute[] applicationAttributes)
        {
            m_applicationAttributes = applicationAttributes ?? Array.Empty<AndroidManifestAttribute>();
        }

        /// <summary>
        /// Sets the activity list.
        /// </summary>
        public void SetActivities(AndroidManifestActivity[] activities)
        {
            m_activities = activities ?? Array.Empty<AndroidManifestActivity>();
        }

        /// <summary>
        /// Sets the provider list.
        /// </summary>
        public void SetProviders(AndroidManifestProvider[] providers)
        {
            m_providers = providers ?? Array.Empty<AndroidManifestProvider>();
        }

        /// <summary>
        /// Sets the service list.
        /// </summary>
        public void SetServices(AndroidManifestService[] services)
        {
            m_services = services ?? Array.Empty<AndroidManifestService>();
        }

        /// <summary>
        /// Sets the receiver list.
        /// </summary>
        public void SetReceivers(AndroidManifestReceiver[] receivers)
        {
            m_receivers = receivers ?? Array.Empty<AndroidManifestReceiver>();
        }

        /// <summary>
        /// Sets the permission list.
        /// </summary>
        public void SetPermissions(AndroidPermission[] permissions)
        {
            m_permissions = permissions ?? Array.Empty<AndroidPermission>();
        }

        /// <summary>
        /// Sets the uses-feature list.
        /// </summary>
        public void SetFeatures(AndroidFeature[] features)
        {
            m_features = features ?? Array.Empty<AndroidFeature>();
        }

        /// <summary>
        /// Sets the meta-data list.
        /// </summary>
        public void SetMetaData(AndroidMetaData[] metaData)
        {
            m_metaData = metaData ?? Array.Empty<AndroidMetaData>();
        }

        /// <summary>
        /// Sets the query intent list.
        /// </summary>
        public void SetQueries(AndroidQueryIntent[] queries)
        {
            m_queries = queries ?? Array.Empty<AndroidQueryIntent>();
        }

        #endregion
    }
}
