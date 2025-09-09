// System
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Watch;

// GUPS - AntiCheat
using GUPS.AntiCheat.Detector;

namespace GUPS.AntiCheat.Protected.Collection.Chain
{
    /// <summary>
    /// Represents a generic data chain that implements the <see cref="IDataChain{T}"/> interface and is observed by the primitive cheating detector. 
    /// Using this class, you can ensure that a chain of data is not modified without your knowledge. Each time you append or remove an item from the
    /// chain, the class verifies its integrity and notifies the primitive cheating detector if a change is detected. This can be performance costly,
    /// so do not use it for large amounts of data.
    /// Only primitive types are supported.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the data chain, must be a nullable value type.</typeparam>
    /// <remarks>
    /// <para>
    /// The <see cref="DataChain{T}"/> class represents a data chain that implements the <see cref="IDataChain{T}"/> interface. It allows 
    /// you to observe changes in the data chain through the <see cref="IWatchedSubject"/> interface. The class supports the storage of 
    /// elements of primitive types and ensures that modifications to the data chain trigger notifications to subscribed observers.
    /// </para>
    /// <para>
    /// The class maintains a linked list of elements, and changes to the data chain are monitored by computing hash codes and notifying the 
    /// primitive detector when a unallowed change is detected.
    /// </para>
    /// </remarks>
    public class DataChain<T> : IDataChain<T>, IWatchedSubject where T: struct
    {
        /// <summary>
        /// Represents the data chain as readonly linked list.
        /// </summary>
        private readonly LinkedList<T> chain;

        /// <summary>
        /// The current hash code of the data chain.
        /// </summary>
        private Int32 hash;

        /// <summary>
        /// Gets the linked list containing the elements of the data chain.
        /// </summary>
        public LinkedList<T> Chain => this.chain;

        /// <summary>
        /// Get if the protected value has integrity, i.e., whether it has maintained its original state.
        /// </summary>
        public bool HasIntegrity { get; private set; } = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataChain{T}"/> class.
        /// </summary>
        public DataChain()
        {
            // Initialize the data chain as an empty linked list.
            this.chain = new LinkedList<T>();

            // Compute the initial hash code.
            this.hash = this.GetHashCode();
        }

        /// <summary>
        /// Appends a new item to the end of the data chain.
        /// </summary>
        /// <param name="_Item">The item to be appended to the data chain.</param>
        /// <returns>True if the element is appended successfully and the chain has its integrity; otherwise, false.</returns>
        public bool Append(T _Item)
        {
            // Verify the integrity of the chain, each time a new item is appended.
            if (!this.CheckIntegrity())
            {
                // The integrity of the chain is compromised, return false.
                return false;
            }

            // Append new item to chain.
            this.chain.AddLast(_Item);

            // Update hash.
            this.hash = this.GetHashCode();

            // The integrity of the chain is maintained, return true.
            return true;
        }

        /// <summary>
        /// Appends a new item to the end of the data chain.
        /// </summary>
        /// <param name="_Item">The item to be appended to the data chain.</param>
        /// <returns>True if the element is appended successfully and the chain has its integrity; otherwise, false.</returns>
        public async Task<bool> AppendAsync(T _Item)
        {
            // Verify the integrity of the chain, each time a new item is appended.
            bool var_HasIntegrity = await Task.Run(() => this.CheckIntegrity()).ConfigureAwait(true);

            if (!var_HasIntegrity)
            {
                // The integrity of the chain is compromised, return false.
                return false;
            }

            // Append new item to chain.
            this.chain.AddLast(_Item);

            // Update hash.
            this.hash = await Task.Run(() => this.GetHashCode()).ConfigureAwait(true);

            // The integrity of the chain is maintained, return true.
            return true;
        }

