using System;

namespace OhioState.Collections
{
    /// <summary>
    /// Holds a sticky current state. If a new item is added (Put), then it replaces
    /// the current state. If an item is removed (GetNext), then it retains this state
    /// as well.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the state.</typeparam>
    /// <remarks>This class is not thread-safe.</remarks>
    [Serializable]
    public class CurrentStateBuffer<T> : IPriorityCollection<T>
    {
        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public CurrentStateBuffer() { }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="item">The item to use as the initial state.</param>
        public CurrentStateBuffer(T item)
        {
            this.item = item;
        }
        #endregion

        #region IPriorityCollection<T> Members
        /// <summary>
        /// Gets the next element in the collection.
        /// </summary>
        /// <returns>The next item in the collection.</returns>
        public T GetNext()
        {
            return item;
        }
        /// <summary>
        /// Gets the next item from the collection.
        /// </summary>
        /// <returns>The next item from the collection.</returns>
        /// <remarks>Unlike GetNext, this method does not change the underlying collection.</remarks>
        public T Peek()
        {
            return item;
        }
        /// <summary>
        /// Adds the item to the collection.
        /// </summary>
        /// <param name="item">The item to insert into the collection.</param>
        public void Put(T item)
        {
            this.item = item;
        }
        /// <summary>
        /// The number of items currently in the collection.
        /// </summary>
        public int Count { get { return 1; } }
        /// <summary>
        /// Clear the buffer.
        /// </summary>
        public void Clear() { }
        /// <summary>
        /// Checks whether the collection is full.
        /// </summary>
        /// <returns>Always returns false, as Put() can always be called.</returns>
        public bool IsFull()
        {
            return false;
        }
        #endregion

        #region Member Variables
        T item = default(T);
        #endregion
    }
}
