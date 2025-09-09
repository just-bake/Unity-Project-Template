// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Demo.Demo_DataChain
{
    /// <summary>
    /// A fake coin that the player, that spawns if the player was detected cheating. If he collect it, the coin will respawn 
    /// somewhere else and he will lose points instead of gaining them.
    /// </summary>
    public class FakeCoin : MonoBehaviour
    {
        /// <summary>
        /// The coin manager that keeps track of the coins.
        /// </summary>
        private CoinManager coinManager;

        /// <summary>
        /// On awake, get the coin manager.
        /// </summary>
        private void Awake()
        {
#if UNITY_2023_1_OR_NEWER
            this.coinManager = FindFirstObjectByType<CoinManager>(FindObjectsInactive.Include);
#else
            this.coinManager = GameObject.FindObjectOfType<CoinManager>();
#endif
        }

        /// <summary>
        /// When the player collides with the coin, remove points from the coin manager and respawn the coin somewhere else.
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            // Check if the player has collided with the coin.
            if (other.CompareTag("Player"))
            {
                // Remove points to the coin manager.
                this.coinManager.RemoveCoin();

                // Respawn somewhere else.
                this.transform.position = new Vector3(Random.Range(-10, 10), Random.Range(1, 3), 0);
            }
        }
    }
}