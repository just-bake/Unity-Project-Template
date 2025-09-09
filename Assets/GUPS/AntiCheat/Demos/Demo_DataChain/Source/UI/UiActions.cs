// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Demo.Demo_DataChain
{
    /// <summary>
    /// Demo UI actions that allow the user to manually validate and tamper with the data chain.
    /// </summary>
    public class UiActions : MonoBehaviour
    {
        /// <summary>
        /// The coin manager that keeps track of the coins.
        /// </summary>
        private CoinManager coinManager;

        /// <summary>
        /// On start, get the coin manager.
        /// </summary>
        private void Start()
        {
#if UNITY_2023_1_OR_NEWER
            this.coinManager = FindFirstObjectByType<CoinManager>(FindObjectsInactive.Include);
#else
            this.coinManager = GameObject.FindObjectOfType<CoinManager>();
#endif
        }

        /// <summary>
        /// If the user pressed the manual validate button, validate the data chain and display the output in the console.
        /// </summary>
        public void ValidatePressed()
        {
            // The data chain needs at least one item to be validated.
            if(this.coinManager.Coins.Chain.Count == 0)
            {
                Debug.Log("The data chain is empty, nothing to validate yet.");
                return;
            }

            // Check if the data chain has integrity.
            bool hasIntegrity = this.coinManager.Coins.CheckIntegrity();

            // Output the result.
            if (hasIntegrity)
            {
                Debug.Log("The data chain has integrity.");
            }
            else
            {
                Debug.LogWarning("The data chain has been tampered with.");
            }
        }

        /// <summary>
        /// If the user pressed the manual tamper button, simulate a tampering attempt with the data chain and validate it again.
        /// </summary>
        public void TamperPressed()
        {
            // This is a simple example of how a data chain could be tampered with through memory manipulation.

            // Get the last link in the data chain and change its value.
            if(this.coinManager.Coins.Chain.Last != null)
            {
                this.coinManager.Coins.Chain.Last.Value = UnityEngine.Random.Range(100, 200);
            }

            // Now the data chain has been tampered with, lets validate it again.
            this.ValidatePressed();
        }
    }
}
