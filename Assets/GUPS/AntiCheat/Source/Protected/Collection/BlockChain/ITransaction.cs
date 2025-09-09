// System
using System;

namespace GUPS.AntiCheat.Protected.Collection.Chain
{
    /// <summary>
    /// Represents a transaction inside a block from a blockchain. A transaction contains a timestamp (the time it was added to the blockchain) 
    /// and a content of type T.
    /// </summary>
    /// <typeparam name="T">The type of the content of the transaction.</typeparam>
    public interface ITransaction<T>
    {
        /// <summary>
        /// The timestamp of the transaction, when it was added to the blockchain. Recommended to use at least milliseconds.
        /// </summary>
        Int64 Timestamp { get; }

        /// <summary>
        /// The serializeable content of the transaction.
        /// </summary>
        T Content { get; }
    }
}
