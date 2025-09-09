// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Watch;

// GUPS - AntiCheat
using GUPS.AntiCheat.Monitor.Android;
using GUPS.AntiCheat.Settings;

namespace GUPS.AntiCheat.Detector.Android
{
    /// <summary>
    /// This detector is used to detect if the Android device has unallowed applications installed which can be used to cheat in the game. If so it will notify the 
    /// observers about the unallowed applications.
    /// </summary>
    /// <remarks>
    /// <para>
    /// On awake the detector will register to the android device monitors on the same game object. Currently it is only the <see cref="AndroidInstalledApplicationMonitor"/>. 
    /// If the monitors are not found on the same game object, the detector will not subscribe to them.
    /// </para>
    /// <para>
    /// The detector observes the android device monitors. If there are any indications of cheating, the detector will notify its observers (mostly the 
    /// AntiCheat-Monitor) about the detected cheating, so you can react to it. The detector will also notify the event listeners through the 
    /// <see cref="OnCheatingDetectionEvent"/>.
    /// </para>
    /// </remarks>
    public class AndroidDeviceCheatingDetector : ADetector
    {
        // Name
        #region Name

        /// <summary>
        /// The name of the detector.
        /// </summary>
        public override String Name => "Android Device Cheating Detector";

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
            if (!(_Subject is IAndroidStatus))
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
            if (_Subject is AndroidInstalledApplicationStatus applicationStatus)
            {
                // Validate for unallowed applications.
                this.StartCoroutine(this.ValidateDeviceApplications(applicationStatus));
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
        /// On awake register to the android device monitors on the same game object.
        /// </summary>
        protected override void Awake()
        {
            // Call the base method.
            base.Awake();

            // Get the android device installed application monitor on the same game object.
            this.installedApplicationMonitor = this.GetComponent<AndroidInstalledApplicationMonitor>();

            // If the android device installed application monitor is not found, do not subscribe to it.
            if (this.installedApplicationMonitor != null)
            {
                // Observe the android device installed application monitor.
                this.installedApplicationMonitor.Subscribe(this);
            }
        }

        #endregion

        // Device Apps
        #region

        /// <summary>
        /// The android device installed application monitor.
        /// </summary>
        private AndroidInstalledApplicationMonitor installedApplicationMonitor;

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
        /// Validate if the device has unallowed (blacklisted) applications installed. If so, notify the observers.
        /// </summary>
        /// <param name="_ApplicationStatus">The android installed application status.</param>
        /// <returns>A coroutine enumerator to validate the device applications.</returns>
        private IEnumerator ValidateDeviceApplications(AndroidInstalledApplicationStatus _ApplicationStatus)
        {
            // If blacklisting is enabled, compare the device apps with the blacklisted apps. If an app installed that is blacklisted, notify observer.
            if (!GlobalSettings.Instance.Android_UseBlacklistingforApplication)
            {
                // Log that blacklisting is disabled.
                Debug.Log(String.Format("[GUPS][AntiCheat] Blacklisting of applications is disabled!"));

                yield break;
            }

            // If the monitor failed to retrieve the data, notify the observers.
            if (_ApplicationStatus.FailedToRetrieveData)
            {
                // Log that the device applications could not be retrieved.
                Debug.LogWarning("[GUPS][AntiCheat] The installed applications on the device could not be retrieved!");

                // Failed to retrieved installed apps - Notify observers.
                this.OnDetectCheating(EAndroidCheatingType.DEVICE_INSTALLED_APPS, true);

                yield break;
            }

            // Stores if there are apps that are blacklisted.
            bool var_UnallowedApps = false;

            // Get the found unallowed applications.
            List<String> var_FoundBlacklistedApplications = new List<String>(_ApplicationStatus.Applications);

            // If the found blacklisted apps list is not empty, notify observer.
            if (var_FoundBlacklistedApplications.Count > 0)
            {
                // Log all libraries that are blacklisted in the console.
                foreach (String var_App in var_FoundBlacklistedApplications)
                {
                    Debug.LogWarning(String.Format("[GUPS][AntiCheat] The installed app '{0}' is blacklisted!", var_App));
                }

                // There are apps that are blacklisted!
                var_UnallowedApps = true;
            }

            // If there are unallowed apps, notify the observers.
            if (!var_UnallowedApps)
            {
                // Log that no unallowed apps are found.
                Debug.Log(String.Format("[GUPS][AntiCheat] No unallowed applications found on the device!"));

                yield break;
            }

            // There are blacklisted installed apps - Notify observers. Usually the AntiCheatMonitor.
            this.OnDetectCheating(EAndroidCheatingType.DEVICE_INSTALLED_APPS, false);
        }

        #endregion
    }
}