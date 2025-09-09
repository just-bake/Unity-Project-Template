// System
using System;

// Unity
using UnityEngine;
using UnityEngine.SceneManagement;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Watch;

// GUPS - AntiCheat
using GUPS.AntiCheat.Monitor.Time;

namespace GUPS.AntiCheat.Detector
{
    /// <summary>
    /// This detector is used to detect game time (UnityEngine.Time) cheating. It observes the game time monitor and subscribes to time deviations,
    /// based on this it calculates the possibility of cheating and notifies observers of the detected cheating. Additionally, it starts doing counter measures
    /// by calculating the game time based on system ticks, if a cheating got detected. So even if cheated, the game time will be calculated correctly and applied
    /// in GUPS.AntiCheat.Protected.Time.ProtectedTime.
    /// </summary>
    public class GameTimeCheatingDetector : ADetector
    {
        // Name
        #region Name

        /// <summary>
        /// The name of the detector.
        /// </summary>
        public override String Name => "Game Time Cheating Detector";

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
        /// The possibility of false positive reachs from 0.0 to 1.0. The game time monitoring is not so reliable, 
        /// because of possible hickups of the game itself. So the possibility of false positive is set to 0.45.
        /// </summary>
        public float PossibilityOfFalsePositive { get; private set; } = 0.45f;

        /// <summary>
        /// The threat rating of this detector. It is set to a low value, because false positives are likely and a high amount of possible cheating detections will be send to the monitor (Recommended: 25).
        /// </summary>
        [SerializeField]
        [Header("Threat Rating - Settings")]
        [Tooltip("The threat rating of this detector. It is set to a low value, because false positives are likely and a high amount of possible cheating detections will be send to the monitor (Recommended: 25).")]
        private uint threatRating = 25;

