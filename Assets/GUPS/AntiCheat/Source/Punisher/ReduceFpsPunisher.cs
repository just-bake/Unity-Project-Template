// System
using System;
using System.Reflection;

// Unity
using UnityEngine;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Punisher;

namespace GUPS.AntiCheat.Punisher
{
    /// <summary>
    /// The reduce fps punisher reduces the max frame rate to a custom low value. Can be very annoying for cheaters.
    /// </summary>
    [Serializable]
    [Obfuscation(Exclude = true)]
    public class ReduceFpsPunisher : MonoBehaviour, IPunisher
    {
        // Name
        #region Name

        /// <summary>
        /// The name of the punisher.
        /// </summary>
        public String Name => "Reduce FPS Punisher";

        #endregion

        // Platform
        #region Platform

        /// <summary>
        /// Is supported on all platforms.
        /// </summary>
        public bool IsSupported => true;

        /// <summary>
        /// Gets or sets whether the punisher is active and can administer punitive actions (Default: true).
        /// </summary>
        [SerializeField]
        [Header("Punisher - Settings")]
        [Tooltip("Gets or sets whether the punisher is active and can administer punitive actions (Default: true).")]
        private bool isActive = true;

        /// <summary>
        /// Gets or sets whether the punisher is active and can administer punitive actions (Default: true).
        /// </summary>
        public bool IsActive { get => this.isActive; set => this.isActive = value; }

        #endregion

        // Threat Rating
        #region Threat Rating

        /// <summary>
        /// Is a funny punishment, and can be very annoying for cheaters (Default: 550).
        /// </summary>
        [SerializeField]
        [Tooltip("Is a funny punishment, and can be very annoying for cheaters (Default: 550).")]
        private uint threatRating = 550;

        /// <summary>
        /// Is a funny punishment, and can be very annoying for cheaters (Default: 550).
        /// </summary>
        public uint ThreatRating => this.threatRating;

        #endregion

        // Punishment
        #region Punishment

        /// <summary>
        /// Reduce the max frame rate to a low value.
        /// </summary>
        [SerializeField]
        [Tooltip("The target frame rate. Reduce it to a low value to annoy players once caught cheating!")]
        private int punishFrameRate = 30;

        /// <summary>
        /// Returns if the punisher should only administer punitive actions once or any time the threat level exceeds the threat rating.
        /// </summary>
        public bool PunishOnce => true;

        /// <summary>
        /// Returns if the punisher has administered punitive actions.
        /// </summary>
        public bool HasPunished { get; private set; } = false;

        /// <summary>
        /// Reduce the max frame rate as punishment.
        /// </summary>
        public void Punish()
        {
            // Has punished.
            this.HasPunished = true;

            // Reduce the max frame rate.
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = this.punishFrameRate;
        }

        #endregion
    }
}