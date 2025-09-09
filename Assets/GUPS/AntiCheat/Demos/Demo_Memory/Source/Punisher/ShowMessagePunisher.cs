// Unity
using UnityEngine;

// GUPS
using GUPS.AntiCheat.Core.Punisher;
using System;

namespace GUPS.AntiCheat.Demo.Demo_Protected
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
        /// Has a very low threat rating, as it is not a real punishment, but just a message.
        /// </summary>
        [SerializeField]
        [Tooltip("Has a very low threat rating, as it is not a real punishment, but just a message.")]
        private uint threatRating = 5;

        /// <summary>
        /// Has a very low threat rating, as it is not a real punishment, but just a message.
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