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
    /// The flip camera punisher flips the camera view horizontally or vertically. Can be very annoying for cheaters in first person shooters.
    /// </summary>
    [Serializable]
    [Obfuscation(Exclude = true)]
    public class FlipCameraPunisher : MonoBehaviour, IPunisher
    {
        // Name
        #region Name

        /// <summary>
        /// The name of the punisher.
        /// </summary>
        public String Name => "Flip Camera Punisher";

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
        /// Is a funny punishment, and can in first persion shooters be very annoying for cheaters (Default: 450).
        /// </summary>
        [SerializeField]
        [Tooltip("Is a funny punishment, and can in first persion shooters be very annoying for cheaters (Default: 450).")]
        private uint threatRating = 450;

        /// <summary>
        /// Is a funny punishment, and can in first persion shooters be very annoying for cheaters (Default: 450).
        /// </summary>
        public uint ThreatRating => this.threatRating;

        #endregion

        // Punishment
        #region Punishment

        /// <summary>
        /// Stores if the camera has been flipped.
        /// </summary>
        private bool isFlipped = false;

        /// <summary>
        /// Flip / mirror the camera view horizontally or vertically.
        /// </summary>
        [SerializeField]
        [Tooltip("Flip / mirror the camera view horizontally or vertically.")]
        private bool flipHorizontal = true;

        /// <summary>
        /// Returns if the punisher should only administer punitive actions once or any time the threat level exceeds the threat rating.
        /// </summary>
        public bool PunishOnce => true;

        /// <summary>
        /// Returns if the punisher has administered punitive actions.
        /// </summary>
        public bool HasPunished => this.isFlipped;

        /// <summary>
        /// Flip / mirror the camera view horizontally or vertically.
        /// </summary>
        public void Punish()
        {
            // If already flipped, return.
            if(this.isFlipped)
            {
                return;
            }

            // Get the main camera.
            var targetCamera = Camera.main;

            // Flip the camera.
            if (this.flipHorizontal)
            {
                // Horizontal flip.
                Matrix4x4 proj = targetCamera.projectionMatrix;
                proj.m11 = -proj.m11;
                proj.m13 = -proj.m13;
                targetCamera.projectionMatrix = proj;
                GL.invertCulling = true;
            }
            else
            {
                // Vertical flip.
                Matrix4x4 proj = targetCamera.projectionMatrix;
                proj.m00 = -proj.m00;
                proj.m01 = -proj.m01;
                targetCamera.projectionMatrix = proj;
                GL.invertCulling = true;
            }

            // Set the flag to true.
            this.isFlipped = true;
        }

        #endregion
    }
}