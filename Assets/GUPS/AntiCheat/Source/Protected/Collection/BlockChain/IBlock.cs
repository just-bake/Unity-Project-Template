// System
using System;

namespace GUPS.AntiCheat.Protected.Collection.Chain
{
    /// <summary>
    /// Represents a generic block interface storing transactions of type T.
    /// </summary>
    /// <typeparam name="T">The type of content stored in the block transactions.</typeparam>
    public interface IBlock<T>
    {
        /// <summary>
        /// Gets the amount of max transactions this block can store.
        /// </summary>
        Int32 Size { get; }

        /// <summary>
        /// Gets an array containing the transactions of the block.
        /// </summary>
        ITransaction<T>[] Items { get; }

        /// <summary>
        /// Gets the transaction at the specified index.
        /// </summary>
        /// <param name="_Index">The index of the transaction to get.</param>
        /// <returns>The transaction at the specified index.</returns>
        ITransaction<T> this[Int32 _Index] { get; }

        /// <summary>
        /// Gets the count of transactions in the block.
        /// </summary>
        Int32 Count { get; }

        /// <summary>
        /// Gets the last transaction appended to the block.
        /// </summary>
        ITransaction<T> Last { get; }

        /// <summary>
        /// Gets the nonce value of the block, which is the hash of the previous block.
        /// </summary>
        Int32 Nonce { get; }

        /// <summary>
        /// Gets the hash value associated with the block.
        /// </summary>
        Int32 Hash { get; }
    }
}