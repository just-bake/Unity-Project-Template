// System
using System;
using System.Threading.Tasks;

namespace GUPS.AntiCheat.Protected.Collection.Chain
{
    /// <summary>
    /// An interface that is responsible for synchronizing the BlockChain with the server.
    /// </summary>
    /// <typeparam name="T">The content type of the transactions.</typeparam>
    public interface ISynchronizer<T>
    {
        /// <summary>
        /// Syncs the blockchain with the server. Downloads the latest transactions from the server and updates the local blockchain.
        /// </summary>
        /// <param name="_SyncTimestamp">The timestamp of the last sync.</param>
        /// <returns>The array of transactions that were downloaded from the server. If no new transactions are available, an empty array is returned.</returns
        Task<ITransaction<T>[]> ReadAsync(Int64 _SyncTimestamp);

        /// <summary>
        /// Syncs the blockchain with the server. Uploads the transaction from the local blockchain to the server.
        /// </summary>
        /// <param name="_Transaction">The transaction to upload to the server.</param>
        Task WriteAsync(ITransaction<T> _Transaction);

        /// <summary>
        /// Syncs the blockchain with the server. Uploads the transactions from the local blockchain to the server.
        /// </summary>
        /// <param name="_Transactions">The transactions to upload to the server.</param>
        Task WriteAsync(ITransaction<T>[] _Transactions);
    }
}
