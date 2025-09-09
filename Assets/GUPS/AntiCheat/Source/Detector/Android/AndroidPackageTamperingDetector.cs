// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;
using UnityEngine.Networking;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Watch;

// GUPS - AntiCheat
using GUPS.AntiCheat.Monitor.Android;
using GUPS.AntiCheat.Settings;

namespace GUPS.AntiCheat.Detector.Android
{
    /// <summary>
    /// This detector is used to detect if the Android package (APK/AAB) is tampered with. If so it will notify the observers of the detected tampering.
    /// </summary>
    /// <remarks>
    /// <para>
    /// On awake the detector will register to the android package monitors on the same game object. These are the <see cref="AndroidPackageSourceMonitor"/>,
    /// <see cref="AndroidPackageHashMonitor"/>, <see cref="AndroidPackageFingerprintMonitor"/>, <see cref="AndroidPackageLibraryMonitor"/>. If the monitors are 
    /// not found on the same game object, the detector will not subscribe to them.
    /// </para>
    /// <para>
    /// The detector observes the android package monitors. If there are any indications of tampering, the detector will notify its observers (mostly the 
    /// AntiCheat-Monitor) about the detected tampering, so you can react to it. The detector will also notify the event listeners through the 
    /// <see cref="OnCheatingDetectionEvent"/>.
    /// </para>
    /// </remarks>
    public class AndroidPackageTamperingDetector : ADetector
    {
        // Name
        #region Name

        /// <summary>
        /// The name of the detector.
        /// </summary>
        public override String Name => "Android Package Tampering Detector";

        #endregion

        // Platform
        #region Platform

        /// <summary>
        /// Is supported only for Android platforms.
        /// </summary>
#if UNITY_ANDROID
        public override bool IsSupported => true;
#else
        public override bool IsSupported => false;
#endif

        #endregion

        // Threat Rating
        #region Threat Rating

        /// <summary>
        /// The possibility of a false positive is very low.
        /// </summary>
        public float PossibilityOfFalsePositive => 0.01f;

        /// <summary>
        /// The threat rating of this detector. It is set to a very high value, because false positives are very unlikely and the impact of cheating is very high (Recommended: 500).
        /// </summary>
        [SerializeField]
        [Header("Threat Rating - Settings")]
        [Tooltip("The threat rating of this detector. It is set to a very high value, because false positives are very unlikely and the impact of cheating is very high (Recommended: 500).")]
        private uint threatRating = 500;

        /// <summary>
        /// The threat rating of this detector. It is set to a very high value, because false positives are very unlikely and the impact of cheating is very high (Recommended: 500).
        /// </summary>
        public override uint ThreatRating { get => this.threatRating; protected set => this.threatRating = value; }

        /// <summary>
        /// Stores whether a cheating got detected.
        /// </summary>
        public override bool PossibleCheatingDetected { get; protected set; } = false;

        #endregion

        // Observable
        #region Observable

        /// <summary>
        /// A unity event that is used to subscribe to the cheating detection events. It is useful if you do not want to write custom observers to subscribe to the detectors and 
        /// simply attach a callback to the detector event through the inspector.
        /// </summary>
        [Header("Observable - Settings")]
        [Tooltip("A unity event that is used to subscribe to the cheating detection events. It is useful if you do not want to write custom observers to subscribe to the detectors and simply attach a callback to the detector event through the inspector.")]
        public CheatingDetectionEvent<AndroidCheatingDetectionStatus> OnCheatingDetectionEvent = new CheatingDetectionEvent<AndroidCheatingDetectionStatus>();

        #endregion

        // Observer
        #region Observer

