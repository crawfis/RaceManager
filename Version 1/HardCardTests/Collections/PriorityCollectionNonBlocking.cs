using System;

namespace OhioState.Collections
{
    /// <summary>
    /// A thread-safe, but non-blocking wrapper on a <typeparamref name="IPriorityQueue{T}"/>. 
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the collection.</typeparam>
    /// <seealso cref="PriorityCollectionBlocking{T}"/>
    /// <remarks>This class is thread-safe.</remarks>
    public class PriorityCollectionNonBlocking<T> : IPriorityCollection<T>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <typeref name="PriorityQueueNonBlocking{T}"/> 
        /// class that is empty and has the default initial capacity.
        /// </summary>
        public PriorityCollectionNonBlocking()
            : this("RingBuffer", defaultCapacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <typeparamref name="PriorityQueueNonBlocking{T}"/> 
        /// class that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="bufferType">A string descriptor for the buffer type.</param>
        /// <param name="capacity">The initial number of elements that the 
        /// <typeparamref name="PriorityQueueNonBlocking{T}"/> can contain.</param>
        /// <seealso cref="PriorityCollectionFactory"/>
        public PriorityCollectionNonBlocking(string bufferType, int capacity)
        {
            buffer = PriorityCollectionFactory.createQueue<T>(bufferType, capacity);
        }
        #endregion

        #region IPriorityCollection<T> Members
        /// <summary>
        /// Clear the buffer.
        /// </summary>
        /// <remarks>This routine is thread-safe.</remarks>
        public void Clear()
        {
            lock(buffer)
                buffer.Clear();
        }
        /// <summary>
        /// The number of items currently in the collection.
        /// </summary>
        /// <remarks>This routine is thread-safe.</remarks>
        public int Count
        {
            get { lock(buffer) return buffer.Count; }
        }
        /// <summary>
        /// Removes the next item from the collection.
        /// </summary>
        /// <returns>The next item from the collection.</returns>
        /// <remarks>This routine is thread-safe.</remarks>
        public T GetNext()
        {
            lock(buffer)
                return buffer.GetNext();
        }

        /// <summary>
        /// Gets the next item from the collection.
        /// </summary>
        /// <returns>The next item from the collection.</returns>
        /// <remarks>Unlike GetNext, this method does not change the underlying collection.</remarks>
        /// <remarks>This routine is thread-safe.</remarks>
        public T Peek()
        {
            lock(buffer)
                return buffer.Peek();
        }

        /// <summary>
        /// Adds the item to the collection.
        /// </summary>
        /// <param name="item">The item to insert into the collection.</param>
        /// <remarks>This routine is thread-safe.</remarks>
        public void Put(T item)
        {
            lock(buffer)
                buffer.Put(item);
        }

        /// <summary>
        /// Checks whether the collection is full.
        /// </summary>
        /// <returns>True if the collection is full. False otherwise.</returns>
        /// <remarks>This routine is thread-safe.</remarks>
        public bool IsFull()
        {
            lock(buffer)
            {
                return (buffer.IsFull());
            }
        }
        #endregion

        #region Member Variables
        private IPriorityCollection<T> buffer;
        private static int defaultCapacity = 128;
        #endregion
    }
}
