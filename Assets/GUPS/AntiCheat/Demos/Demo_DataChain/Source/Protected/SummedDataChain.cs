// System
using System;

// GUPS - AntiCheat
using GUPS.AntiCheat.Protected;
using GUPS.AntiCheat.Protected.Collection.Chain;

namespace GUPS.AntiCheat.Demo.Demo_DataChain
{
    /// <summary>
    /// Represents a demo data chain that sums the elements in the chain while maintaining its integrity. Data chains can be useful if the 
    /// order of the data is important. If the data is manipulated in any way, the primitive cheating detector will be notified and so 
    /// notifing you. At the very best combine DataChains / BlockChains with other protected data types in real scenarios, for example use 
    /// ProtectedInt32 instead of Int32.
    /// </summary>
    public class SummedDataChain : DataChain<Int32>
    {
        /// <summary>
        /// Stores the current sum of the chain.
        /// </summary>
        public ProtectedInt32 Sum { get; private set; }

        /// <summary>
        /// Create a new instance of the <see cref="SummedDataChain"/> class.
        /// </summary>
        public SummedDataChain()
        {
            this.Sum = 0;
        }

        /// <summary>
        /// Add a new value to the chain and update the sum. If there are integrity issues, the chain is cleared and the sum is recalculated.
        /// </summary>
        /// <param name="_Value">The value to add to the chain.</param>
        public void Add(Int32 _Value)
        {
            // If the value is appended to the chain, update the sum.
            if(this.Append(_Value))
            {
                // Calculate the sum base on the chain.
                int sum = 0;

                foreach (var var_Item in this.Chain)
                {
                    sum += var_Item;
                }

                // Make sure the sum is not negative.
                if (sum < 0)
                {
                    sum = 0;
                }

                // Assign to the protected sum property.
                this.Sum = sum;
            }
            else
            {
                // There was an integrity issue, the value was not appended to the chain.

                // Clear the chain.
                this.Chain.Clear();

                // Now continue using only the protected sum property.
                this.Sum += _Value;
            }
        }

        /// <summary>
        /// Substract a value from the chain and update the sum. If there are integrity issues, the chain is cleared and the sum is recalculated.
        /// </summary>
        /// <param name="_Value">The value to substract from the chain.</param>
        public void Remove(Int32 _Value)
        {
            // Add the negative value to the chain.
            this.Add(-_Value);
        }
    }
}
