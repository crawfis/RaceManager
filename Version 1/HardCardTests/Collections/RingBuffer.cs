using System;
using System.Collections.Generic;
using System.Text;

namespace OhioState.Collections
{
    /// <summary>
    /// Represents a first-in, first-out collection of objects as in a queue, but
    /// uses a fixed memory size. Also known as a circular queue.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the ring buffer.</typeparam>
    /// <remarks>The priority of data in this can be thought of as not keeping track
    /// of old news, so "I'm not a pack rat" priority or "That is yesterday's news" 
    /// priority.</remarks>
    /// <remarks>This class is not thread-safe.</remarks>
    [Serializable]
    public class RingBuffer<T> : IPriorityCollection<T>, ICollection<T>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <typeparamref name="RingBuffer{T}"/> 
        /// class that is empty and has the default initial capacity.
        /// </summary>
        public RingBuffer() : this(defaultCapacity) { }

        /// <summary>
        /// Initializes a new instance of the <typeparamref name="RingBuffer{T}"/> 
        /// class that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the 
        /// <typeparamref name="RingBuffer{T}"/> can contain.</param>
        public RingBuffer(int capacity)
        {
            if (capacity < 1)
                throw new ArgumentOutOfRangeException("Capacity must be at least one.");
            this.capacity = capacity;
            buffer = new T[capacity];
        }
        #endregion

        /// <summary>
        /// Removes the next item from the queue.
        /// </summary>
        /// <returns>The next item in the ring buffer.</returns>
        /// <remarks>This routine is not thread-safe.</remarks>
        public T Dequeue()
        {
            if (numItems == 0)
                throw new InvalidOperationException("The ringbuffer is empty, can not dequeue.");

            // Copy the item over and release the buffer instance.
            T item = buffer[front];
            buffer[front] = default(T);

            // Update the ring buffer state.
            numItems--;
            front++;
            if (front == capacity)
                front = 0;

            return item;
        }

        /// <summary>
        /// Adds the item to the queue.
        /// </summary>
        /// <param name="item">The item to insert into the queue.</param>
        /// <remarks>Note, if the queue is full, this will overwrite the least-recently 
        /// written item in the queue.</remarks>
        /// <remarks>This routine is not thread-safe.</remarks>
        public void Enqueue(T item)
        {
            if (rear == (capacity - 1))
                rear = -1;
            buffer[++rear] = item;
            numItems++;
            if (numItems > capacity)
                Dequeue();
        }

        /// <summary>
        /// Checks whether the queue is full.
        /// </summary>
        /// <returns>True if the queue is full. False otherwise.</returns>
        /// <remarks>This routine is not thread-safe.</remarks>
        public bool IsFull()
        {
            return (capacity == numItems);
        }


        #region ICollection<T> Members
        /// <summary>
        /// Add the item to the queue.
        /// </summary>
        /// <param name="item">The item to add.</param>
        void ICollection<T>.Add(T item)
        {
            Enqueue(item);
        }

        /// <summary>
        /// Clear the buffer.
        /// </summary>
        /// <remarks>This routine is not thread-safe.</remarks>
        public void Clear()
        {
            // Remove the items from the buffer to release any pointers.
            for (int i = 0; i < numItems; i++)
                this.Dequeue();

            numItems = 0;
            front = 0;
            rear = -1;
        }

        /// <summary>
        /// Determines whether an element is in the <typeparamref name="RingBuffer{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <typeparamref name="RingBuffer{T}"/>. 
        /// The value can be <value>null</value> for reference types.</param>
        /// <returns>true if item is found in the <typeparamref name="RingBuffer{T}"/>; 
        /// otherwise, false.</returns>
        public bool Contains(T item)
        {
            foreach (T item2 in this)
                if (item.Equals(item2))
                    return true;
            return false;
        }

        /// <summary>
        /// Copies the elements of the ICollection to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements 
        /// copied from ICollection. The Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the current number of items in the queue.
        /// </summary>
        public int Count
        {
            get { return numItems; }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IEnumerable<T> Members
        /// <summary>
        /// Gets a strongly-typed enumerator.
        /// </summary>
        /// <returns>An enumerator for the queue.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (int index = 0; index < numItems; index++)
            {
                int i = (front + index) % capacity;
                yield return buffer[i];
            }
        }

        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Gets a generic enumerator
        /// </summary>
        /// <returns>An enumerator for the queue.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region IPriorityCollection<T> Members
        /// <summary>
        /// Gets the next element in the queue.
        /// </summary>
        /// <returns>The next item in the queue.</returns>
        public T GetNext()
        {
            return Dequeue();
        }

        /// <summary>
        /// Gets the next item from the queue.
        /// </summary>
        /// <returns>The next item from the queue.</returns>
        /// <remarks>Unlike GetNext, this method does not change the underlying queue.</remarks>
        public T Peek()
        {
            if (numItems == 0)
                throw new InvalidOperationException("The ringbuffer is empty, can not peek.");
            return buffer[front];
        }

        /// <summary>
        /// Adds the item to the queue.
        /// </summary>
        /// <param name="item">The item to insert into the queue.</param>
        /// <remarks>This routine is not thread-safe.</remarks>
        public void Put(T item)
        {
            Enqueue(item);
        }
        #endregion

        #region Member Variables
        private int capacity = defaultCapacity;
        private T[] buffer;
        private int front = 0;
        private int rear = -1;
        private int numItems = 0;
        private static int defaultCapacity = 256;
        #endregion

    }
}
