// System
using System.Reflection;

// Unity
using UnityEngine;

// GUPS - AntiCheat
using GUPS.AntiCheat.Protected;

namespace GUPS.AntiCheat.Demo.Demo_Protected
{
    /// <summary>
    /// A simple demo ui component that simulates how a cheater would try to cheat variables stored inside the memory, from outside the game.
    /// </summary>
    public class UiCheatButtons : MonoBehaviour
    {
        /// <summary>
        /// The score manager.
        /// </summary>
        public ScoreManager scoreManager;

        /// <summary>
        /// Simulate how a cheater would try to cheat the game, from outside the game.
        /// </summary>
        public void CheatButton()
        {
            // Get the private "score" field from the score manager via reflection.
            ProtectedInt32 protectedScore = (ProtectedInt32) this.scoreManager.GetType().GetField("score", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(this.scoreManager);

            // Get the private "fakeValue" field from the protected score via reflection.
            FieldInfo fieldInfo = typeof(ProtectedInt32).GetField("fakeValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Modify the honeypot score to 1000.
            object boxedProtectedScore = protectedScore;

            fieldInfo.SetValue(boxedProtectedScore, 1000);

            protectedScore = (ProtectedInt32) boxedProtectedScore;

            // Set the private "score" field from the score manager via reflection to the modified protected score.
            this.scoreManager.GetType().GetField("score", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(this.scoreManager, protectedScore);

            // Refresh the score.
            this.scoreManager.RefreshScore();
        }
    }
}