        /// <summary>
        /// Removes the last item from the end of the data chain.
        /// </summary>
        /// <returns>True if the element could be removed successfully and the chain has its integrity; otherwise, false.</returns>
        public bool RemoveLast()
        {
            // Verify the integrity of the chain, each time an item is removed.
            if (!this.CheckIntegrity())
            {
                // The integrity of the chain is compromised, return false.
                return false;
            }

            // Append new item to chain.
            this.chain.RemoveLast();

            // Update hash.
            this.hash = this.GetHashCode();

            // The integrity of the chain is maintained, return true.
            return true;
        }

        /// <summary>
        /// Removes the last item from the end of the data chain.
        /// </summary>
        /// <returns>True if the element could be removed successfully and the chain has its integrity; otherwise, false.</returns>
        public async Task<bool> RemoveLastAsync()
        {
            // Verify the integrity of the chain, each time an item is removed.
            bool var_HasIntegrity = await Task.Run(() => this.CheckIntegrity()).ConfigureAwait(true);

            if (!var_HasIntegrity)
            {
                // The integrity of the chain is compromised, return false.
                return false;
            }

            // Append new item to chain.
            this.chain.RemoveLast();

            // Update hash.
            this.hash = this.GetHashCode();

            // The integrity of the chain is maintained, return true.
            return true;
        }

        /// <summary>
        /// Verifies the integrity of the data chain, notifying observers if a change is detected.
        /// </summary>
        /// <returns>True if the data chain is verified successfully; otherwise, false.</returns>
        public bool CheckIntegrity()
        {
            // If the integrity of the chain is already compromised, return false.
            if (!this.HasIntegrity)
            {
                return false;
            }

            // Get the current hash code.
            Int32 currentHash = this.GetHashCode();

            // Verify the integrity of the data chain.
            if (this.hash != currentHash)
            {
                this.HasIntegrity = false;
            }

            // Notify the primitive cheating detectoor of the result if the chain has no longer integrity.
            if (!this.HasIntegrity)
            {
                AntiCheatMonitor.Instance.GetDetector<PrimitiveCheatingDetector>()?.OnNext(this);
            }

            return this.HasIntegrity;
        }

        /// <summary>
        /// Computes the hash code for the data chain based on the hash codes of its elements.
        /// </summary>
        /// <returns>A hash code for the current data chain.</returns>
        public override Int32 GetHashCode()
        {
            // Initialize the hash code with a prime number.
            int var_Hash = 17;

            // Make sure to not throw an exception when an overflow occurs and wrap the result.
            unchecked
            {
                // Iterate through the list and add each item to the hash code.
                foreach (T item in this.chain)
                {
                    var_Hash = var_Hash + item.GetHashCode() * 23;
                }
            }

            // Return the final hash code.
            return var_Hash;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current data chain.
        /// </summary>
        /// <param name="_Obj">The object to compare with the current data chain.</param>
        /// <returns>True if the specified object is equal to the current data chain; otherwise, false.</returns>
        public override bool Equals(object _Obj)
        {
            if (_Obj == null || GetType() != _Obj.GetType())
            {
                return false;
            }

            DataChain<T> other = (DataChain<T>)_Obj;

            if (this.chain.Count != other.chain.Count)
            {
                return false;
            }

            LinkedListNode<T> thisNode = this.chain.First;
            LinkedListNode<T> otherNode = other.chain.First;

            while (thisNode != null && otherNode != null)
            {
                if (!thisNode.Value.Equals(otherNode.Value))
                {
                    return false;
                }

                thisNode = thisNode.Next;
                otherNode = otherNode.Next;
            }

            return true;
        }

        /// <summary>
        /// Provides an enumerator for iterating over the elements of the data chain.
        /// </summary>
        /// <returns>An enumerator for the data chain.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.chain.GetEnumerator();
        }

        /// <summary>
        /// Provides a non-generic enumerator for iterating over the elements of the data chain.
        /// </summary>
        /// <returns>A non-generic enumerator for the data chain.</returns>
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
