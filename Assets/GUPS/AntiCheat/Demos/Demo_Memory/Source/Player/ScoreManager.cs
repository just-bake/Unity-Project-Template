// System
using System;

// Unity
using UnityEngine;

// GUPS - AntiCheat
using GUPS.AntiCheat.Protected;

namespace GUPS.AntiCheat.Demo.Demo_Protected
{
    /// <summary>
    /// A simple score manager that increases and decreases the score by 10 if an asteriod gets destroyed or if the player collides with an asteroid.
    /// The scores is protected from cheaters by using the ProtectedInt32 class and providing a honeypot score, trapping possible cheaters.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        /// <summary>
        /// The protected score value.
        /// </summary>
        private ProtectedInt32 score = 0;

        /// <summary>
        /// The current score value.
        /// </summary>
        public ProtectedInt32 Score => this.score;

        /// <summary>
        /// Shows the actual score in the UI.
        /// </summary>
        public UnityEngine.UI.Text scoreText;

        /// <summary>
        /// Shows the honeypot score in the UI, trapping possible cheaters.
        /// </summary>
        public UnityEngine.UI.Text honeypotScoreText;

        /// <summary>
        /// Increase the score by 10.
        /// </summary>
        public void IncreaseScore()
        {
            // Increase the score by 10.
            this.score += 10;

            // Update the score text.
            this.RefreshScore();
        }

        /// <summary>
        /// Decrease the score by 10 without going below 0.
        /// </summary>
        public void DecreaseScore()
        {
            // Decrease the score by 10.
            this.score -= 10;

            if (this.score < 0)
            {
                this.score = 0;
            }

            // Update the score text.
            this.RefreshScore();
        }

        /// <summary>
        /// Refresh the score ui text.
        /// </summary>
        public void RefreshScore()
        {
            // Update the score text.
            this.scoreText.text = this.score.ToString();

            // Update the honeypot score text.

            // Get the private "fakeValue" field from the protected score via reflection.
            int honeypotScore = (Int32)this.score.GetType().GetField("fakeValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(this.score);

            // Update the honeypot score text.
            this.honeypotScoreText.text = honeypotScore.ToString();

            // Reset the score (only for the demonstration purposes, you will not need this).
            this.score = this.score.Value;
        }
    }
}
