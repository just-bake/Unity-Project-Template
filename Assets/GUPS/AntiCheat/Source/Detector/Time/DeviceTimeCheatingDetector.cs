// System
using System;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;

// Unity
using UnityEngine;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Watch;

// GUPS - AntiCheat
using GUPS.AntiCheat.Monitor.Time;

namespace GUPS.AntiCheat.Detector
{
    /// <summary>
    /// This detector is used to detect device or system time manipulation. It observes the device time monitor and subscribes to time deviations, 
    /// based on this it calculates the possibility of cheating and notifies observers of the detected cheating. It also provides a trustworthy 
    /// DateTime.UtcNow either calculated based on the internet time or device time, provided in the ProtectedTime class.
    /// </summary>
    public class DeviceTimeCheatingDetector : ADetector
    {
        // Name
        #region Name

        /// <summary>
        /// The name of the detector.
        /// </summary>
        public override String Name => "Device Time Cheating Detector";

        #endregion

        // Platform
        #region Platform

        /// <summary>
        /// Is supported on all platforms.
        /// </summary>
        public override bool IsSupported => true;

        #endregion

        // Threat Rating
        #region Threat Rating

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
        public CheatingDetectionEvent<CheatingDetectionStatus> OnCheatingDetectionEvent = new CheatingDetectionEvent<CheatingDetectionStatus>();

        #endregion

        // Observer
        #region Observer

