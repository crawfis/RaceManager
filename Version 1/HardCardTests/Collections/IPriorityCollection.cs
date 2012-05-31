using System;
using System.Collections.Generic;
using System.Text;

namespace OhioState.Collections
{
    /// <summary>
    /// Interface for a priority-based collection or a collection with a determined 
    /// order in accessing the elements of a collection  (e.g., queue, stack, etc.).
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    public interface IPriorityCollection<T>
    {
        /// <summary>
        /// Gets the next element in the collection.
        /// </summary>
        /// <returns>The next item in the collection.</returns>
        T GetNext();
        /// <summary>
        /// Gets the next item from the collection.
        /// </summary>
        /// <returns>The next item from the collection.</returns>
        /// <remarks>Unlike GetNext, this method does not change the underlying collection.</remarks>
        T Peek();
        /// <summary>
        /// Adds the item to the collection.
        /// </summary>
        /// <param name="item">The item to insert into the collection.</param>
        void Put(T item);
        /// <summary>
        /// The number of items currently in the collection.
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Clear the buffer.
        /// </summary>
        void Clear();
        /// <summary>
        /// Checks whether the collection is full.
        /// </summary>
        /// <returns>True if the collection is full. False otherwise.</returns>
        bool IsFull();
    }
}
