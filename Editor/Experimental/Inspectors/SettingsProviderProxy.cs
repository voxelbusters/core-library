using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.Editor.Experimental
{
    public class SettingsProviderProxy : SettingsProvider
    {
        #region Constants

        private     const       string      kDefaultInstallMessageFormat        = "In order to use {0} system you need to install the {0} package. Clicking the button below will install {0} package and allow you to configure.";

        private     const       string      kDefaultProjectSettingsPathFormat   = "Project/Voxel Busters/{0}";            

        // install path
        private     const       string      kInstallUrlSnapchat                 = "https://u3d.as/1gWc";

        private     const       string      kInstallUrlInstagram                = "https://u3d.as/1pMn";

        private     const       string      kInstallUrlEssentialKit             = "https://u3d.as/1szE";

        private     const       string      kInstallUrlReportingKit             = "https://u3d.as/2Q6p";

        private     const       string      kInstallUrlMLKit                    = "https://u3d.as/2PMe";

        private     const       string      kInstallUrlRecorderKit              = "http://u3d.as/1nN3";

        #endregion

        #region Fields

        private     string      m_installMessage;

        private     string      m_installButtonLabel;

        private     string      m_installUrl;

        #endregion

        #region Constructors

        public SettingsProviderProxy(string name, string installUrl,
            string path = null, SettingsScope scopes = SettingsScope.Project)
            : base(path ?? string.Format(kDefaultProjectSettingsPathFormat, name), scopes)
        {
            // set properties
            m_installMessage        = string.Format(kDefaultInstallMessageFormat, name);
            m_installButtonLabel    = $"Install {name}";
            m_installUrl            = installUrl;
        }

        #endregion

        #region Create methods

#if !ENABLE_VOXELBUSTERS_SNAPCHAT_KIT
        [SettingsProvider]
        private static SettingsProvider CreateSnapchatKitSettingsProvider()
        {
            return new SettingsProviderProxy(
                name: "Snapchat Kit",
                installUrl: kInstallUrlSnapchat);
        }
#endif

#if !ENABLE_VOXELBUSTERS_INSTAGRAM_KIT
        [SettingsProvider]
        private static SettingsProvider CreateInstagramKitSettingsProvider()
        {
            return new SettingsProviderProxy(
                name: "Instagram Kit",
                installUrl: kInstallUrlInstagram);
        }
#endif

#if !ENABLE_VOXELBUSTERS_ESSENTIAL_KIT
        [SettingsProvider]
        private static SettingsProvider CreateEssentialKitSettingsProvider()
        {
            return new SettingsProviderProxy(
                name: "Essential Kit",
                installUrl: kInstallUrlEssentialKit);
        }
#endif

#if !ENABLE_VOXELBUSTERS_ML_KIT
        [SettingsProvider]
        private static SettingsProvider CreateMLKitSettingsProvider()
        {
            return new SettingsProviderProxy(
                name: "ML Kit",
                installUrl: kInstallUrlMLKit);
        }
#endif

#if !ENABLE_VOXELBUSTERS_REPORTING_KIT
        [SettingsProvider]
        private static SettingsProvider CreateReportingKitSettingsProvider()
        {
            return new SettingsProviderProxy(
                name: "Reporting Kit",
                installUrl: kInstallUrlReportingKit);
        }
#endif

#if !ENABLE_VOXELBUSTERS_RECORDER_KIT
        [SettingsProvider]
        private static SettingsProvider CreateRecorderKitSettingsProvider()
        {
            return new SettingsProviderProxy(
                name: "Recorder Kit",
                installUrl: kInstallUrlRecorderKit);
        }
#endif

        #endregion

        #region Base class methods

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.HelpBox(m_installMessage, MessageType.Info);
            if (GUILayout.Button(m_installButtonLabel))
            {
                Application.OpenURL(m_installUrl);
            }
        }

        #endregion
    }
}