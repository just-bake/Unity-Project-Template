// System
using System;

// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Monitor.Time
{
    /// <summary>
    /// Monitors the actual game time (UnityEngine.Time), calculating and notifying observers about possible game time deviations, possibly caused by cheating.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="GameTimeMonitor"/> class monitors the game time, calculating the deviation between 'Game Time' and 'Real Time'. It records the history 
    /// of delta time values and calculates the mean values to determine whether there is a significant deviation. If a deviation is detected, it notifies 
    /// observers about the specific type of detected possible deviation.
    /// </para>
    /// </remarks>
    public class GameTimeMonitor : AMonitor
    {
        // Name
        #region Name

        /// <summary>
        /// The name of the Monitor.
        /// </summary>
        public override String Name => "Game Time Monitor";

        #endregion

        // Time Deviation
        #region Time Deviation

        /// <summary>
        /// Allowed time difference in milliseconds between 'Game Time' and 'Real Time' until possible deviation get monitored (Recommend: 10-20).
        /// </summary>
        [SerializeField]
        [Range(1f, 5000.0f)]
        [Header("Time Deviation - Settings")]
        [Tooltip("Allowed time difference in milliseconds between 'Game Time' and 'Real Time' until possible deviation get monitored (Recommend: 10-20).")]
        private float tolerance = 10f;

        /// <summary>
        /// The count of last deviation values, used to calculate the mean value.
        /// </summary>
        private int historySize = 0;

        /// <summary>
        /// The max count of last deviation values to store.
        /// </summary>
        private int maxHistorySize = 25;

        /// <summary>
        /// List of the last 'maxHistorySize' delta time values.
        /// </summary>
        private float[] deltaTimeValues = new float[0];

        /// <summary>
        /// The mean value of the last 'maxHistorySize' delta time values.
        /// </summary>
        private float deltaTimeValueMean = 0;

        /// <summary>
        /// Utc time of previous tick.
        /// </summary>
        private long previousUtcTime;

        /// <summary>
        /// List of the last 'maxHistorySize' utc time values.
        /// </summary>
        private float[] utcTimeValues = new float[0];

        /// <summary>
        /// The mean value of the last 'maxHistorySize' utc time values.
        /// </summary>
        private float utcTimeValueMean = 0;

        /// <summary>
        /// Resets the internal state of the <see cref="GameTimeMonitor"/>.
        /// </summary>
        private void Reset()
        {
            // Reset history size.
            this.historySize = 0;

            // Reset time values.
            this.deltaTimeValues = new float[this.maxHistorySize];
            this.utcTimeValues = new float[this.maxHistorySize];

            // Reset time mean values.
            this.deltaTimeValueMean = 0;
            this.utcTimeValueMean = 0;

            // Reset the utc time ticks.
            this.previousUtcTime = DateTime.UtcNow.Ticks;
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
        /// Records a new deviation value, updating the internal array and calculating the mean value.
        /// </summary>
        /// <param name="_Value">The new deviation value to record.</param>
        /// <param name="_ValueArray">The array of previous deviation values.</param>
        /// <returns>The mean value of the updated deviation values.</returns>
        private float Record(float _Value, float[] _ValueArray)
        {
            // Store the mean value.
            float var_MeanValue = 0;

            // Push new values to the end of the array and shift the array to the left.
            for (int i = 0; i < this.maxHistorySize; i++)
            {
                // Push to the left.
                if (i < this.maxHistorySize - 1)
                {
                    _ValueArray[i] = _ValueArray[i + 1];
                }
                // Store the current value.
                else
                {
                    _ValueArray[i] = _Value;
                }

                // Increase the mean value.
                var_MeanValue += _ValueArray[i];
            }

            // Find the mean value.
            return var_MeanValue / this.maxHistorySize;
        }

        /// <summary>
        /// Gets the type of time deviation based on the mean deviation values.
        /// </summary>
        /// <returns>The type of time deviation.</returns>
        private ETimeDeviation GetTimeDeviation()
        {
            // Check if there is a deviation...
            if (Math.Abs(this.deltaTimeValueMean - this.utcTimeValueMean) < this.tolerance / 1000.0f)
            {
                return ETimeDeviation.None;
            }

            // ... if so, check what kind of deviation it is.
            if (this.deltaTimeValueMean <= 0.0001f)
            {
                return ETimeDeviation.Stopped;
            }
            else if (this.deltaTimeValueMean < this.utcTimeValueMean)
            {
                return ETimeDeviation.SlowedDown;
            }
            else if (this.deltaTimeValueMean > this.utcTimeValueMean)
            {
                return ETimeDeviation.SpeedUp;
            }
            return ETimeDeviation.None;
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

        // Lifecycle
        #region Lifecycle

        /// <summary>
        /// Called when the monitor is started, resetting time values.
        /// </summary>
        protected override void OnStart()
        {
            // Reset time values.
            this.Reset();
        }

        /// <summary>
        /// Called when the monitor is resumed, resetting time values.
        /// </summary>
        protected override void OnResume()
        {
            // Reset time values.
            this.Reset();
        }

        /// <summary>
        /// Called on each frame update, calculating and notifying observers about possible time deviations.
        /// </summary>
        protected override void OnUpdate()
        {
            // Calculate the passed ticks.
            long var_UtcTimeNow = DateTime.UtcNow.Ticks;
            long var_SpanUtcTime = var_UtcTimeNow - this.previousUtcTime;

            // Reset Utc previous tick.
            this.previousUtcTime = var_UtcTimeNow;

            // Record utc delta time.
            this.utcTimeValueMean = this.Record(TickToSec(var_SpanUtcTime) * UnityEngine.Time.timeScale, this.utcTimeValues);

            // Record unity delta time.
            this.deltaTimeValueMean = this.Record(UnityEngine.Time.deltaTime, this.deltaTimeValues);

            // Increase history size.
            this.historySize++;

            // Check if the history size is bigger than the max history size, then notify watchers.
            if (this.historySize > this.maxHistorySize)
            {
                // Create a new notification status.
                GameTimeStatus var_GameTimeStatus = new GameTimeStatus(this.GetTimeDeviation());

                // Notify watchers.
                this.Notify(var_GameTimeStatus);

                // Reset time values.
                this.Reset();
            }
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

        #endregion
    }
}
