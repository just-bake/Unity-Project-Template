// Unity
using UnityEngine;

// GUPS - AntiCheat
using GUPS.AntiCheat.Protected.Time;

namespace GUPS.AntiCheat.Demo.Demo_Time
{
    /// <summary>
    /// Spin a game object around the y-axis.
    /// </summary>
    public class Spin : MonoBehaviour
    {
        /// <summary>
        /// The speed of the spin.
        /// </summary>
        public float Speed = 15.0f;

        /// <summary>
        /// Use the demo possible cheatable time or the protected time. Use the GUPS.AntiCheat.Protected.Time.ProtectedTime as replacement for UnityEngine.Time.
        /// </summary>
        public bool UseDemoTime = true;

        /// <summary>
        /// Rotate the game object around the y-axis.
        /// </summary>
        private void Update()
        {
            this.transform.Rotate(Vector3.up, this.Speed * (this.UseDemoTime ? CheatAbleTime.deltaTime : ProtectedTime.deltaTime));
        }
    }
}