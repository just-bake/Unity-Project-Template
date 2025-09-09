// System
using System;
using System.Collections;
using System.Collections.Generic;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Integrity;

namespace GUPS.AntiCheat.Protected.Collection
{
    /// <summary>
    /// Represents a protected list that implements the <see cref="IList{T}"/> interface. This list allows tracking changes 
    /// and provides a hash code for verification purposes. Before interacting with the list, you should call the <see cref="CheckIntegrity"/> 
    /// method to verify its integrity.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public class ProtectedList<T> : IList<T>, IDataIntegrity where T : struct
    {
        /// <summary>
        /// Stores the list of items.
        /// </summary>
        private readonly List<T> list;

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="_Index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public T this[int _Index]
        {
            get => this.list[_Index];
            set
            {
                // First remove the existing item from the hash code.
                this.Hash = this.RemoveFromHashCode(this.Hash, this.list[_Index]);

                // Second set the new value.
                this.list[_Index] = value;

                // Third add the new item to the hash code.
                this.Hash = this.AddToHashCode(this.Hash, value);
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the list.
        /// </summary>
        public int Count => this.list.Count;

        /// <summary>
        /// Gets a value indicating whether the list is read-only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets the hash code associated with the current state of the list.
        /// </summary>
        public Int32 Hash { get; private set; }

        /// <summary>
        /// Get if the protected value has integrity, i.e., whether it has maintained its original state.
        /// </summary>
        public bool HasIntegrity { get; private set; } = true;

        /// <summary>
        /// Create a new instance of the <see cref="ProtectedList{T}"/> class.
        /// </summary>
        public ProtectedList()
        {
            this.list = new List<T>();

            this.Hash = this.GetHashCode();
        }

        /// <summary>
        /// Adds an item to the end of the list.
        /// </summary>
        /// <param name="_Item">The item to add to the list.</param>
        public void Add(T _Item)
        {
            // Add the new item to the list.
            this.list.Add(_Item);

            // Add the new item to the hash code.
            this.Hash = this.AddToHashCode(this.Hash, _Item);
        }

        /// <summary>
        /// Inserts an item into the list at the specified index.
        /// </summary>
        /// <param name="_Index">The zero-based index at which the item should be inserted.</param>
        /// <param name="_Item">The item to insert into the list.</param>
        public void Insert(int _Index, T _Item)
        {
            // Inser the new item into the list.
            this.list.Insert(_Index, _Item);

            // Add the new item to the hash code.
            this.Hash = this.AddToHashCode(this.Hash, _Item);
        }

        /// <summary>
        /// Determines whether the list contains a specific value.
        /// </summary>
        /// <param name="_Item">The object to locate in the list.</param>
        /// <returns>true if item is found in the list; otherwise, false.</returns>
        public bool Contains(T _Item) => this.list.Contains(_Item);

        /// <summary>
        /// Copies the elements of the list to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="_Array">The one-dimensional Array that is the destination of the elements copied from the list.</param>
        /// <param name="_ArrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] _Array, int _ArrayIndex) => this.list.CopyTo(_Array, _ArrayIndex);

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire list.
        /// </summary>
        /// <param name="_Item">The object to locate in the list.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire list, if found; otherwise, -1.</returns>
        public int IndexOf(T _Item) => this.list.IndexOf(_Item);

        /// <summary>
        /// Removes the first occurrence of a specific object from the list.
        /// </summary>
        /// <param name="_Item">The object to remove from the list.</param>
        /// <returns>true if item was successfully removed from the list; otherwise, false.</returns>
        public bool Remove(T _Item)
        {
            // Remove the item from the list.
            bool var_Removed = this.list.Remove(_Item);

            // Remove the item from the hash code.
            if (var_Removed)
            {
                this.Hash = this.RemoveFromHashCode(this.Hash, _Item);
            }

            // Return if the item was removed successfully.
            return var_Removed;
        }

        /// <summary>
        /// Removes the element at the specified index of the list.
        /// </summary>
        /// <param name="_Index">The zero-based index of the element to remove.</param>
        public void RemoveAt(int _Index)
        {
            // First get the item to remove from the list.
            T var_Item = this.list[_Index];

            // Second remove the item from the list.
            this.list.RemoveAt(_Index);

            // Third remove the existing item from the hash code.
            this.Hash = this.RemoveFromHashCode(this.Hash, var_Item);
        }

        /// <summary>
        /// Removes all elements from the list.
        /// </summary>
        public void Clear()
        {
            // Clear the list.
            this.list.Clear();

            // Recalculate the hash code.
            this.Hash = this.GetHashCode();
        }

        /// <summary>
        /// Verifies the integrity of the list by comparing the current hash with the computed hash.
        /// </summary>
        /// <returns>True if the list is verified successfully; otherwise, false.</returns>
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
        /// Returns a hash code for the list based on its elements.
        /// </summary>
        /// <returns>A hash code for the current list.</returns>
        public override int GetHashCode()
        {
            // Initialize the hash code with a prime number.
            int var_Hash = 17;

            // Iterate through the list and add each item to the hash code.
            foreach (T var_Item in this.list)
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
        /// Returns an enumerator that iterates through the list.
        /// </summary>
        /// <returns>An enumerator for the list.</returns>
        public IEnumerator<T> GetEnumerator() => this.list.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the list.
        /// </summary>
        /// <returns>An enumerator for the list.</returns>
        IEnumerator IEnumerable.GetEnumerator() => this.list.GetEnumerator();
    }
}