        /// <summary>
        /// When notified by the device time monitor, notify observers of the detected cheating.
        /// </summary>
        /// <param name="_Subject"></param>
        public override void OnNext(IWatchedSubject _Subject)
        {
            // Only notify observers if the detector is active.
            if (this.IsActive)
            {
                // Only react to the device time status.
                if (_Subject is DeviceTimeStatus var_DeviceTimeStatus)
                {
                    // If no deviation is detected, return.
                    if(var_DeviceTimeStatus.Deviation == ETimeDeviation.None)
                    {
                        return;
                    }

                    // Possible cheating detected.
                    this.PossibleCheatingDetected = true;

                    // Notify observers (mostly the AntiCheatMonitor) of the detected deviation.
                    this.Notify(new CheatingDetectionStatus(0.35f, this.ThreatRating));

                    // Notify event listeners of the detected deviation.
                    this.OnCheatingDetectionEvent?.Invoke(new CheatingDetectionStatus(0.35f, this.ThreatRating));
                }
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

        // Device Time Monitor
        #region Device Time Monitor

        /// <summary>
        /// The device time monitor to observe.
        /// </summary>
        private DeviceTimeMonitor deviceTimeMonitor;

        #endregion

        // Device Time
        #region Device Time

        /// <summary>
        /// Determines whether the detector uses internet time for fetching the application start time.
        /// </summary>
        [SerializeField]
        [Header("Device Time - Settings")]
        [Tooltip("Determines whether the detector uses internet time for fetching the application start time.")]
        private bool useInternetTime = true;

        /// <summary>
        /// Gets whether the monitor uses internet time for synchronization.
        /// </summary>
        public bool UseInternetTime { get => this.useInternetTime; }

        /// <summary>
        /// Shared HTTP client for time synchronization.
        /// </summary>
        private HttpClient client;

        /// <summary>
        /// The address of the server used to fetch the current utc time.
        /// </summary>
        [SerializeField]
        [Tooltip("The address of the server used to fetch the current utc time.")]
        private string serverAddress = "https://google.com";

        /// <summary>
        /// Hash of the server certificate used for validation (Optional, but enhances security).
        /// </summary>
        [SerializeField]
        [Tooltip("Hash of the server X509 certificate used for validation (Optional, but enhances security).")]
        private string serverCertificateHash = null;

        /// <summary>
        /// The application start time, either from the internet or device time.
        /// </summary>
        private DateTime? applicationStartDateTime =  null;

        /// <summary>
        /// The unity calculated real time since the application started in seconds.
        /// </summary>
        private float applicationStartRealTimeSinceStartup = 0;

        /// <summary>
        /// The calculated utc time, which may differs from the original DateTime.UtcNow because it is calculated to be secure and trustable as possible.
        /// </summary>
        public DateTime CurrentUtcTime = DateTime.UtcNow;

        /// <summary>
        /// Asynchronously calculates the time to be compared, either from the internet or device time.
        /// </summary>
        private async Task<DateTime> CalculateCompareTime()
        {
            if (this.UseInternetTime)
            {
                return await this.GetInternetCompareTime();
            }
            else
            {
                return DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Asynchronously retrieves the compare time from an internet server.
        /// </summary>
        private async Task<DateTime> GetInternetCompareTime()
        {
            try
            {
                // If there is no client, create one.
                if (this.client == null)
                {
                    // Create a custom handler to check the certificate (Optional, but prevents users from tampering the response).
                    var var_Handler = new HttpClientHandler();
                    var_Handler.ServerCertificateCustomValidationCallback = (var_HttpRequestMessage, var_Certificate, var_Chain, var_PolicyError) =>
                    {
                        if (String.IsNullOrEmpty(this.serverCertificateHash))
                        {
                            return true;
                        }

                        return this.serverCertificateHash.Equals(var_Certificate.GetCertHashString());
                    };
                    var_Handler.CheckCertificateRevocationList = true;

                    // Create the client.
                    this.client = new HttpClient(var_Handler);

                    // Set the timeout to 5 seconds.
                    this.client.Timeout = TimeSpan.FromSeconds(5);
                }

                // Get the response from the server.
                var var_Response = await this.client.GetAsync(this.serverAddress, HttpCompletionOption.ResponseHeadersRead);
                return var_Response.Headers.Date.HasValue ? var_Response.Headers.Date.Value.UtcDateTime : DateTime.UtcNow;
            }
            catch (HttpRequestException var_HttpRequestException)
            {
                // Check if it's an SSL certificate validation issue
                if (var_HttpRequestException.InnerException is HttpRequestException var_InnerException &&
                    var_InnerException.InnerException is AuthenticationException var_SSLException)
                {
                    // Throw an exception to notify the user about the possible tampering.
                    throw new AuthenticationException("SSL certificate validation failed. Possible tampering!", var_SSLException);
                }
                return DateTime.UtcNow;
            }
            catch
            {
                return DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Calculates the application start time.
        /// </summary>
        private void GetApplicationStartTime()
        {
            // Get the device utc time before the request.
            DateTime var_DeviceUtcTime_Pre = DateTime.UtcNow;

            // Fetch the utc time, synchronously.
            var var_TimeTask = Task.Run(async () => { return await this.CalculateCompareTime(); });
            var_TimeTask.Wait();

            // Get the device utc time after the request.
            DateTime var_DeviceUtcTime_Post = DateTime.UtcNow;

            // Calculate the time difference between the pre and post device utc time.
            float var_DeviceUtcTimeDifference = (float)(var_DeviceUtcTime_Post - var_DeviceUtcTime_Pre).TotalSeconds;

            // Store and adjust the compare utc time by adding the half of device utc time difference.
            this.applicationStartDateTime = var_TimeTask.Result.AddSeconds(var_DeviceUtcTimeDifference / 2f);

            // Store the real time since startup.
            this.applicationStartRealTimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
        }

        #endregion

        // Lifecycle
        #region Lifecycle

        /// <summary>
        /// On awake register to the device time monitor on the same game object.
        /// </summary>
        protected override void Awake()
        {
            // Call the base method.
            base.Awake();

            // Get the device time monitor on the same game object.
            this.deviceTimeMonitor = this.GetComponent<DeviceTimeMonitor>();

            // If the device time monitor is not found, log an error.
            if (this.deviceTimeMonitor == null)
            {
                UnityEngine.Debug.LogWarning("DeviceTimeCheatingDetector requires a DeviceTimeMonitor to be present on the same game object.");
                return;
            }

            // Observe the device time monitor.
            this.deviceTimeMonitor.Subscribe(this);

            // If the application start time is not set, calculate it now.
            if (this.applicationStartDateTime == null)
            {
                this.GetApplicationStartTime();
            }
        }

        /// <summary>
        /// On unity fixed update, calculate the current utc time.
        /// </summary>
        protected virtual void FixedUpdate()
        {
            // If the application start time is not set, calculate it now.
            if (this.applicationStartDateTime == null)
            {
                this.GetApplicationStartTime();
            }

            // Calculate the current utc time.
            this.CurrentUtcTime = this.applicationStartDateTime.Value.AddSeconds(UnityEngine.Time.realtimeSinceStartup - this.applicationStartRealTimeSinceStartup);
        }

        /// <summary>
        /// When the application gains focus again after losing it, the application time gets recalculated.
        /// </summary>
        /// <param name="_HasFocus">True if the application gains focus, false if it loses focus.</param>
        protected virtual void OnApplicationFocus(bool _HasFocus)
        {
            // On gain focus, recalculate the application start time.
            if (_HasFocus)
            {
                this.GetApplicationStartTime();
            }
        }

        /// <summary>
        /// When the application is unpaused after being paused,the application time gets recalculated.
        /// </summary>
        /// <param name="_IsPaused">True if the application is paused, false if it is unpaused.</param>
        protected virtual void OnApplicationPause(bool _IsPaused)
        {
            // On unpause, recalculate the application start time.
            if (!_IsPaused)
            {
                this.GetApplicationStartTime();
            }
        }

        #endregion
    }
}