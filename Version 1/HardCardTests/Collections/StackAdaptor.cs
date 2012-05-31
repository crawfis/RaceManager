using System;
using System.Collections.Generic;
using System.Text;

namespace OhioState.Collections
{
    /// <summary>
    /// A wrapper that adapts a <typeref name="Stack{T}"/> to a 
    /// <typeparamref name="IPriorityCollection{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    [Serializable]
    public class StackAdaptor<T> : Stack<T>, IPriorityCollection<T>
    {
        #region Constructors
        /// <summary>
        /// Constructor specifying an initial stack size.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the 
        /// <typeparamref name="StackAdaptor{T}"/> can contain.</param>
        public StackAdaptor(int capacity)
            : base(capacity)
        {
        }
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public StackAdaptor()
            : base()
        {
        }
        #endregion

        #region IPriorityCollection<T> Members
        /// <summary>
        /// Gets the next element in the stack.
        /// </summary>
        /// <returns>The next item in the stack.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// An <paramref name="InvalidOperationException"/> is thrown if the 
        /// <typeparamref name="StackAdaptor{T}"/> is empty.</exception>
        public T GetNext()
        {
            return this.Pop();
        }

        /// <summary>
        /// Adds the item to the stack.
        /// </summary>
        /// <param name="item">The item to insert into the stack.</param>
        public void Put(T item)
        {
            this.Push(item);
        }

        /// <summary>
        /// Checks whether the stack is full.
        /// </summary>
        /// <returns>True if the stack is full. False otherwise.</returns>
        public bool IsFull()
        {
            return false;
        }
        #endregion
    }
}
