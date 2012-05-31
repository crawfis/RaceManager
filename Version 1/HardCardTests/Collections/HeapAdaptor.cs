using System;
using System.Collections.Generic;
using System.Text;

namespace OhioState.Collections
{
    /// <summary>
    /// A wrapper that adapts a <typeparamref name="Heap{T}"/> to a 
    /// <typeparamref name="IPriorityCollection{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    [Serializable]
    public class HeapAdaptor<T> : Heap<T>, IPriorityCollection<T>
    {
        #region Constructors
        /// <summary>
        /// Constructor specifying an inital size and an <typeparamref name="IComparer{T}"/>.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the 
        /// <typeparamref name="HeapAdaptor{T}"/> can contain.</param>
        /// <param name="costComparer">The comparer to use.</param>
        public HeapAdaptor(int capacity, IComparer<T> costComparer)
            : base(capacity, costComparer)
        {
        }
        /// <summary>
        /// Constructor specifying an initial heap size.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the 
        /// <typeparamref name="HeapAdaptor{T}"/> can contain.</param>
        public HeapAdaptor(int capacity)
            : base(capacity)
        {
        }
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public HeapAdaptor()
            : base()
        {
        }
        #endregion

        #region IPriorityCollection<T> Members

        /// <summary>
        /// Gets the next element in the heap.
        /// </summary>
        /// <returns>The next item in the heap.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// An <paramref name="InvalidOperationException"/> is thrown if the 
        /// <typeparamref name="HeapAdaptor{T}"/> is empty.</exception>
        public T GetNext()
        {
            return this.RemoveRoot();
        }

        /// <summary>
        /// Gets the next item from the heap.
        /// </summary>
        /// <returns>The next item from the heap.</returns>
        /// <remarks>Unlike GetNext, this method does not change the underlying heap.</remarks>
        /// <exception cref="System.InvalidOperationException">
        /// An <paramref name="InvalidOperationException"/> is thrown if the 
        /// <typeparamref name="HeapAdaptor{T}"/> is empty.</exception>
        public T Peek()
        {
            return this.Root;
        }

        /// <summary>
        /// Adds the item to the heap.
        /// </summary>
        /// <param name="item">The item to insert into the heap.</param>
        public void Put(T item)
        {
            this.Add(item);
        }

        /// <summary>
        /// Checks whether the heap is full.
        /// </summary>
        /// <returns>True if the heap is full. False otherwise.</returns>
        public bool IsFull()
        {
            return false;
        }
        #endregion
    }
}
