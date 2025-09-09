// System
using System;
using System.Collections;
using System.Collections.Generic;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Integrity;

namespace GUPS.AntiCheat.Protected.Collection
{
    /// <summary>
    /// Represents a protected queue that implements the <see cref="IEnumerable{T}"/>, <see cref="IEnumerable"/>, 
    /// <see cref="IReadOnlyCollection{T}"/>, and <see cref="ICollection"/> interfaces. This queue allows tracking changes and 
    /// provides a hash code for verification purposes. Before interacting with the queue, you should call the <see cref="CheckIntegrity"/>
    /// to verify its integrity.
    /// </summary>
    /// <typeparam name="T">The type of elements in the queue.</typeparam>
    public class ProtectedQueue<T> : IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, ICollection, IDataIntegrity where T : struct
    {
        /// <summary>
        /// Stores the queue of items.
        /// </summary>
        private readonly Queue<T> queue;

        /// <summary>
        /// Gets the hash code associated with the current state of the queue.
        /// </summary>
        public Int32 Hash { get; private set; }

        /// <summary>
        /// Gets the number of elements contained in the queue.
        /// </summary>
        public int Count => this.queue.Count;

        /// <summary>
        /// Gets a value indicating whether access to the queue is synchronized (thread-safe).
        /// </summary>
        bool ICollection.IsSynchronized => ((ICollection)this.queue).IsSynchronized;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the queue.
        /// </summary>
        object ICollection.SyncRoot => ((ICollection)this.queue).SyncRoot;

