// System
using System;
using System.Collections;
using System.Collections.Generic;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Integrity;

namespace GUPS.AntiCheat.Protected.Collection
{
    /// <summary>
    /// Represents a protected stack that implements the <see cref="IEnumerable{T}"/>, <see cref="IEnumerable"/>, 
    /// <see cref="IReadOnlyCollection{T}"/>, and <see cref="ICollection"/> interfaces. This stack allows tracking changes 
    /// and provides a hash code for verification purposes. Before interacting with the stack, you should call the <see cref="CheckIntegrity"/>
    /// to verify its integrity.
    /// </summary>
    /// <typeparam name="T">The type of elements in the stack.</typeparam>
    public class ProtectedStack<T> : IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, ICollection, IDataIntegrity where T : struct
    {
        /// <summary>
        /// Stores the stack of items.
        /// </summary>
        private readonly Stack<T> stack;

        /// <summary>
        /// Gets a value indicating whether access to the this.stack is synchronized (thread-safe).
        /// </summary>
        bool ICollection.IsSynchronized => ((ICollection)this.stack).IsSynchronized;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the this.stack.
        /// </summary>
        object ICollection.SyncRoot => ((ICollection)this.stack).SyncRoot;

        /// <summary>
        /// Gets the hash code associated with the current state of the stack.
        /// </summary>
        public Int32 Hash { get; private set; }

        /// <summary>
        /// Get if the protected value has integrity, i.e., whether it has maintained its original state.
        /// </summary>
        public bool HasIntegrity { get; private set; } = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtectedStack{T}"/> class.
        /// </summary>
        public ProtectedStack()
        {
            this.stack = new Stack<T>();

            this.Hash = this.GetHashCode();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtectedStack{T}"/> class that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="_Collection">The collection whose elements are copied to the new stack.</param>
        public ProtectedStack(IEnumerable<T> _Collection)
        {
            this.stack = new Stack<T>(_Collection);

            this.Hash = this.GetHashCode();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtectedStack{T}"/> class with the specified initial capacity.
        /// </summary>
        /// <param name="_Capacity">The initial number of elements that the stack can contain.</param>
        public ProtectedStack(int _Capacity)
        {
            this.stack = new Stack<T>(_Capacity);

            this.Hash = this.GetHashCode();
        }

        /// <summary>
        /// Gets the number of elements contained in the stack.
        /// </summary>
        public int Count => this.stack.Count;

        /// <summary>
        /// Determines whether the stack contains a specific value.
        /// </summary>
        /// <param name="_Item">The object to locate in the stack.</param>
        /// <returns>true if item is found in the stack; otherwise, false.</returns>
        public bool Contains(T _Item) => this.stack.Contains(_Item);

        /// <summary>
        /// Copies the elements of the stack to an array, starting at a particular array index.
        /// </summary>
        /// <param name="_Array">The one-dimensional Array that is the destination of the elements copied from the stack.</param>
        /// <param name="_ArrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] _Array, int _ArrayIndex) => this.stack.CopyTo(_Array, _ArrayIndex);

        /// <summary>
        /// Copies the elements of the ICollection to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="_Array">The one-dimensional Array that is the destination of the elements copied from the ICollection.</param>
        /// <param name="_Index">The zero-based index in _Array at which copying begins.</param>
        void ICollection.CopyTo(Array _Array, int _Index) => ((ICollection)this.stack).CopyTo(_Array, _Index);

        /// <summary>
        /// Inserts an object at the top of the stack.
        /// </summary>
        /// <param name="_Item">The object to push onto the stack.</param>
        public void Push(T _Item)
        {
            // Push the new item to the stack.
            this.stack.Push(_Item);

            // Add the new item to the hash code.
            this.Hash = this.AddToHashCode(this.Hash, _Item);
        }

        /// <summary>
        /// Returns the object at the top of the stack without removing it.
        /// </summary>
        /// <returns>The object at the top of the stack.</returns>
        public T Peek() => this.stack.Peek();

        /// <summary>
        /// Removes and returns the object at the top of the stack.
        /// </summary>
        /// <returns>The object removed from the top of the stack.</returns>
        public T Pop()
        {
            // Pop the item from the queue.
            T var_Item = this.stack.Pop();

            // Remove the item from the hash code.
            this.Hash = this.RemoveFromHashCode(this.Hash, var_Item);

            // Return the popped item.
            return var_Item;
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements in the stack, if that number is less than a threshold value.
        /// </summary>
        public void TrimExcess() => this.stack.TrimExcess();

        /// <summary>
        /// Returns the object at the top of the stack without removing it and returns whether the operation succeeded.
        /// </summary>
        /// <param name="_Result">When this method returns, contains the object at the top of the stack, if the stack is not empty; otherwise, the default value for the element type.</param>
        /// <returns>true if there was an object to return; otherwise, false.</returns>
        public bool TryPeek(out T _Result) => this.stack.TryPeek(out _Result);

        /// <summary>
        /// Removes and returns the object at the top of the stack and returns whether the operation succeeded.
        /// </summary>
        /// <param name="_Result">When this method returns, contains the object removed from the top of the stack, if the stack is not empty; otherwise, the default value for the element type.</param>
        /// <returns>true if there was an object to return; otherwise, false.</returns>
        public bool TryPop(out T _Result)
        {
            if (this.stack.Count > 0)
            {
                _Result = this.Pop();
                return true;
            }
            _Result = default;
            return false;
        }

        /// <summary>
        /// Copies the elements of the stack to a new array.
        /// </summary>
        /// <returns>An array containing copies of the elements of the stack.</returns>
        public T[] ToArray() => this.stack.ToArray();

        /// <summary>
        /// Removes all elements from the stack.
        /// </summary>
        public void Clear() => this.stack.Clear();

        /// <summary>
        /// Verifies the integrity of the stack by comparing the current hash with the computed hash.
        /// </summary>
        /// <returns>True if the stack is verified successfully; otherwise, false.</returns>
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
            foreach (T var_Item in this.stack)
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
        /// Returns an enumerator that iterates through the stack.
        /// </summary>
        /// <returns>An enumerator for the stack.</returns>
        public IEnumerator<T> GetEnumerator() => this.stack.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the stack.
        /// </summary>
        /// <returns>An enumerator for the stack.</returns>
        IEnumerator IEnumerable.GetEnumerator() => this.stack.GetEnumerator();
    }
}