// System
using System;
using System.Threading.Tasks;

// Unity
using UnityEngine;
using UnityEngine.Networking;

namespace GUPS.AntiCheat.Protected.Collection.Chain
{
    /// <summary>
    /// Represents a synchronizer that is responsible for synchronizing a BlockChain with a web server. The web server should provide a read and 
    /// write endpoint. The read endpoint should return the transactions as JSON array and accept an int64 'timestamp' as query parameter. The 
    /// write endpoint should accept transactions as JSON array. And set the timestamp for each received transaction. Use at least milliseconds.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WebSynchronizer<T> : ISynchronizer<T>
        where T : struct
    {
        /// <summary>
        /// Get the read endpoint of the web server. The endpoint should return the transactions as JSON array.
        /// </summary>
        public String ReadEndpoint { get; private set; }

        /// <summary>
        /// Get the write endpoint of the web server. The endpoint should accept transactions as JSON array.
        /// </summary>
        public String WriteEndpoint { get; private set; }

        /// <summary>
        /// Create a new instance of the <see cref="WebSynchronizer{T}"/> class.
        /// </summary>
        /// <param name="_ReadEndpoint">The read endpoint of the web server. The endpoint should return the transactions as JSON array. The endpoint 
        /// should accept a timestamp as query parameter.</param>
        /// <param name="_WriteEndpoint">The write endpoint of the web server. The endpoint should accept transactions as JSON array. The endpoint 
        /// should set the timestamp for each received transaction.</param>
        public WebSynchronizer(String _ReadEndpoint, String _WriteEndpoint)
        {
            // Set the endpoints.
            this.ReadEndpoint = _ReadEndpoint;
            this.WriteEndpoint = _WriteEndpoint;
        }

        /// <summary>
        /// Syncs the blockchain with a remove server. Downloads the latest transactions from the server and updates the local blockchain, starting
        /// from the passed sync timestamp.
        /// </summary>
        /// <param name="_SyncTimestamp">The timestamp of the last sync.</param>
        /// <returns>The array of transactions that were loaded from the server. If no new transactions are available, an empty array is returned.</returns>
        /// <exception cref="Exception">If the web request was not successful, an exception is thrown.</exception>
        public async Task<ITransaction<T>[]> ReadAsync(Int64 _SyncTimestamp)
        {
            // Send a request to the server to get the transactions.
            using (UnityWebRequest var_Request = UnityWebRequest.Get(this.ReadEndpoint + "?timestamp=" + _SyncTimestamp))
            {
                // Send the request and wait for the response.
                var var_RequestWaiter = var_Request.SendWebRequest();

                while (!var_RequestWaiter.isDone)
                {
                    await Task.Delay(100);
                }

                // If the request was not successful, throw an exception.
                if (var_Request.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception(var_Request.error);
                }

                // Get the content of the response.
                String var_Content = var_Request.downloadHandler.text;

                // If the content is empty or null, return an empty array.
                if (String.IsNullOrEmpty(var_Content))
                {
                    return new ITransaction<T>[0];
                }

                // Parse the content of the response.
                Transaction<T>[] var_Transactions = JsonUtility.FromJson<Transaction<T>[]>(var_Content);

                // Return the transactions.
                return var_Transactions;
            }
        }

        /// <summary>
        /// Syncs the blockchain with a remove server. Uploads the transaction from the local blockchain to the server.
        /// </summary>
        /// <typeparam name="T">The content type of the transaction.</typeparam>
        /// <param name="_Transaction">The transaction to upload to the server.</param>
        public async Task WriteAsync(ITransaction<T> _Transaction)
        {
            // Put the transaction into an array and write it to the server.
            await this.WriteAsync(new ITransaction<T>[] { _Transaction });
        }

        /// <summary>
        /// Syncs the blockchain with a remove server. Uploads the transactions from the local blockchain to the server.
        /// </summary>
        /// <typeparam name="T">The content type of the transactions.</typeparam>
        /// <param name="_Transactions">The transactions to upload to the server.</param>
        public async Task WriteAsync(ITransaction<T>[] _Transactions)
        {
            // Serialize the transactions to JSON.
            String var_Content = JsonUtility.ToJson(_Transactions);

            // Send a request to the server to post the transactions.
#if UNITY_2022_2_OR_NEWER
            using (UnityWebRequest var_Request = UnityWebRequest.Post(this.WriteEndpoint, var_Content, "application/json"))
            {
                // Send the request and wait for the response.
                var var_RequestWaiter = var_Request.SendWebRequest();

                while (!var_RequestWaiter.isDone)
                {
                    await Task.Delay(100);
                }

                // If the request was not successful, throw an exception.
                if (var_Request.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception(var_Request.error);
                }
            }
#else
            using (UnityWebRequest var_Request = UnityWebRequest.Post(this.WriteEndpoint, var_Content))
            {
                // Send the request and wait for the response.
                var var_RequestWaiter = var_Request.SendWebRequest();

                while (!var_RequestWaiter.isDone)
                {
                    await Task.Delay(100);
                }

                // If the request was not successful, throw an exception.
                if (var_Request.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception(var_Request.error);
                }
            }
#endif
        }
    }
}
