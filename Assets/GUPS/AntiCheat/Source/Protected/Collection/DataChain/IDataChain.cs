// System
using System.Collections.Generic;
using System.Threading.Tasks;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Integrity;

namespace GUPS.AntiCheat.Protected.Collection.Chain
{
    /// <summary>
    /// Represents a generic data chain interface, providing functionality to iterate, and manipulate a linked list of elements of type T that validates it integrity.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the data chain.</typeparam>
    public interface IDataChain<T> : IEnumerable<T>, IDataIntegrity
    {
        /// <summary>
        /// Gets the linked list containing the elements of the data chain.
        /// </summary>
        LinkedList<T> Chain { get; }

        /// <summary>
        /// Appends a new item to the end of the data chain.
        /// </summary>
        /// <param name="_Item">The item to be appended to the data chain.</param>
        /// <returns>True if the element is appended successfully and the chain has its integrity; otherwise, false.</returns>
        bool Append(T _Item);

        /// <summary>
        /// Appends a new item to the end of the data chain.
        /// </summary>
        /// <param name="_Item">The item to be appended to the data chain.</param>
        /// <returns>True if the element is appended successfully and the chain has its integrity; otherwise, false.</returns>
        Task<bool> AppendAsync(T _Item);
    }
}