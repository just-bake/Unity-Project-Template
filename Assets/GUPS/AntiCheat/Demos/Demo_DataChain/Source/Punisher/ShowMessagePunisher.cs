// Unity
using UnityEngine;

// GUPS
using GUPS.AntiCheat.Core.Punisher;
using System;

namespace GUPS.AntiCheat.Demo.Demo_DataChain
{
    /// <summary>
    /// A simple punisher that shows a message to the player, that his cheating attempt has been detected.
    /// </summary>
    public class ShowMessagePunisher : MonoBehaviour, IPunisher
    {
        // Name
        #region Name

        /// <summary>
        /// The name of the punisher.
        /// </summary>
        public String Name => "Show Message Punisher";

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
        /// Has a low high threat rating, to showcase the punisher quickly.
        /// </summary>
        [SerializeField]
        [Tooltip("Has a low high threat rating, to showcase the punisher quickly.")]
        private uint threatRating = 250;

        /// <summary>
        /// Has a low high threat rating, to showcase the punisher quickly.
        /// </summary>
        public uint ThreatRating => this.threatRating;

        #endregion

        // Punishment
        #region Punishment

        /// <summary>
        /// The ui text to inform about the cheating.
        /// </summary>
        public UnityEngine.UI.Text messageText;

        /// <summary>
        /// Returns if the punisher should only administer punitive actions once or any time the threat level exceeds the threat rating.
        /// </summary>
        public bool PunishOnce => true;

        /// <summary>
        /// Returns if the punisher has administered punitive actions.
        /// </summary>
        public bool HasPunished { get; private set; } = false;

        /// <summary>
        /// Activate the messages game object.
        /// </summary>
        public void Punish()
        {
            // Has punished.
            this.HasPunished = true;

            // Activate the messages game object.
            this.messageText.gameObject.SetActive(true);
        }

        #endregion
    }
}