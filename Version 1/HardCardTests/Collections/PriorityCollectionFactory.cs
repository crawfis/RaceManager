using System;
using System.Collections.Generic;

namespace OhioState.Collections
{
    /// <summary>
    /// Provides a Factory Method to construct a concrete 
    /// <para name="IPriorityCollection{T}"/> instance.
    /// </summary>
    public static class PriorityCollectionFactory
    {
        /// <summary>
        /// Creates a concrete <typeparamref name="IPriorityCollection{T}"/> instance.
        /// </summary>
        /// <typeparam name="T">Specifies the type of elements in the collection.</typeparam>
        /// <param name="queueType">Provides a string-based descriptor of the desired
        /// concrete implementation.</param>
        /// <param name="capacity">The initial number of elements that the 
        /// <typeparamref name="IPriorityCollection{T}"/> can contain.</param>
        /// <returns>An instance of an <typeparamref name="IPriorityCollection{T}"/>.</returns>
        public static IPriorityCollection<T> createQueue<T>(string queueType, int capacity)
        {
            if (queueType == "RingBuffer")
                return new RingBuffer<T>(capacity);
            else if (queueType == "Heap")
                return new HeapAdaptor<T>(capacity);
            else if (queueType == "Queue")
                return new QueueAdaptor<T>(capacity);
            else if (queueType == "Stack")
                return new StackAdaptor<T>(capacity);
            else if (queueType == "CurrentState")
                return new CurrentStateBuffer<T>();

            return new HeapAdaptor<T>(capacity);
        }
    }
}