        /// <summary>
        /// Get if the protected value has integrity, i.e., whether it has maintained its original state.
        /// </summary>
        public bool HasIntegrity { get; private set; } = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtectedQueue{T}"/> class.
        /// </summary>
        public ProtectedQueue()
        {
            this.queue = new Queue<T>();

            this.Hash = this.GetHashCode();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtectedQueue{T}"/> class that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="_Collection">The collection whose elements are copied to the new queue.</param>
        public ProtectedQueue(IEnumerable<T> _Collection)
        {
            this.queue = new Queue<T>(_Collection);

            this.Hash = this.GetHashCode();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtectedQueue{T}"/> class with the specified initial capacity.
        /// </summary>
        /// <param name="_Capacity">The initial number of elements that the queue can contain.</param>
        public ProtectedQueue(int _Capacity)
        {
            this.queue = new Queue<T>(_Capacity);

            this.Hash = this.GetHashCode();
        }

        /// <summary>
        /// Determines whether the queue contains a specific value.
        /// </summary>
        /// <param name="_Item">The object to locate in the queue.</param>
        /// <returns>true if item is found in the queue; otherwise, false.</returns>
        public bool Contains(T _Item) => this.queue.Contains(_Item);

        /// <summary>
        /// Adds an object to the end of the queue.
        /// </summary>
        /// <param name="_Item">The object to add to the queue.</param>
        public void Enqueue(T _Item)
        {
            // Enqueue the new item to the queue.
            this.queue.Enqueue(_Item);

            // Add the new item to the hash code.
            this.Hash = this.AddToHashCode(this.Hash, _Item);
        }

        /// <summary>
        /// Returns the object at the beginning of the queue without removing it.
        /// </summary>
        /// <returns>The object at the beginning of the queue.</returns>
        public T Peek() => this.queue.Peek();

        /// <summary>
        /// Removes and returns the object at the beginning of the queue.
        /// </summary>
        /// <returns>The object removed from the beginning of the queue.</returns>
        public T Dequeue()
        {
            // Dequeue the item from the queue.
            T var_Item = this.queue.Dequeue();

            // Remove the item from the hash code.
            this.Hash = this.RemoveFromHashCode(this.Hash, var_Item);

            // Return the dequeued item.
            return var_Item;
        }

        /// <summary>
        /// Copies the elements of the ICollection to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="_Array">The one-dimensional Array that is the destination of the elements copied from the ICollection.</param>
        /// <param name="_Index">The zero-based index in _Array at which copying begins.</param>
        void ICollection.CopyTo(Array _Array, int _Index) => ((ICollection)this.queue).CopyTo(_Array, _Index);

        /// <summary>
        /// Copies the elements of the queue to an array, starting at a particular array index.
        /// </summary>
        /// <param name="_Array">The one-dimensional array that is the destination of the elements copied from the queue.</param>
        /// <param name="_ArrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] _Array, int _ArrayIndex) => this.queue.CopyTo(_Array, _ArrayIndex);

        /// <summary>
        /// Sets the capacity to the actual number of elements in the queue, if that number is less than a threshold value.
        /// </summary>
        public void TrimExcess() => this.queue.TrimExcess();

        /// <summary>
        /// Returns the object at the beginning of the queue without removing it and returns whether the operation succeeded.
        /// </summary>
        /// <param name="_Result">When this method returns, contains the object at the beginning of the queue, if the queue is not empty; otherwise, the default value for the element type.</param>
        /// <returns>true if there was an object to return; otherwise, false.</returns>
        public bool TryPeek(out T _Result)
        {
            if (this.queue.Count > 0)
            {
                _Result = this.Peek();
                return true;
            }
            _Result = default;
            return false;
        }

        /// <summary>
        /// Removes and returns the object at the beginning of the queue and returns whether the operation succeeded.
        /// </summary>
        /// <param name="_Result">When this method returns, contains the object removed from the beginning of the queue, if the queue is not empty; otherwise, the default value for the element type.</param>
        /// <returns>true if there was an object to return; otherwise, false.</returns>
        public bool TryDequeue(out T _Result)
        {
            if (this.queue.Count > 0)
            {
                _Result = this.Dequeue();
                return true;
            }
            _Result = default;
            return false;
        }

        /// <summary>
        /// Copies the elements of the queue to a new array.
        /// </summary>
        /// <returns>An array containing copies of the elements of the queue.</returns>
        public T[] ToArray() => this.queue.ToArray();

        /// <summary>
        /// Removes all elements from the queue.
        /// </summary>
        public void Clear()
        {
            // Clear the queue.
            this.queue.Clear();

            // Reset the hash code.
            this.Hash = this.GetHashCode();
        }

        /// <summary>
        /// Verifies the integrity of the queue by comparing the current hash with the computed hash.
        /// </summary>
        /// <returns>True if the queue is verified successfully; otherwise, false.</returns>
        public bool CheckIntegrity()
        {
            Int32 currentHash = this.GetHashCode();

            if (this.Hash != currentHash)
            {
                this.HasIntegrity = false;
            }

            return this.HasIntegrity;
        }

        /// <summary>
        /// Returns a hash code for the queue based on its elements.
        /// </summary>
        /// <returns>A hash code for the current queue.</returns>
        public override int GetHashCode()
        {
            // Initialize the hash code with a prime number.
            int var_Hash = 17;

            // Iterate through the queue and add each item to the hash code.
            foreach (T var_Item in this.queue)
            {
                var_Hash = this.AddToHashCode(var_Hash, var_Item);
            }

            // Return the final hash code.
            return var_Hash;
        }

        /// <summary>
        /// Add a new item to the hash, instead of calculating the hash code from scratch.
        /// </summary>
        /// <param name="_HashCode">The current hash code.</param>
        /// <param name="_Item">The item to add to the hash code.</param>
        /// <returns>The new hash code.</returns>
        private int AddToHashCode(int _HashCode, T _Item)
        {
            // Make sure to not throw an exception when an overflow occurs and wrap the result.
            unchecked
            {
                return _HashCode + _Item.GetHashCode() * 23;
            }
        }

        /// <summary>
        /// Remove an existing item from the hash, instead of calculating the hash code from scratch.
        /// </summary>
        /// <param name="_HashCode">The current hash code.</param>
        /// <param name="_Item">The item to remove from the hash code.</param>
        /// <returns>The new hash code.</returns>
        private int RemoveFromHashCode(int _HashCode, T _Item)
        {
            // Make sure to not throw an exception when an overflow occurs and wrap the result.
            unchecked
            {
                return _HashCode - _Item.GetHashCode() * 23;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the elements of the queue.
        /// </summary>
        /// <returns>An enumerator for the queue.</returns>
        public IEnumerator<T> GetEnumerator() => this.queue.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the elements of the queue.
        /// </summary>
        /// <returns>An enumerator for the queue.</returns>
        IEnumerator IEnumerable.GetEnumerator() => this.queue.GetEnumerator();
    }
}