// System
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Protected.Collection.Chain
{
    /// <summary>
    /// Represents an implementation of the <see cref="IBlock{T}"/> interface, storing transactions of type T for a blockchain. A block contains a 
    /// fixed size of transactions and can be chained with other blocks to a blockchain.
    /// </summary>
    /// <typeparam name="T">The value type of the content stored in the block transactions.</typeparam>
    /// <remarks>
    /// <para>
    /// The block class is designed to store data transactions for a blockchain. A block has a nonce value, which is the hash of the previous block, 
    /// allowing to chain blocks together, while easily verifying the integrity of the chain.
    /// </para>
    /// <para>
    /// The class provides methods to append transactions to the block and verify its integrity. To do so a hash is stored and updated upon each 
    /// transaction addition.
    /// </para>
    /// <para>
    /// Note: Only primitive types or structs are supported.
    /// </para>
    /// </remarks>
    [Serializable]
    [Obfuscation(Exclude = true)]
    public class Block<T> : IBlock<T>, IEnumerable<ITransaction<T>> 
        where T : struct
    {
        /// <summary>
        /// A shared random number generator used for nonce generation.
        /// </summary>
        private static readonly System.Random random = new System.Random();

        /// <summary>
        /// The amount of max transactions this block can store.
        /// </summary>
        [SerializeField]
        private int size;

        /// <summary>
        /// Gets the amount of max transactions this block can store.
        /// </summary>
        public int Size { get => size; private set => size = value; }

        /// <summary>
        /// The array of transactions within the block.
        /// </summary>
        [SerializeReference]
        private readonly ITransaction<T>[] transactions;

        /// <summary>
        /// Gets the array of transactions within the block.
        /// </summary>
        public ITransaction<T>[] Items => transactions;

        /// <summary>
        /// Gets the transaction at the specified index.
        /// </summary>
        /// <param name="_Index">The index of the transaction to get.</param>
        /// <returns>The transaction at the specified index.</returns>
        public ITransaction<T> this[int _Index] { get => this.transactions[_Index]; }

        /// <summary>
        /// The count of transactions currently appended to the block.
        /// </summary>
        [SerializeField]
        private int count;

        /// <summary>
        /// Gets the count of transactions currently appended to the block.
        /// </summary>
        public int Count { get => count; private set => count = value; }

        /// <summary>
        /// Gets the last transaction appended to the block. If no transactions are appended, null is returned.
        /// </summary>
        public ITransaction<T> Last => this.transactions.Length > 0 ? this.transactions[this.Count - 1] : null;

        /// <summary>
        /// Get the last transaction timestamp, may be 0 if the block is empty.
        /// </summary>
        public Int64 LastTransactionTimestamp => this.Last?.Timestamp ?? 0;

        /// <summary>
        /// The nonce value of the block, which is the hash of the previous block.
        /// </summary>
        [SerializeField]
        public int nonce;

        /// <summary>
        /// Gets the nonce value of the block, which is the hash of the previous block.
        /// </summary>
        public int Nonce { get => nonce; private set => nonce = value; }

        /// <summary>
        /// A calculated hash based on the nonce and the transactions of the block.
        /// </summary>
        [SerializeField]
        public int hash;

        /// <summary>
        /// Gets the calculated hash based on the nonce and the transactions of the block.
        /// </summary>
        public int Hash { get => hash; private set => hash = value; }

        /// <summary>
        /// Initializes a new instance of <see cref="Block{T}"/> with the specified size.
        /// </summary>
        /// <param name="_Size">The size of the block.</param>
        public Block(int _Size)
        {
            // Assign parameters.
            this.size = _Size;
            this.transactions = new ITransaction<T>[this.size];

            // Initialize the block with a random nonce.
            this.nonce = random.Next(Int32.MaxValue);

            // Compute the initial hash code.
            this.hash = this.GetHashCode();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Block{T}"/> with the specified size and nonce.
        /// </summary>
        /// <param name="_Size">The size of the block.</param>
        /// <param name="_Nonce">The nonce associated with the block.</param>
        public Block(int _Size, int _Nonce)
        {
            // Assign parameters.
            this.size = _Size;
            this.transactions = new ITransaction<T>[this.size];
            this.nonce = _Nonce;

            // Compute the initial hash code.
            this.hash = this.GetHashCode();
        }

        /// <summary>
        /// Appends a transaction to the block. If the block is full, the transaction is not appended and false is returned.
        /// </summary>
        /// <param name="_Transaction">The transaction to append.</param>
        /// <returns>True if the transaction was successfully appended; otherwise, false if the block is full.</returns>
        public bool Append(ITransaction<T> _Transaction)
        {
            // Check if the block is full.
            if (this.count == this.size)
            {
                return false;
            }

            // Append the transaction.
            this.transactions[Count] = _Transaction;

            // Increment the count.
            this.count++;

            // Update the hash.
            this.hash = this.GetHashCode();

            return true;
        }

        /// <summary>
        /// Verifies the integrity of the block by comparing the stored hash with the computed hash.
        /// </summary>
        /// <returns>True if the block is intact; otherwise, false.</returns>
        public bool Verify()
        {
            return this.hash == this.GetHashCode();
        }

        /// <summary>
        /// Overrides the default GetHashCode method to calculate a hash based on the nonce and the transactions of the block.
        /// </summary>
        /// <returns>The computed hash code.</returns>
        public override int GetHashCode()
        {
            // Initialize the hash code with the random nonce.
            int var_Hash = this.nonce;

            // Make sure to not throw an exception when an overflow occurs and wrap the result.
            unchecked
            {
                // Iterate through the transactions and calculate the hash code.
                for (int i = 0; i < Count; i++)
                {
                    var_Hash = var_Hash + this.transactions[i].GetHashCode() * 23;
                }
            }

            // Return the computed hash code.
            return var_Hash;
        }

        /// <summary>
        /// Overrides the default Equals method to compare the current block with another block.
        /// </summary>
        /// <param name="_Obj">The object to compare with the current block.</param>
        /// <returns>True if the specified object is equal to the current block; otherwise, false.</returns>
        public override bool Equals(object _Obj)
        {
            if (_Obj == null || GetType() != _Obj.GetType())
            {
                return false;
            }

            Block<T> var_Other = (Block<T>)_Obj;

            if (this.count != var_Other.Count)
            {
                return false;
            }

            for (int i = 0; i < Count; i++)
            {
                if (!this.transactions[i].Equals(var_Other.transactions[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Enumerates the transactions in the block.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<ITransaction<T>> GetEnumerator()
        {
            foreach (ITransaction<T> var_Transaction in this.transactions)
            {
                if (var_Transaction == null)
                {
                    break;
                }

                yield return var_Transaction;
            }
        }

        /// <summary>
        /// Enumerates the transactions in the block.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}