// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Demo.Demo_DataChain
{
    /// <summary>
    /// A simple coin manager that keeps track of the coins the player has collected and displays the sum of the coins as points in the UI.
    /// </summary>
    public class CoinManager : MonoBehaviour
    {
        /// <summary>
        /// The text that displays the sum of the coins.
        /// </summary>
        public UnityEngine.UI.Text coinSumText;

        /// <summary>
        /// The chain that keeps track of the coins the player has collected.
        /// </summary>
        internal SummedDataChain Coins { get; private set; }

        /// <summary>
        /// On awake, initialize the coin chain.
        /// </summary>
        private void Awake()
        {
            this.Coins = new SummedDataChain();
        }

        /// <summary>
        /// Add a coin to the chain, if any integrity issues are detected, the chain will be reset.
        /// </summary>
        public void AddCoin()
        {
            // Add a coin to the chain.
            this.Coins.Add(10);

            // Verify the integrity of the chain.
            if(!this.Coins.HasIntegrity)
            {
                // Has no integrity, reset the chain.
                this.Coins = new SummedDataChain();

                // Set the current protected sum as first element.
                this.Coins.Add(this.Coins.Sum);
            }

            // Update the coin sum text.
            this.coinSumText.text = this.Coins.Sum.ToString();
        }

        /// <summary>
        /// Remove a coin (-10 points) from the chain, if any integrity issues are detected, the chain will be reset.
        /// </summary>
        public void RemoveCoin()
        {
            // Add a coin to the chain.
            this.Coins.Remove(10);

            // Verify the integrity of the chain.
            if (!this.Coins.HasIntegrity)
            {
                // Has no integrity anymore.

                // Get the current protected sum.
                int currentSum = this.Coins.Sum;

                // Reset the chain.
                this.Coins = new SummedDataChain();

                // Set the current protected sum as first element.
                this.Coins.Add(currentSum);
            }

            // Update the coin sum text.
            this.coinSumText.text = this.Coins.Sum.ToString();
        }

        /// <summary>
        /// Returns the current point sum the player has.
        /// </summary>
        /// <returns></returns>
        public int GetCoinSum()
        {
            return this.Coins.Sum;
        }

        /// <summary>
        /// Custom method to verify the integrity of the chain.
        /// </summary>
        public void VerifyCoins()
        {
            this.Coins.CheckIntegrity();
        }
    }
}