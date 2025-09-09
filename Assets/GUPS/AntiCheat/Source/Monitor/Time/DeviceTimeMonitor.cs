// System
using System;

// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Monitor.Time
{
    /// <summary>
    /// Monitors the device time and notifies observers about time deviations during the application lifecycle events.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="DeviceTimeMonitor"/> class validates the device time against the game time during key application events,
    /// such as regaining focus or resuming from pause. If any time deviations are detected, it notifies observers through the
    /// implementation of the <see cref="AMonitor"/> class.
    /// </para>
    /// </remarks>
    public class DeviceTimeMonitor : AMonitor
    {
        // Name
        #region Name

        /// <summary>
        /// The name of the Monitor.
        /// </summary>
        public override String Name => "Device Time Monitor";

        #endregion

        // Time Deviation
        #region Time Deviation

        /// <summary>
        /// Tolerance level in seconds for allowed time deviation until an actual deviation got detected (Recommend: 3-10).
        /// </summary>
        [SerializeField]
        [Header("Time Deviation - Settings")]
        [Tooltip("Tolerance level in seconds for allowed time deviation until an actual deviation got detected (Recommend: 3-10).")]
        private float tolerance = 5f;

        /// <summary>
        /// The previous date time when the application lost focus.
        /// </summary>
        private DateTime? previousUnFocusDateTime = null;

        /// <summary>
        /// The previous real time since startup when the application lost focus.
        /// </summary>
        private float previousUnFocusRealtimeSinceStartup;

        /// <summary>
        /// The previous date time when the application was paused.
        /// </summary>
        private DateTime? previousPauseDateTime = null;

        /// <summary>
        /// The previous real time since startup when the application was paused.
        /// </summary>
        private float previousPauseRealtimeSinceStartup;

        /// <summary>
        /// Determines the time deviation based on passed device and game time.
        /// </summary>
        /// <param name="_PassedDeviceTime">Time passed in the device time calculation.</param>
        /// <param name="_PassedGameTime">Time passed in the game time calculation.</param>
        /// <returns>An <see cref="ETimeDeviation"/> indicating the type of time deviation.</returns>
        private ETimeDeviation GetTimeDeviation(float _PassedDeviceTime, float _PassedGameTime)
        {
            // Check if there is a deviation...
            if (Math.Abs(_PassedDeviceTime - _PassedGameTime) < this.tolerance)
            {
                return ETimeDeviation.None;
            }

            // ... if so, check what kind of deviation it is.
            if (_PassedDeviceTime <= 0.001f)
            {
                return ETimeDeviation.Stopped;
            }
            else if (_PassedDeviceTime < _PassedGameTime)
            {
                return ETimeDeviation.SlowedDown;
            }
            else if (_PassedDeviceTime > _PassedGameTime)
            {
                return ETimeDeviation.SpeedUp;
            }
            return ETimeDeviation.None;
        }

        #endregion

        // Lifecycle
        #region Lifecycle

        /// <summary>
        /// When the application gains focus again after losing it, the device time will be validated. If there are any deviations, the observers will be notified.
        /// </summary>
        /// <param name="_HasFocus">True if the application gains focus, false if it loses focus.</param>
        protected virtual void OnApplicationFocus(bool _HasFocus)
        {
            if (_HasFocus)
            {
                // On focus, calculate the time difference.
                if (this.previousUnFocusDateTime.HasValue)
                {
                    // Calculate the time difference.
                    var var_PassedDeviceTime = DateTime.UtcNow - this.previousUnFocusDateTime.Value;
                    var var_PassedGameTime = UnityEngine.Time.realtimeSinceStartup - this.previousUnFocusRealtimeSinceStartup;

                    // Calculate the time deviation.
                    ETimeDeviation var_TimeDeviation = this.GetTimeDeviation((float)var_PassedDeviceTime.TotalSeconds, var_PassedGameTime);

                    // Notify the observers.
                   this.Notify(new DeviceTimeStatus(var_TimeDeviation));

                    // Reset the previous unfocus date time.
                    this.previousUnFocusDateTime = null;
                }
            }
            else
            {
                // On focus lost, store the current date time.
                this.previousUnFocusDateTime = DateTime.UtcNow;

                // Store the current real time since startup.
                this.previousUnFocusRealtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
            }
        }

        /// <summary>
        /// When the application is unpaused after being paused, the device time will be validated. If there are any deviations, the observers will be notified.
        /// </summary>
        /// <param name="_IsPaused">True if the application is paused, false if it is unpaused.</param>
        protected virtual void OnApplicationPause(bool _IsPaused)
        {
            if (_IsPaused)
            {
                // On pause, store the current date time.
                this.previousPauseDateTime = DateTime.UtcNow;

                // Store the current real time since startup.
                this.previousPauseRealtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
            }
            else
            {
                // On resume, calculate the time difference.
                if (this.previousUnFocusDateTime.HasValue)
                {
                    // Calculate the time difference.
                    var var_PassedDeviceTime = DateTime.UtcNow - this.previousPauseDateTime.Value;
                    var var_PassedGameTime = UnityEngine.Time.realtimeSinceStartup - this.previousPauseRealtimeSinceStartup;

                    // Calculate the time deviation.
                    ETimeDeviation var_TimeDeviation = this.GetTimeDeviation((float)var_PassedDeviceTime.TotalSeconds, var_PassedGameTime);

                    // Notify the observers.
                    this.Notify(new DeviceTimeStatus(var_TimeDeviation));

                    // Reset the previous pause date time.
                    this.previousPauseDateTime = null;
                }
            }
        }

        #endregion
    }
}
