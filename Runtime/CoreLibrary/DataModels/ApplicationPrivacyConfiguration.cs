using UnityEngine;
using System.Collections;

namespace VoxelBusters.CoreLibrary
{
    public class ApplicationPrivacyConfiguration
    {
        #region Properties

        public ApplicationTrackingConsentStatus ConsentStatus { get; private set; }

        public string Version { get; private set; }

        #endregion

        #region Constructors

        public ApplicationPrivacyConfiguration(ApplicationTrackingConsentStatus consentStatus, string version)
        {
            // Set properties
            ConsentStatus   = consentStatus;
            Version         = version;
        }

        #endregion

    }
}