// System
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Protected.Collection.Chain
{
    /// <summary>
    /// Represents a synchronizer that is responsible for synchronizing a BlockChain with a file on the local file storage.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Note: Only primitive types or structs are supported.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">The content type of the transactions.</typeparam>
    public class FileSynchronizer<T> : ISynchronizer<T>
        where T : struct
    {
        /// <summary>
        /// Gets the file path of the file that is used for the synchronization.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// The timestamp of the file was last read.
        /// </summary>
        private Int64 lastFileReadTimestamp = 0;

        /// <summary>
        /// A ordered list of transactions that are stored in the file.
        /// </summary>
        private List<ITransaction<T>> transactions = new List<ITransaction<T>>();

        /// <summary>
        /// Create a new instance of the <see cref="FileSynchronizer{T}"/> class.
        /// </summary>
        /// <param name="_FilePath">The local file path of the file that is used for the synchronization.</param>
        public FileSynchronizer(String _FilePath)
        {
            // Set the file path.
            this.FilePath = _FilePath;
        }

        /// <summary>
        /// Read all transactions from the file and return them in an ordered list.
        /// </summary>
        /// <param name="_Timestamp">The timestamp the transactions should be read from.</param>
        /// <returns>The list of transactions that were read from the file.</returns>
        private async Task<List<ITransaction<T>>> ReadFromFileAsync(Int64 _Timestamp)
        {
            // The result list of transactions.
            List<ITransaction<T>> var_Result = new List<ITransaction<T>>();

            // Open the file stream.
            using(FileStream var_FileStream = new FileStream(this.FilePath, FileMode.Open, FileAccess.Read))
            {
                // Read all text from the file, backwards.
                using(StreamReader var_StreamReader = new StreamReader(var_FileStream))
                {
                    // Store all read lines.
                    List<String> var_Lines = new List<String>();

                    // Read all lines from the file.
                    while(!var_StreamReader.EndOfStream)
                    {
                        // Read the line.
                        String var_Line = await var_StreamReader.ReadLineAsync();

                        // Add the line to the list.
                        var_Lines.Add(var_Line);
                    }

                    // Iterate the read lines backwards and add them to the result.
                    for(int i = var_Lines.Count - 1; i >= 0; i--)
                    {
                        // Deserialize the transaction.
                        Transaction<T> var_Transaction = JsonUtility.FromJson<Transaction<T>>(var_Lines[i]);

                        // Check if the transaction is newer than the last sync timestamp.
                        if(var_Transaction.timestamp > _Timestamp)
                        {
                            // Add the transaction to the result.
                            var_Result.Insert(0, var_Transaction);
                        }
                        else
                        {
                            // If the transaction is older than the last sync timestamp, break the loop.
                            break;
                        }
                    }
                }
            }

            // Return the result.
            return var_Result;
        }

        /// <summary>
        /// Syncs the blockchain with a file on the file storage. Loads the latest transactions from the file and updates the local blockchain, starting
        /// from the passed sync timestamp.
        /// </summary>
        /// <param name="_SyncTimestamp">The timestamp of the last sync.</param>
        /// <returns>The array of transactions that were loaded from the file. If no new transactions are available, an empty array is returned.</returns>
        /// <summary>
        public async Task<ITransaction<T>[]> ReadAsync(Int64 _SyncTimestamp)
        {
            // #1: Synchronize the blockchain with the file storage.

            // Get the files last modified timestamp.
            Int64 var_LastModifiedTimestamp = File.GetLastWriteTime(this.FilePath).ToFileTimeUtc();

            // Check if the file was modified since the last read, if yes, read the transactions from the file.
            if(var_LastModifiedTimestamp > this.lastFileReadTimestamp)
            {
                // Read the transactions from the file.
                List<ITransaction<T>> var_Transactions = await this.ReadFromFileAsync(this.transactions.Count > 0 ? this.transactions[this.transactions.Count - 1].Timestamp : 0);

                // Lock the transactions list.
                lock(this.transactions)
                {
                    // Add the transactions to the list.
                    this.transactions.AddRange(var_Transactions);
                }

                // Update the last read timestamp.
                this.lastFileReadTimestamp = var_LastModifiedTimestamp;
            }

            // #2: Aggregate the transactions that are newer than the passed sync timestamp.

            // Create a list of result transactions.
            List<ITransaction<T>> var_Result = new List<ITransaction<T>>();

            // Lock the transactions queue.
            lock(this.transactions)
            {
                // Iterate the file transactions backwards and add them to the result.
                for(int i = this.transactions.Count - 1; i >= 0; i--)
                {
                    // Get the transaction.
                    ITransaction<T> var_Transaction = this.transactions[i];

                    // Check if the transaction is newer than the passed sync timestamp.
                    if (var_Transaction.Timestamp > _SyncTimestamp)
                    {
                        // Add the transaction to the result.
                        var_Result.Insert(0, var_Transaction);
                    }
                    else
                    {
                        // If the transaction is older than the passed sync timestamp, break the loop.
                        break;
                    }
                }
            }

            // Return the result.
            return var_Result.ToArray();
        }

        /// <summary>
        /// Syncs the blockchain with a file on the file storage. Saves the transaction from the local blockchain to the file.
        /// </summary>
        /// <typeparam name="T">The content type of the transaction.</typeparam>
        /// <param name="_Transaction">The transaction to save in the file.</param>
        public async Task WriteAsync(ITransaction<T> _Transaction)
        {
            // Create a new transaction with the current ticks as timestamp and the content of the passed transaction.
            Transaction<T> var_WrittenTransaction = new Transaction<T>(DateTime.UtcNow.Ticks, _Transaction.Content);

            // Write it to the file.
            using(FileStream var_FileStream = new FileStream(this.FilePath, FileMode.Append, FileAccess.Write))
            {
                // Write the transaction to the file.
                using(StreamWriter var_StreamWriter = new StreamWriter(var_FileStream))
                {
                    // Serialize the transaction.
                    String var_SerializedTransaction = JsonUtility.ToJson(var_WrittenTransaction);

                    // Write the transaction to the file.
                    await var_StreamWriter.WriteLineAsync(var_SerializedTransaction);

                    // Flush the stream.
                    await var_StreamWriter.FlushAsync();
                }
            }
        }

        /// <summary>
        /// Syncs the blockchain with a file on the file storage. Saves the transactions from the local blockchain to the file.
        /// </summary>
        /// <typeparam name="T">The content type of the transactions.</typeparam>
        /// <param name="_Transactions">The transactions to save in the file.</param>
        public async Task WriteAsync(ITransaction<T>[] _Transactions)
        {
            // Iterate through all transactions, write them to the file.
            foreach(ITransaction<T> var_Transaction in _Transactions)
            {
                await this.WriteAsync(var_Transaction);
            }
        }
    }
}
