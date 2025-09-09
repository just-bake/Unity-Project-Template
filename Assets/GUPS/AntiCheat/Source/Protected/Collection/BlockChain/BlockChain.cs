// System
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Watch;

// GUPS - AntiCheat
using GUPS.AntiCheat.Detector;

namespace GUPS.AntiCheat.Protected.Collection.Chain
{
    /// <summary>
    /// Represents an implementation of the <see cref="IDataChain{Block{T}}"/> interface, storing linked blocks to form a blockchain. The 
    /// blockchain can be synchronized with a remote source to retrieve and upload data while maintaining its integrity. Valid use cases
    /// could be to store game data, such as scores, achievements, or player progress and synchronize it with a remote server. This 
    /// implementation is not designed to store large amounts of data, but rather to store small amounts of data that require integrity 
    /// and synchronization.
    /// </summary>
    /// <typeparam name="T">The value type of the content stored in the block transactions.</typeparam>
    /// <remarks>
    /// <para>
    /// The <see cref="BlockChain{T}"/> class implements the <see cref="IDataChain{Block{T}}"/> interface and supports observation through the 
    /// <see cref="IWatchedSubject"/> interface. The blockchain consists of blocks, each containing a specified number of transactions storing 
    /// content of type <typeparamref name="T"/>. 
    /// </para>
    /// <para>
    /// The blocks are linked together based on their hash, ensuring the integrity of the chain. New transactions can be appended to the chain 
    /// only if it maintains its integrity.
    /// </para>
    /// <para>
    /// Everytime the blockchain is modified (through synchronization or appending new transactions), the integrity of the chain is verified. 
    /// If the integrity is compromised, the blockchain notifies the primitive cheating detector of the possible cheating attempt.
    /// </para>
    /// <para>
    /// Note: Only primitive types or structs are supported.
    /// </para>
    /// </remarks>
    public class BlockChain<T> : IDataChain<Block<T>>, IWatchedSubject 
        where T : struct
    {
        /// <summary>
        /// Represents the blockchain as a readonly linked list of blocks.
        /// </summary>
        private readonly LinkedList<Block<T>> chain;

        /// <summary>
        /// Gets the blockchain as ordered linked list of blocks.
        /// </summary>
        public LinkedList<Block<T>> Chain => this.chain;

        /// <summary>
        /// The size of each block in the blockchain.
        /// </summary>
        private readonly int blockSize;

        /// <summary>
        /// Get the last block of the chain, may be null if the chain is empty.
        /// </summary>
        public Block<T> LastBlock => this.chain.Last?.Value ?? null;

        /// <summary>
        /// The synchronizer is used to synchronize the blockchain with a remote source.
        /// </summary>
        private ISynchronizer<T> synchronizer;

        /// <summary>
        /// Get if the protected value has integrity, i.e., whether it has maintained its original state.
        /// </summary>
        public bool HasIntegrity { get; private set; } = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockChain{T}"/> class with a default block size of 10 and without a synchronizer.
        /// Without a synchronizer, the blockchain cannot be synchronized with a remote source and is only used for local purposes.
        /// </summary>
        public BlockChain()
            :this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockChain{T}"/> class with the specified block size and without a synchronizer.
        /// Without a synchronizer, the blockchain cannot be synchronized with a remote source and is only used for local purposes.
        /// </summary>
        /// <param name="_BlockSize">The size of each block in the blockchain.</param>
        public BlockChain(int _BlockSize)
            : this(_BlockSize, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockChain{T}"/> class with the default block size of 10 and specified synchronizer.
        /// Without a synchronizer, the blockchain cannot be synchronized with a remote source and is only used for local purposes.
        /// </summary>
        /// <param name="_Synchronizer">The synchronizer used to synchronize the blockchain with a remote source.</param>
        public BlockChain(ISynchronizer<T> _Synchronizer)
            :this(10, _Synchronizer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockChain{T}"/> class with the specified block size and synchronizer.
        /// Without a synchronizer, the blockchain cannot be synchronized with a remote source and is only used for local purposes.
        /// </summary>
        /// <param name="_BlockSize">The size of each block in the blockchain.</param>
        /// <param name="_Synchronizer">The synchronizer used to synchronize the blockchain with a remote source.</param>
        public BlockChain(int _BlockSize, ISynchronizer<T> _Synchronizer)
        {
            // Assign the synchronizer.
            this.synchronizer = _Synchronizer;

            // Initialize the blockchain as an empty linked list.
            this.chain = new LinkedList<Block<T>>();
            this.blockSize = _BlockSize;
        }

        /// <summary>
        /// Synchronizes the blockchain with the remote source, appending all remote transactions to the local chain beginning from the last sync timestamp.
        /// If no synchronizer is available, no synchronization is performed and the method just returns true.
        /// </summary>
        /// <returns>True if the synchronization is successful; otherwise, false.</returns>
        public async Task<bool> SynchronizeAsync()
        {
            // If the synchronizer is null, just return true, because the blockchain is local and does not need to be synchronized.
            if (this.synchronizer == null)
            {
                return false;
            }

            // Check if the last block of the blockchain has its integrity before synchronizing.
            if (!this.CheckIntegrityOfLastBlock())
            {
                // The integrity of the blockchain is compromised, return false.
                return false;
            }

            // Get all the transactions from the remote source beginning from the last sync timestamp.
            ITransaction<T>[] var_RemoteTransactions = await this.synchronizer.ReadAsync(this.LastBlock?.Last?.Timestamp ?? 0);

            // Check if the remote transactions are null, return false.
            if (var_RemoteTransactions == null)
            {
                return false;
            }

            // Append all the transactions to the local blockchain.
            this.Append(var_RemoteTransactions);

            // Return true if the synchronization is successful.
            return true;
        }

        /// <summary>
        /// Appends a transaction to the blockchain without synchronizing it with the remote source and without checking the integrity of the chain.
        /// </summary>
        /// <param name="_Transaction">The transaction to append to the blockchain.</param>
        private void Append(ITransaction<T> _Transaction)
        {
            // Find the last block in the chain.
            Block<T> var_LastBlock = null;

            if (this.chain.Count == 0)
            {
                var_LastBlock = new Block<T>(this.blockSize);
                this.chain.AddFirst(var_LastBlock);
            }
            else
            {
                var_LastBlock = this.chain.Last.Value;
            }

            // If the last block is full, create a new block and append it to the chain.
            if (var_LastBlock.Count == var_LastBlock.Size)
            {
                int var_Nonce = var_LastBlock.Hash;
                var_LastBlock = new Block<T>(this.blockSize, var_Nonce);
                this.chain.AddLast(var_LastBlock);
            }

            // Append the item to the last block.
            var_LastBlock.Append(_Transaction);
        }

        /// <summary>
        /// Appends an array of transactions to the blockchain without synchronizing it with the remote source and without checking the integrity of the chain.
        /// </summary>
        /// <param name="_Transaction">The transaction to append to the blockchain.</param>
        private void Append(ITransaction<T>[] _Transactions)
        {
            // Iterate through the transactions and append each to the blockchain.
            foreach (ITransaction<T> var_Transaction in _Transactions)
            {
                // Append the transaction to the blockchain.
                this.Append(var_Transaction);
            }
        }

        /// <summary>
        /// Appends a new item to the blockchain. This item will be packed into a transaction and synchronized with the remote source if available.
        /// </summary>
        /// <param name="_Item">The item to be appended to the blockchain.</param>
        /// <returns>True if the item is appended successfully and the chain has still its integrity; otherwise, false.</returns>
        public bool Append(T _Item)
        {
            // Verify the integrity of the current block, each time a new item is appended.
            if(!this.CheckIntegrityOfLastBlock())
            {
                // The integrity of the blockchain is compromised, return false.
                return false;
            }

            // If the synchronizer is null, directly append the item to the blockchain...
            if(this.synchronizer == null)
            {
                this.Append(new Transaction<T>(_Item));

                return true;
            }

            // ... else write the transaction to the remote source.
            var writeTask = Task.Run(async () => { await this.synchronizer.WriteAsync(new Transaction<T>(_Item)); });
            writeTask.Wait();

            // Synchronize the blockchain with the remote source.
            var syncTask = Task.Run(async () => { return await this.SynchronizeAsync(); });
            syncTask.Wait();

            // Return the result of the synchronization.
            return syncTask.Result;
        }

        /// <summary>
        /// Appends a new item to the blockchain. This item will be packed into a transaction and synchronized with the remote source if available.
        /// </summary>
        /// <param name="_Item">The item to be appended to the blockchain.</param>
        /// <returns>True if the item is appended successfully and the chain has still its integrity; otherwise, false.</returns>
        public async Task<bool> AppendAsync(T _Item)
        {
            // Verify the integrity of the current block, each time a new item is appended.
            if (!this.CheckIntegrityOfLastBlock())
            {
                // The integrity of the blockchain is compromised, return false.
                return false;
            }

            // If the synchronizer is null, directly append the item to the blockchain...
            if (this.synchronizer == null)
            {
                this.Append(new Transaction<T>(_Item));

                return true;
            }

            // ... else write the transaction to the remote source.
            await this.synchronizer.WriteAsync(new Transaction<T>(_Item));

            // Synchronize the blockchain with the remote source.
            return await this.SynchronizeAsync();
        }

        /// <summary>
        /// Appends the blocks content to the blockchain. The content will be synchronized with the remote source if available.
        /// </summary>
        /// <param name="_Item">The block to be appended to the blockchain.</param>
        /// <returns>True if the block is appended successfully and the chain has still its integrity; otherwise, false.</returns>
        bool IDataChain<Block<T>>.Append(Block<T> _Item)
        {
            // Iterate through the transactions and append each content to the blockchain.
            foreach (ITransaction<T> var_Transaction in _Item.Items)
            {
                // Append the transactions content to the blockchain.
                this.Append(var_Transaction.Content);
            }

            return true;
        }

        /// <summary>
        /// Appends the blocks content to the blockchain. The content will be synchronized with the remote source if available.
        /// </summary>
        /// <param name="_Item">The block to be appended to the blockchain.</param>
        /// <returns>True if the block is appended successfully and the chain has still its integrity; otherwise, false.</returns>
        async Task<bool> IDataChain<Block<T>>.AppendAsync(Block<T> _Item)
        {
            // Iterate through the transactions and append each content to the blockchain.
            foreach (ITransaction<T> var_Transaction in _Item.Items)
            {
                // Append the transactions content to the blockchain.
                await this.AppendAsync(var_Transaction.Content);
            }

            return true;
        }

        /// <summary>
        /// Check the integrity of the passed block, also verify the chain integrity with the previous block.
        /// </summary>
        /// <param name="_Node">The block to verify.</param>
        /// <returns>True if the block integrity is verified successfully; otherwise, false.</returns>
        private bool CheckIntegrityOfBlock(LinkedListNode<Block<T>> _Node)
        {
            // Verify the integrity of the passed block.
            if (!_Node.Value.Verify())
            {
                // The integrity of the block is compromised, return false.
                return false;
            }

            // Verify the nonce of the passed block with the hash of the previous block.
            if (_Node.Previous != null && _Node.Value.nonce != _Node.Previous.Value.hash)
            {
                // The integrity of the block or previous is compromised, return false.
                return false;
            }

            // The integrity of the block is not compromised, return true.
            return true;
        }

        /// <summary>
        /// Verifies the integrity of the entire blockchain, notifying observers if a change is detected.
        /// </summary>
        /// <returns>True if the blockchain is verified successfully; otherwise, false.</returns>
        public bool CheckIntegrity()
        {
            // If the integrity of the chain is already compromised, return false.
            if (!this.HasIntegrity)
            {
                return false;
            }

            // Iterate backward through the chain and verify each block.
            var var_Node = this.chain.Last;

            while (var_Node != null)
            {
                // Verify the current block.
                if (!this.CheckIntegrityOfBlock(var_Node))
                {
                    this.HasIntegrity = false;
                    break;
                }

                // Move to the previous block.
                var_Node = var_Node.Previous;
            }

            // Notify the primitive cheating detector of the result if the blockchain has no longer integrity.
            if (!this.HasIntegrity)
            {
                AntiCheatMonitor.Instance.GetDetector<PrimitiveCheatingDetector>()?.OnNext(this);
            }

            // Return the result.
            return this.HasIntegrity;
        }

        /// <summary>
        /// Verifies the integrity of the last block in the blockchain, notifying observers if a change is detected.
        /// </summary>
        /// <returns>True if the last block is verified successfully; otherwise, false.</returns>
        public bool CheckIntegrityOfLastBlock()
        {
            // If the integrity of the chain is already compromised, return false.
            if (!this.HasIntegrity)
            {
                return false;
            }

            // If the chain has at least one block, verify the last block.
            if (this.chain.Count > 0)
            {
                // Verify the last block.
                if(!this.CheckIntegrityOfBlock(this.chain.Last))
                {
                    this.HasIntegrity = false;
                }
            }

            // Notify the primitive cheating detector of the result if the last block has no longer integrity.
            if (!this.HasIntegrity)
            {
                AntiCheatMonitor.Instance.GetDetector<PrimitiveCheatingDetector>()?.OnNext(this);
            }

            // Return the result.
            return this.HasIntegrity;
        }

        /// <summary>
        /// Provides an enumerator for iterating over the blocks in the blockchain.
        /// </summary>
        /// <returns>An enumerator for the blockchain.</returns>
        public IEnumerator<Block<T>> GetEnumerator()
        {
            return this.chain.GetEnumerator();
        }

        /// <summary>
        /// Provides a non-generic enumerator for iterating over the blocks in the blockchain.
        /// </summary>
        /// <returns>A non-generic enumerator for the blockchain.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.chain.GetEnumerator();
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public void Dispose()
        {
            // Does nothing.
        }
    }
}