        /// <summary>
        /// The detector observes android package monitors. If there are any indications of tampering, the detector will notify the observers.
        /// </summary>
        /// <param name="_Subject"></param>
        public override void OnNext(IWatchedSubject _Subject)
        {
            // Only react if the detector is active.
            if (!this.IsActive)
            {
                return;
            }

            // Only react to android status.
            if (!(_Subject is IAndroidStatus var_AndroidStatus))
            {
                return;
            }

#if DEVELOPMENT_BUILD || UNITY_EDITOR

            // If the current build is a development build or editor, and the validation of development builds on android is deactivated, return here.
            if (!GlobalSettings.Instance.Android_Enable_Development)
            {
                return;
            }

#endif

            // Validate each status using a coroutine.
            if (_Subject is AndroidSourceStatus var_SourceStatus)
            {
                // Validate the allowed installation sources.
                this.StartCoroutine(this.ValidatePackageSource(var_SourceStatus));
            }
            else if(_Subject is AndroidHashStatus var_HashStatus)
            {
                // Validate the hash.
                this.StartCoroutine(this.ValidatePackageHash(var_HashStatus));
            }
            else if(_Subject is AndroidFingerprintStatus var_FingerprintStatus)
            {
                // Validate the fingerprint.
                this.StartCoroutine(this.ValidatePackageFingerprint(var_FingerprintStatus));
            }
            else if(_Subject is AndroidLibraryStatus var_LibraryStatus)
            {
                // Validate the library.
                this.StartCoroutine(this.ValidatePackageLibrary(var_LibraryStatus));
            }
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="_Error">Error to handle.</param>
        public override void OnError(Exception _Error)
        {
            // Does nothing.
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public override void OnCompleted()
        {
            // Does nothing.
        }

        #endregion

        // Lifecycle
        #region Lifecycle

        /// <summary>
        /// On awake register to the android package monitors on the same game object.
        /// </summary>
        protected override void Awake()
        {
            // Call the base method.
            base.Awake();

            // Get the android package source monitor on the same game object.
            this.packageSourceMonitor = this.GetComponent<AndroidPackageSourceMonitor>();

            // If the android package source monitor is not found, do not subscribe to it.
            if (this.packageSourceMonitor != null)
            {
                // Observe the android package source monitor.
                this.packageSourceMonitor.Subscribe(this);
            }

            // Get the android package hash monitor on the same game object.
            this.packageHashMonitor = this.GetComponent<AndroidPackageHashMonitor>();

            // If the android package hash monitor is not found, do not subscribe to it.
            if (this.packageHashMonitor != null)
            {
                // Observe the android package hash monitor.
                this.packageHashMonitor.Subscribe(this);
            }

            // Get the android package fingerprint monitor on the same game object.
            this.packageFingerprintMonitor = this.GetComponent<AndroidPackageFingerprintMonitor>();

            // If the android package fingerprint monitor is not found, do not subscribe to it.
            if (this.packageFingerprintMonitor != null)
            {
                // Observe the android package fingerprint monitor.
                this.packageFingerprintMonitor.Subscribe(this);
            }

            // Get the android package library monitor on the same game object.
            this.packageLibraryMonitor = this.GetComponent<AndroidPackageLibraryMonitor>();

            // If the android package library monitor is not found, do not subscribe to it.
            if (this.packageLibraryMonitor != null)
            {
                // Observe the android package library monitor.
                this.packageLibraryMonitor.Subscribe(this);
            }
        }

        #endregion

        // Package Tampering
        #region Package Tampering

        /// <summary>
        /// When comparing the hash of the app with the remote hash, the version parameter in the url is replaced with the current version of the app.
        /// </summary>
        private const String CHashVersionParameter = "{version}";

        /// <summary>
        /// The android package source monitor.
        /// </summary>
        private AndroidPackageSourceMonitor packageSourceMonitor;

        /// <summary>
        /// The android package hash monitor.
        /// </summary>
        private AndroidPackageHashMonitor packageHashMonitor;

        /// <summary>
        /// The android package fingerprint monitor.
        /// </summary>
        private AndroidPackageFingerprintMonitor packageFingerprintMonitor;

        /// <summary>
        /// The android package library monitor.
        /// </summary>
        private AndroidPackageLibraryMonitor packageLibraryMonitor;

        /// <summary>
        /// A shared method to notify observers of the detected tampering.
        /// </summary>
        /// <param name="_AndroidCheatingType">The type of cheating that got detected.</param>
        /// <param name="_FailedToRetrieveData">The monitor notifing the detector failed to retrieve its data or an exception occurred.</param>
        private void OnDetectCheating(EAndroidCheatingType _AndroidCheatingType, bool _FailedToRetrieveData)
        {
            // Possible cheating detected.
            this.PossibleCheatingDetected = true;

            // Create a new detection status.
            AndroidCheatingDetectionStatus var_DetectionStatus = new AndroidCheatingDetectionStatus(_FailedToRetrieveData ? 0.75f : this.PossibilityOfFalsePositive, this.ThreatRating, _AndroidCheatingType, _FailedToRetrieveData);

            // Notify observers (mostly the AntiCheatMonitor) of the detection.
            this.Notify(var_DetectionStatus);

            // Notify event listeners of the detection.
            this.OnCheatingDetectionEvent?.Invoke(var_DetectionStatus);
        }

        /// <summary>
        /// Validate the package installation source using the settings from the global settings.
        /// </summary>
        /// <param name="_SourceStatus">The source status to validate.</param>
        /// <returns>A coroutine enumerator to validate the package installation source.</returns>
        private IEnumerator ValidatePackageSource(AndroidSourceStatus _SourceStatus)
        {
            // If all app stores are allowed, return.
            if(GlobalSettings.Instance.Android_AllowAllAppStores)
            {
                // Log a debug info that all app stores are allowed.
                Debug.Log(String.Format("[GUPS][AntiCheat] All app stores are allowed. The installation source '{0}' is allowed!", _SourceStatus.AppStoreSource.ToString()));

                yield break;
            }

            // If the source status failed to retrieve data, log a warning and notify observers.
            if (_SourceStatus.FailedToRetrieveData)
            {
                // Log a warning info that the installation source could not be retrieved.
                Debug.LogWarning("[GUPS][AntiCheat] The installation source could not be retrieved!");

                // The installation source could not be retrieved - Notify observers.
                this.OnDetectCheating(EAndroidCheatingType.PACKAGE_SOURCE, true);

                yield break;
            }

            // If the app store is allowed, return.
            if (GlobalSettings.Instance.Android_AllowedAppStores.Contains(_SourceStatus.AppStoreSource))
            {
                // Log a debug info that the installation source is allowed.
                Debug.Log(String.Format("[GUPS][AntiCheat] The installation source '{0}' is allowed!", _SourceStatus.AppStoreSource.ToString()));

                yield break;
            }

            // If the app store is in the allowed custom app stores, return.
            if(GlobalSettings.Instance.Android_AllowedCustomAppStores.Contains(_SourceStatus.AppStoreSourcePackage))
            {
                // Log a debug info that the installation source is allowed.
                Debug.Log(String.Format("[GUPS][AntiCheat] The installation source '{0}' is allowed!", _SourceStatus.AppStoreSourcePackage));

                yield break;
            }

            // Log a warning info that the installation source is not valid.
            Debug.LogWarning(String.Format("[GUPS][AntiCheat] The installation source '{0}' is not allowed!", _SourceStatus.AppStoreSource.ToString()));

            // Else the installation source is not valid - Notify observers.
            this.OnDetectCheating(EAndroidCheatingType.PACKAGE_SOURCE, false);
        }

        /// <summary>
        /// Validate the package hash using the settings from the global settings.
        /// </summary>
        /// <param name="_HashStatus">The hash status to validate.</param>
        /// <returns>A coroutine enumerator to validate the package hash.</returns>
        private IEnumerator ValidatePackageHash(AndroidHashStatus _HashStatus)
        {
            if(!GlobalSettings.Instance.Android_VerifyAppHash)
            {
                // Log a debug info that the app hash verification is disabled.
                Debug.Log(String.Format("[GUPS][AntiCheat] The app hash verification is disabled. The hash '{0}' is not validated!", _HashStatus.Hash));

                yield break;
            }

            // If the hash status failed to retrieve data, log a warning and notify observers.
            if (_HashStatus.FailedToRetrieveData)
            {
                // Log a warning info that the hash could not be retrieved.
                Debug.LogWarning("[GUPS][AntiCheat] The app hash could not be retrieved!");

                // The hash could not be retrieved - Notify observers.
                this.OnDetectCheating(EAndroidCheatingType.PACKAGE_HASH, true);

                yield break;
            }

            // Get the endpoint from the gloabl settings to request the hash from.
            String var_HashEndpoint = GlobalSettings.Instance.Android_AppHashEndpoint;

            // Replace the version parameter in the url with the current version (Application.version) of the app. Application.version returns the current version of
            // the Application. To set the version number in Unity, go to Edit > Project Settings > Player. This is the same as PlayerSettings.bundleVersion.
            var_HashEndpoint = var_HashEndpoint.ToLower().Replace(CHashVersionParameter, Application.version);

            // Send a request to the server to get the hash as hex-string.
            using (UnityWebRequest var_Request = UnityWebRequest.Get(var_HashEndpoint))
            {
                // Send the request and wait for the response.
                var var_RequestWaiter = var_Request.SendWebRequest();

                yield return var_RequestWaiter;

                // If the request was not successful, log the error and return.
                if (var_Request.result != UnityWebRequest.Result.Success)
                {
                    // Log an error info that the request was not successful.
                    Debug.LogError(String.Format("[GUPS][AntiCheat] Failed to request hash from server '{0}' with error: {1}", var_HashEndpoint, var_Request.error));

                    // The hash could not be retrieved - Notify observers.
                    this.OnDetectCheating(EAndroidCheatingType.PACKAGE_HASH, true);

                    yield break;
                }

                // Get the content of the response.
                String var_DownloadedHash = var_Request.downloadHandler.text;

                // Trim the downloaded hash.
                var_DownloadedHash = var_DownloadedHash?.Trim() ?? "";

                // Remove common separators from the downloaded hash.
                var_DownloadedHash = var_DownloadedHash.Replace("-", "").Replace(" ", "").Replace(":", "");

                // Trim the compare hash.
                String var_CompareHash = _HashStatus.Hash?.Trim() ?? "";

                // Remove common separators from the compare hash.
                var_CompareHash = var_CompareHash.Replace("-", "").Replace(" ", "").Replace(":", "");

                // If the downloaded content is not empty and the hash is valid, return.
                if (!String.IsNullOrEmpty(var_DownloadedHash) && var_DownloadedHash.Equals(var_CompareHash, StringComparison.OrdinalIgnoreCase))
                {
                    // Log a debug info that the hash is valid.
                    Debug.Log(String.Format("[GUPS][AntiCheat] The app hash '{0}' is equals to the remote hash read from endpoint '{1}'!", var_CompareHash, var_HashEndpoint));

                    yield break;
                }

                // Log a warning info that the hash is not valid containing the received hash.
                Debug.LogWarning(String.Format("[GUPS][AntiCheat] The app hash '{0}' is not equals to the remote hash '{1}' read from endpoint '{2}'!", var_CompareHash, var_DownloadedHash, var_HashEndpoint));
            }

            // The hash is not valid - Notify observers.
            this.OnDetectCheating(EAndroidCheatingType.PACKAGE_HASH, false);
        }

        /// <summary>
        /// Validate the package fingerprint using the settings from the global settings.
        /// </summary>
        /// <param name="_FingerprintStatus">The fingerprint status to validate.</param>
        /// <returns>A coroutine enumerator to validate the package fingerprint.</returns>
        private IEnumerator ValidatePackageFingerprint(AndroidFingerprintStatus _FingerprintStatus)
        {
            // If the app fingerprint verification is disabled, return.
            if(!GlobalSettings.Instance.Android_VerifyAppFingerprint)
            {
                // Log a debug info that the app fingerprint verification is disabled.
                Debug.Log(String.Format("[GUPS][AntiCheat] The app fingerprint verification is disabled. The fingerprint '{0}' is not validated!", _FingerprintStatus.Fingerprint));

                yield break;
            }

            // If the fingerprint status failed to retrieve data, log a warning and notify observers.
            if (_FingerprintStatus.FailedToRetrieveData)
            {
                // Log a warning info that the fingerprint could not be retrieved.
                Debug.LogWarning("[GUPS][AntiCheat] The app fingerprint could not be retrieved!");

                // The fingerprint could not be retrieved - Notify observers.
                this.OnDetectCheating(EAndroidCheatingType.PACKAGE_FINGERPRINT, true);

                yield break;
            }

            // Trim the settings stored fingerprint.
            String var_SettingsFingerprint = GlobalSettings.Instance.Android_AppFingerprint?.Trim() ?? "";

            // Remove common separators from the settings fingerprint.
            var_SettingsFingerprint = var_SettingsFingerprint.Replace("-", "").Replace(" ", "").Replace(":", "");

            // Trim the calculated compare fingerprint.
            String var_CompareHash = _FingerprintStatus.Fingerprint?.Trim() ?? "";

            // Remove common separators from the compare fingerprint.
            var_CompareHash = var_CompareHash.Replace("-", "").Replace(" ", "").Replace(":", "");

            // If the app fingerprint is valid, return.
            if (var_SettingsFingerprint.Equals(var_CompareHash, StringComparison.OrdinalIgnoreCase))
            {
                // Log a debug info that the fingerprint is valid.
                Debug.Log(String.Format("[GUPS][AntiCheat] The app fingerprint '{0}' is equals to the expected fingerprint!", var_CompareHash));

                yield break;
            }

            // Log a warning info that the fingerprint is not valid.
            Debug.LogWarning(String.Format("[GUPS][AntiCheat] The app fingerprint '{0}' is not equals to the expected fingerprint '{1}'!", var_CompareHash, var_SettingsFingerprint));

            // The fingerprint is not valid - Notify observers.
            this.OnDetectCheating(EAndroidCheatingType.PACKAGE_FINGERPRINT, false);
        }

        /// <summary>
        /// Validate the package library using the settings from the global settings.
        /// </summary>
        /// <param name="_LibraryStatus"The library status to validate.></param>
        /// <returns>A coroutine enumerator to validate the package library.</returns>
        private IEnumerator ValidatePackageLibrary(AndroidLibraryStatus _LibraryStatus)
        {
            // If the library validation is disabled, return.
            if (!GlobalSettings.Instance.Android_UseWhitelistingForLibraries && !GlobalSettings.Instance.Android_UseBlacklistingforApplication)
            {
                // Log a debug info that the library validation is disabled.
                Debug.Log("[GUPS][AntiCheat] The library validation is disabled. The libraries are not validated!");

                yield break;
            }

            // If whitelisting or blacklisting is enabled, but the libraries could not be retrieved, log a warning and notify observers.
            if (_LibraryStatus.FailedToRetrieveData)
            {
                // Log a warning info that the libraries could not be retrieved.
                Debug.LogWarning("[GUPS][AntiCheat] The libraries could not be retrieved!");

                // The libraries could not be retrieved - Notify observers.
                this.OnDetectCheating(EAndroidCheatingType.PACKAGE_LIBRARY, true);

                yield break;
            }

            // Stores if the libraries are tampered with.
            bool var_TamperedLibraries = false;

            // If whitelisting is enabled, compare the app libraries with the whitelisted libraries. If a library included in the app is not whitelisted, notify observer.
            if (GlobalSettings.Instance.Android_UseWhitelistingForLibraries)
            {
                // Clone the passed package libraries in a tmp list.
                List<String> var_PackageLibraries = new List<String>(_LibraryStatus.Libraries);

                // Clone the whitelisted libraries in a tmp list.
                List<String> var_WhitelistedLibraries = new List<String>(GlobalSettings.Instance.Android_WhitelistedLibraries);

                // Remove all libraries that are not whitelisted from the tmp list.
                for(int w = 0; w < var_WhitelistedLibraries.Count; w++)
                {
                    for (int p = 0; p < var_PackageLibraries.Count; p++)
                    {
                        if (var_WhitelistedLibraries[w].Equals(var_PackageLibraries[p], StringComparison.OrdinalIgnoreCase))
                        {
                            var_PackageLibraries.RemoveAt(p);

                            p -= 1;
                        }
                    }
                }

                // If the tmp list is not empty, notify observer.
                if (var_PackageLibraries.Count > 0)
                {
                    // Log all libraries that are not whitelisted in the console.
                    foreach(String var_Library in var_PackageLibraries)
                    {
                        Debug.LogWarning(String.Format("[GUPS][AntiCheat] The library '{0}' is not whitelisted!", var_Library));
                    }

                    // There are libraries that are not whitelisted!
                    var_TamperedLibraries = true;
                }
            }

            // If whitelisting is enabled, also validate the blacklisted, compare the app libraries with the blacklisted libraries. If a library included in the app is blacklisted, notify observer
            if (GlobalSettings.Instance.Android_UseWhitelistingForLibraries)
            {
                // Clone the passed package libraries in a tmp list.
                List<String> var_PackageLibraries = new List<String>(_LibraryStatus.Libraries);

                // Clone the blacklisted libraries in a tmp list.
                List<String> var_BlacklistedLibraries = new List<String>(GlobalSettings.Instance.Android_BlacklistedLibraries);

                // A temporary list to store the found blacklisted libraries.
                List<string> var_FoundBlacklistedLibraries = new List<string>();

                // Iterate over all blacklisted libraries.
                for (int b = 0; b < var_BlacklistedLibraries.Count; b++)
                {
                    // Iterate over all package libraries.
                    for (int p = 0; p < var_PackageLibraries.Count; p++)
                    {
                        // If the package library is blacklisted, add it to the found blacklisted libraries list.
                        if (var_BlacklistedLibraries[b].Equals(var_PackageLibraries[p], StringComparison.OrdinalIgnoreCase))
                        {
                            var_FoundBlacklistedLibraries.Add(var_PackageLibraries[p]);
                        }
                    }
                }

                // If the found blacklisted libraries list is not empty, notify observer.
                if (var_FoundBlacklistedLibraries.Count > 0)
                {
                    // Log all libraries that are blacklisted in the console.
                    foreach (String var_Library in var_FoundBlacklistedLibraries)
                    {
                        Debug.LogWarning(String.Format("[GUPS][AntiCheat] The library '{0}' is blacklisted!", var_Library));
                    }

                    // There are libraries that are blacklisted!
                    var_TamperedLibraries = true;
                }
            }

            // If the libraries are not tampered with, return.
            if(!var_TamperedLibraries)
            {
                // Log a debug info that the libraries are not tampered with.
                Debug.Log(String.Format("[GUPS][AntiCheat] Found the expected libraries. The libraries are not tampered with!"));

                yield break;
            }

            // The libraries are tampered with - Notify observers.
            this.OnDetectCheating(EAndroidCheatingType.PACKAGE_LIBRARY, false);
        }

        #endregion
    }
}