        /// <summary>
        /// The threat rating of this detector. It is set to a low value, because false positives are likely and a high amount of possible cheating detections will be send to the monitor (Recommended: 25).
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
        /// When notified by the game time monitor, record the deviation and notify observers of the detected cheating.
        /// </summary>
        /// <param name="_Subject"></param>
        public override void OnNext(IWatchedSubject _Subject)
        {
            // Only notify observers if the detector is active.
            if (this.IsActive)
            {
                // Only react to the game time status.
                if (_Subject is GameTimeStatus var_GameTimeStatus)
                {
                    // If game time cheating already got detected, do not react to any further notifications.
                    if(this.PossibleCheatingDetected)
                    {
                        // Do nothing. Cheating already detected.
                    }
                    else
                    {
                        // Record the deviation.
                        if (this.Record(var_GameTimeStatus.Deviation))
                        {
                            // Set the cheating detected flag.
                            this.PossibleCheatingDetected = true;
                        }

                        // If no deviation got detected, do nothing...
                        if (var_GameTimeStatus.Deviation == ETimeDeviation.None)
                        {
                            return;
                        }

                        // ...else notify the observers (normally the AntiCheatMonitor) of the detected deviation.
                        this.Notify(new CheatingDetectionStatus(this.PossibilityOfFalsePositive, this.ThreatRating));

                        // Notify event listeners of the detected deviation.
                        this.OnCheatingDetectionEvent?.Invoke(new CheatingDetectionStatus(this.PossibilityOfFalsePositive, this.ThreatRating));
                    }
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

        // Game Time Monitor
        #region Game Time Monitor

        /// <summary>
        /// The game time monitor to observe.
        /// </summary>
        private GameTimeMonitor gameTimeMonitor;

        #endregion

        // Lifecycle
        #region Lifecycle

        /// <summary>
        /// On awake register to the game time monitor on the same game object.
        /// </summary>
        protected override void Awake()
        {
            // Call the base method.
            base.Awake();

            // Get the game time monitor on the same game object.
            this.gameTimeMonitor = this.GetComponent<GameTimeMonitor>();

            // If the game time monitor is not found, log an error.
            if (this.gameTimeMonitor == null)
            {
                UnityEngine.Debug.LogWarning("GameTimeCheatingDetector requires a GameTimeMonitor to be present on the same game object.");
                return;
            }

            // Observe the game time monitor.
            this.gameTimeMonitor.Subscribe(this);

            // Set the values array size.
            this.values = new ETimeDeviation[this.maxHistorySize];
        }

        protected virtual void Update()
        {
            // Update the time.
            this.UpdateTime();
        }

        /// <summary>
        /// Called when the application loses or gains focus, resetting time.
        /// </summary>
        /// <param name="_Focus">True if the application gains focus, false if it loses focus.</param>
        private void OnApplicationFocus(bool _Focus)
        {
            // Reset Utc Time
            this.ResetUtcTime();
        }

        /// <summary>
        /// Called when the application is paused or unpaused, resetting time.
        /// </summary>
        /// <param name="_Pause">True if the application is paused, false if it is unpaused.</param>
        private void OnApplicationPause(bool _Pause)
        {
            // Reset Utc Time
            this.ResetUtcTime();
        }

        /// <summary>
        /// On a new level got loaded, reset the time since level loaded time.
        /// </summary>
        /// <param name="_Scene">The loaded scene.</param>
        /// <param name="_Mode">The scene loading mode.</param>
        protected virtual void OnLevelFinishedLoading(Scene _Scene, LoadSceneMode _Mode)
        {
            // Reset level time.
            this.ResetLevelTime();
        }

        #endregion

        // History
        #region History

        /// <summary>
        /// The max count of last deviation values to store.
        /// </summary>
        private int maxHistorySize = 25;

        /// <summary>
        /// Stores the last deviation values.
        /// </summary>
        private ETimeDeviation[] values = new ETimeDeviation[0];

        /// <summary>
        /// Add a new deviation value to the history and return if there is any possible time cheating.
        /// </summary>
        /// <param name="_Value">The deviation value to add.</param>
        /// <returns>Returns true if there is any possible time cheating.</returns>
        private bool Record(ETimeDeviation _Value)
        {
            // Count the type of deviation.
            int var_None = 0;
            int var_Stopped = 0;
            int var_SlowedDown = 0;
            int var_SpeedUp = 0;

            // Push new values to the end of the array and shift the array to the left.
            for (int i = 0; i < this.maxHistorySize; i++)
            {
                // Push to the left.
                if (i < this.maxHistorySize - 1)
                {
                    this.values[i] = this.values[i + 1];
                }
                // Store the current value.
                else
                {
                    this.values[i] = _Value;
                }

                // Count the type of deviation.
                switch (this.values[i])
                {
                    case ETimeDeviation.None:
                        var_None++;
                        break;
                    case ETimeDeviation.Stopped:
                        var_Stopped++;
                        break;
                    case ETimeDeviation.SlowedDown:
                        var_SlowedDown++;
                        break;
                    case ETimeDeviation.SpeedUp:
                        var_SpeedUp++;
                        break;
                }
            }

            // Return if there is any possible time cheating.
            return var_Stopped >= this.maxHistorySize / 2 || var_SlowedDown >= this.maxHistorySize / 2 || var_SpeedUp >= this.maxHistorySize / 2;
        }

        #endregion

        // Time
        #region Time

        // Device Time
        private long previousUtcTime;

        // Unity Time
        private float time;
        private float deltaTime;
        private float unscaledTime;
        private float unscaledDeltaTime;
        private float realtimeSinceStartup;
        private float timeSinceLevelLoad;

        internal float Time { get { return this.PossibleCheatingDetected ? this.time : UnityEngine.Time.time; } set { this.time = value; } }
        internal float DeltaTime { get { return this.PossibleCheatingDetected ? this.deltaTime : UnityEngine.Time.deltaTime; } set { this.deltaTime = value; } }
        internal float UnscaledTime { get { return this.PossibleCheatingDetected ? this.unscaledTime : UnityEngine.Time.unscaledTime; } set { this.unscaledTime = value; } }
        internal float UnscaledDeltaTime { get { return this.PossibleCheatingDetected ? this.unscaledDeltaTime : UnityEngine.Time.unscaledDeltaTime; } set { this.unscaledDeltaTime = value; } }
        internal float RealtimeSinceStartup { get { return this.PossibleCheatingDetected ? this.realtimeSinceStartup : UnityEngine.Time.realtimeSinceStartup; } set { this.realtimeSinceStartup = value; } }
        internal float TimeSinceLevelLoad { get { return this.PossibleCheatingDetected ? this.timeSinceLevelLoad : UnityEngine.Time.timeSinceLevelLoad; } set { this.timeSinceLevelLoad = value; } }
        internal float TimeScale { get { return UnityEngine.Time.timeScale; } }

        /// <summary>
        /// Update the unity time.
        /// </summary>
        private void UpdateTime()
        {
            // Calculate the passed ticks.
            long var_UtcTimeNow = DateTime.UtcNow.Ticks;
            long var_SpanUtcTime = var_UtcTimeNow - this.previousUtcTime;

            // Set the previous time to the current time.
            this.previousUtcTime = var_UtcTimeNow;

            // If a cheat got detected calculate time based on system ticks.
            if (this.PossibleCheatingDetected)
            {
                // Speeding got detected, so calculate time based on system ticks.
                this.unscaledDeltaTime = TickToSec(var_SpanUtcTime);
                this.unscaledTime += this.unscaledDeltaTime;
                this.realtimeSinceStartup += this.unscaledDeltaTime;

                this.deltaTime = this.unscaledDeltaTime * this.TimeScale;
                this.time += this.deltaTime;
                this.timeSinceLevelLoad += this.deltaTime;
            }
            else
            {
                // No cheat / hack got detected, so synchronize calculated time with unity time.
                this.time = UnityEngine.Time.time;
                this.unscaledTime = UnityEngine.Time.unscaledTime;
                this.deltaTime = UnityEngine.Time.deltaTime;
                this.unscaledDeltaTime = UnityEngine.Time.unscaledDeltaTime;
                this.realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
                this.timeSinceLevelLoad = UnityEngine.Time.timeSinceLevelLoad;
            }
        }

        /// <summary>
        /// Reset the time since level loaded.
        /// </summary>
        private void ResetLevelTime()
        {
            // Reset Level Time
            this.timeSinceLevelLoad = 0.0f;
        }

        /// <summary>
        /// Reset the previous utc time to the current utc time.
        /// </summary>
        private void ResetUtcTime()
        {
            // Reset Utc Time
            this.previousUtcTime = DateTime.UtcNow.Ticks;
        }

        /// <summary>
        /// Convert CPU ticks to seconds.
        /// </summary>
        /// <param name="_Tick"></param>
        /// <returns></returns>
        private static float TickToSec(long _Tick)
        {
            return Convert.ToSingle(_Tick) / TimeSpan.TicksPerSecond;
        }

        #endregion
    }
}