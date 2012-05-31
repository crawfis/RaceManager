using System;
using System.Collections.Generic;
using System.Text;

namespace OhioState.Collections
{
    /// <summary>
    /// A wrapper that adapts a <typeparamref name="Queue{T}"/> to a 
    /// <typeparamref name="IPriorityCollection{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    [Serializable]
    public class QueueAdaptor<T> : Queue<T>, IPriorityCollection<T>
    {
        #region Constructors
        /// <summary>
        /// Constructor specifying an initial queue size.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the 
        /// <typeparamref name="QueueAdaptor{T}"/> can contain.</param>
        public QueueAdaptor(int capacity)
            : base(capacity)
        {
        }
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public QueueAdaptor()
            : base()
        {
        }
        #endregion

        #region IPriorityCollection<T> Members
        /// <summary>
        /// Gets the next element in the queue.
        /// </summary>
        /// <returns>The next item in the queue.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// An <paramref name="InvalidOperationException"/> is thrown if the 
        /// <typeparamref name="QueueAdaptor{T}"/> is empty.</exception>
        public T GetNext()
        {
            return this.Dequeue();
        }

        /// <summary>
        /// Adds the item to the queue.
        /// </summary>
        /// <param name="item">The item to insert into the queue.</param>
        public void Put(T item)
        {
            this.Enqueue(item);
        }

        /// <summary>
        /// Checks whether the queue is full.
        /// </summary>
        /// <returns>True if the queue is full. False otherwise.</returns>
        public bool IsFull()
        {
            return false;
        }
        #endregion
    }

}
