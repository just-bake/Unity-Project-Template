// System
using System;

// Unity
using UnityEngine;

// GUPS - AntiCheat
using GUPS.AntiCheat.Detector;

namespace GUPS.AntiCheat.Protected.Time
{
    /// <summary>
    /// Represents a set of protected time-related properties and methods, safeguarded against cheating. Use this ProtectedTime class instead of UnityEngine.Time 
    /// or System.DateTime to access time-related values while protecting against cheating.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="ProtectedTime"/> class provides access to various time-related properties while protecting against cheating. It incorporates detection 
    /// mechanisms to ensure the integrity of time-related values and prevents manipulation by external tools or hacks.
    /// </para>
    /// <para>
    /// The class includes properties for accessing delta time, unscaled delta time, time scale, time, unscaled time, time since level load, realtime since 
    /// startup, and Coordinated Universal Time (UTC). These properties are backed by detectors from the <see cref="AntiCheatMonitor"/> to detect and mitigate 
    /// cheating attempts.
    /// </para>
    /// </remarks>
    public sealed class ProtectedTime
    {
        /// <summary>
        /// The detector for the game time cheating.
        /// </summary>
        private static GameTimeCheatingDetector gameTimeCheatingDetector;

        /// <summary>
        /// The detector for the device time cheating.
        /// </summary>
        private static DeviceTimeCheatingDetector deviceTimeCheatingDetector;

        /// <summary>
        /// The protected time in seconds it took to complete the last frame (Read Only).
        /// </summary>
        public static float deltaTime
        {
            get
            {
                // Get the detector from the AntiCheatMonitor.
                if(gameTimeCheatingDetector == null)
                {
                    gameTimeCheatingDetector = AntiCheatMonitor.Instance.GetDetector<GameTimeCheatingDetector>();
                }

                // If the detector is still null, return the default...
                if (gameTimeCheatingDetector == null)
                {
                    return UnityEngine.Time.deltaTime;
                }

                // ...otherwise return the protected value.
                return gameTimeCheatingDetector.DeltaTime;
            }
        }

        /// <summary>
        /// The protected timeScale-independent interval in seconds from the last frame to the current one (Read Only).
        /// </summary>
        public static float unscaledDeltaTime
        {
            get
            {
                // Get the detector from the AntiCheatMonitor.
                if (gameTimeCheatingDetector == null)
                {
                    gameTimeCheatingDetector = AntiCheatMonitor.Instance.GetDetector<GameTimeCheatingDetector>();
                }

                // If the detector is still null, return the default...
                if (gameTimeCheatingDetector == null)
                {
                    return UnityEngine.Time.unscaledDeltaTime;
                }

                // ...otherwise return the protected value.
                return gameTimeCheatingDetector.UnscaledDeltaTime;
            }
        }

        /// <summary>
        /// The protected scale at which the time is passing. This can be used for slow motion effects.
        /// </summary>
        public static float timeScale
        {
            get
            {
                return UnityEngine.Time.timeScale;
            }
            set
            {
                UnityEngine.Time.timeScale = value;
            }
        }

        /// <summary>
        /// The protected time at the beginning of this frame (Read Only). This is the time in seconds since the start of the game.
        /// </summary>
        public static float time
        {
            get
            {
                // Get the detector from the AntiCheatMonitor.
                if (gameTimeCheatingDetector == null)
                {
                    gameTimeCheatingDetector = AntiCheatMonitor.Instance.GetDetector<GameTimeCheatingDetector>();
                }

                // If the detector is still null, return the default...
                if (gameTimeCheatingDetector == null)
                {
                    return UnityEngine.Time.time;
                }

                // ...otherwise return the protected value.
                return gameTimeCheatingDetector.Time;
            }
        }

        /// <summary>
        /// The protected timeScale-independent time for this frame (Read Only). This is the time in seconds since the start of the game.
        /// </summary>
        public static float unscaledTime
        {
            get
            {
                // Get the detector from the AntiCheatMonitor.
                if (gameTimeCheatingDetector == null)
                {
                    gameTimeCheatingDetector = AntiCheatMonitor.Instance.GetDetector<GameTimeCheatingDetector>();
                }

                // If the detector is still null, return the default...
                if (gameTimeCheatingDetector == null)
                {
                    return UnityEngine.Time.unscaledTime;
                }

                // ...otherwise return the protected value.
                return gameTimeCheatingDetector.UnscaledTime;
            }
        }

        /// <summary>
        /// The protected time this frame has started (Read Only). This is the time in seconds since the last level has been loaded.
        /// </summary>
        public static float timeSinceLevelLoad
        {
            get
            {
                // Get the detector from the AntiCheatMonitor.
                if (gameTimeCheatingDetector == null)
                {
                    gameTimeCheatingDetector = AntiCheatMonitor.Instance.GetDetector<GameTimeCheatingDetector>();
                }

                // If the detector is still null, return the default...
                if (gameTimeCheatingDetector == null)
                {
                    return UnityEngine.Time.timeSinceLevelLoad;
                }

                // ...otherwise return the protected value.
                return gameTimeCheatingDetector.TimeSinceLevelLoad;
            }
        }

        /// <summary>
        /// The protected real time in seconds since the game started (Read Only).
        /// </summary>
        public static float realtimeSinceStartup
        {
            get
            {
                // Get the detector from the AntiCheatMonitor.
                if (gameTimeCheatingDetector == null)
                {
                    gameTimeCheatingDetector = AntiCheatMonitor.Instance.GetDetector<GameTimeCheatingDetector>();
                }

                // If the detector is still null, return the default...
                if (gameTimeCheatingDetector == null)
                {
                    return UnityEngine.Time.realtimeSinceStartup;
                }

                // ...otherwise return the protected value.
                return gameTimeCheatingDetector.RealtimeSinceStartup;
            }
        }

        /// <summary>
        /// The protected Coordinated Universal Time (UTC) DateTime (Read Only). The calculated utc time, which may differs from the original DateTime.UtcNow 
        /// because it is calculated to be secure and trustable as possible.
        /// </summary>
        public static DateTime UtcNow
        {
            get
            {
                // Get the detector from the AntiCheatMonitor.
                if (deviceTimeCheatingDetector == null)
                {
                    deviceTimeCheatingDetector = AntiCheatMonitor.Instance.GetDetector<DeviceTimeCheatingDetector>();
                }

                // If the detector is still null, return the default...
                if (deviceTimeCheatingDetector == null)
                {
                    return DateTime.UtcNow;
                }

                // ...otherwise return the protected value.
                return deviceTimeCheatingDetector.CurrentUtcTime;
            }
        }
    }
}