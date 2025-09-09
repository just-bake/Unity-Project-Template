// System
using System;

// Unity
using UnityEngine;

// GUPS
using GUPS.AntiCheat.Core.Punisher;

namespace GUPS.AntiCheat.Demo.Demo_DataChain
{
    /// <summary>
    /// A simple punisher that spawns fake coins, if a cheating attempt has been detected.
    /// </summary>
    public class SpawnFakeCoinsPunisher : MonoBehaviour, IPunisher
    {
        // Name
        #region Name

        /// <summary>
        /// The name of the punisher.
        /// </summary>
        public String Name => "Spawn Fake Coins Punisher";

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
        /// The fake coin prefab to spawn.
        /// </summary>
        public GameObject FakeCoins;

        /// <summary>
        /// The amount of fake coins to spawn.
        /// </summary>
        public int FakeCoinsCount = 10;

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

            // Spawn fake coins. A fake coin removes 10 points from the player on touch.
            for (int i = 0; i < this.FakeCoinsCount; i++)
            {
                Instantiate(this.FakeCoins, new Vector3(UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(1, 3), 0), Quaternion.Euler(0, 0, 45));
            }
        }

        #endregion
    }
}