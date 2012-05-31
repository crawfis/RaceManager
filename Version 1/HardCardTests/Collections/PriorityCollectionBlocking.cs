using System;
using System.Threading;

namespace OhioState.Collections
{
    /// <summary>
    /// Represents a first-in, first-out collection of objects. It is based on
    /// a concrete implementation of the <typeparamref name="IPriorityCollection{T}"/> 
    /// interface. This class
    /// blocks the thread when adding an item (enqueue) if the buffer is full, and
    /// blocks the thread when removing an item (dequeue) if the buffer is empty.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the collection.</typeparam>
    /// <seealso cref="PriorityCollectionNonBlocking{T}"/>
    /// <remarks>This class is thread-safe.</remarks>
    public class PriorityCollectionBlocking<T> : IPriorityCollection<T>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <typeparamref name="PriorityQueueBlocking{T}"/> 
        /// class that is empty and has the default initial capacity.
        /// </summary>
        public PriorityCollectionBlocking() : this("RingBuffer", defaultCapacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <typeparamref name="PriorityQueueBlocking{T}"/> 
        /// class that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="bufferType">A string descriptor for the buffer type.</param>
        /// <param name="capacity">The initial number of elements that the 
        /// <typeparamref name="PriorityQueueBlocking{T}"/> can contain.</param>
        /// <seealso cref="PriorityQueueFactory"/>
        public PriorityCollectionBlocking(string bufferType, int capacity)
        {
            buffer = PriorityCollectionFactory.createQueue<T>(bufferType, capacity);
        }
        #endregion

        /// <summary>
        /// Signal any blocked threads to quit.
        /// </summary>
        /// <remarks>This routine is thread-safe.</remarks>
        public void Quit()
        {
            lock(buffer)
            {
                quitting = true;
                Monitor.PulseAll(buffer);
            }
            System.Threading.Thread.Sleep(sleepTime);
        }

        #region IPriorityCollection<T> Members
        /// <summary>
        /// Removes the next item from the collection.
        /// </summary>
        /// <returns>The next item in the collection.</returns>
        /// <remarks>This routine is thread-safe.</remarks>
        public T GetNext()
        {
            T item;
            lock(buffer)
            {
                while (!quitting && (buffer.Count == 0))
                {
                    Monitor.Wait(buffer);
                }
                //
                // What to do now if we are quitting? If the application using this
                // expects a put for every get, we should not return a default T.
                // Give the system time to wrap up. May still throw an exeption in the
                // GetNext call.
                //
                // TODO: Handle quit better. Note that we currently have the lock.
                //
                item = buffer.GetNext();
                Monitor.Pulse(buffer);
            }
            return item;
        }

        /// <summary>
        /// Gets the next item from the collection.
        /// </summary>
        /// <returns>The next item in the collection.</returns>
        /// <remarks>Unlike GetNext, this method does not change the underlying collection.</remarks>
        /// <remarks>This routine is thread-safe.</remarks>
        public T Peek()
        {
            T item;
            lock(buffer)
            {
                while (!quitting && (buffer.Count == 0))
                {
                    Monitor.Wait(buffer);
                }
                item = buffer.Peek();
            }
            return item;
        }

        /// <summary>
        /// Adds the item to the collection.
        /// </summary>
        /// <param name="item">The item to insert into the collection.</param>
        /// <remarks>This routine is thread-safe.</remarks>
        public void Put(T item)
        {
            lock(buffer)
            {
                while (!quitting && buffer.IsFull())
                {
                    Monitor.Wait(buffer);
                }
                buffer.Put(item);
                Monitor.Pulse(buffer);
            }
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

        /// <summary>
        /// Clear the buffer.
        /// </summary>
        /// <remarks>This routine is thread-safe.</remarks>
        public void Clear()
        {
            lock(buffer)
            {
                buffer.Clear();
                Monitor.PulseAll(buffer);
            }
        }

        /// <summary>
        /// Returns the current number of items in the collection.
        /// </summary>
        /// <remarks>This routine is thread-safe.</remarks>
        public int Count
        {
            get { lock (buffer) return buffer.Count; }
        }
        #endregion

        #region Member variables
        private IPriorityCollection<T> buffer;
        private bool quitting = false;
        private static int sleepTime = 256;
        private static int defaultCapacity = 128;
        #endregion

    }
}