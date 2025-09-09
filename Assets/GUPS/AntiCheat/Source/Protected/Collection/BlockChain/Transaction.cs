// System
using System;
using System.Reflection;

// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Protected.Collection.Chain
{
    /// <summary>
    /// Represents a transaction inside a block from a blockchain. A transaction contains a timestamp (the time it was added to the blockchain) 
    /// and a content of type T.
    /// </summary>
    /// <typeparam name="T">The type of the content of the transaction.</typeparam>
    [Serializable]
    [Obfuscation(Exclude = true)]
    public class Transaction<T> : ITransaction<T>
        where T : struct
    {
        /// <summary>
        /// The timestamp of the transaction, when it was added to the blockchain. Recommended to use at least milliseconds.
        /// </summary>
        [SerializeField]
        public Int64 timestamp;

        /// <summary>
        /// The timestamp of the transaction, when it was added to the blockchain. Recommended to use at least milliseconds.
        /// </summary>
        public Int64 Timestamp { get => timestamp; private set => timestamp = value; }

        /// <summary>
        /// The serializeable content of the transaction.
        /// </summary>
        [SerializeField]
        public T content;

        /// <summary>
        /// The serializeable content of the transaction.
        /// </summary>
        public T Content { get => content; private set => content = value; }

        /// <summary>
        /// Create a new transaction with the current timestamp and content.
        /// </summary>
        /// <param name="_Content">The content of the transaction.</param>
        public Transaction(T _Content)
            :this(DateTimeOffset.UtcNow.Ticks, _Content)
        {
        }

        /// <summary>
        /// Create a new transaction with the passed index, timestamp and content.
        /// </summary>
        /// <param name="_Timestamp">The imestamp of the transaction.</param>
        /// <param name="_Content">The content of the transaction.</param>
        public Transaction(Int64 _Timestamp, T _Content)
        {
            this.timestamp = _Timestamp;
            this.content = _Content;
        }

        /// <summary>
        /// Calculate the hash code of the transaction based on its timestamp and content.
        /// </summary>
        /// <returns>The hash code of the transaction.</returns>
        public override int GetHashCode()
        {
            return (Int32)this.timestamp ^ this.Content.GetHashCode();
        }
    }
}
