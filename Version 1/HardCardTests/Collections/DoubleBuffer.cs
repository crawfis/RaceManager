using System;

namespace OhioState.Collections
{
    /// <summary>
    /// Contains two buffers that can be swapped when needed.
    /// </summary>
    /// <typeparam name="T">Specifies the type of buffers.</typeparam>
    public class DoubleBuffer<T>
    {
        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="buffer1">The first buffer. Initially the front buffer.</param>
        /// <param name="buffer2">The second buffer. Initially the back buffer.</param>
        DoubleBuffer(T buffer1, T buffer2)
        {
            this.buffer1 = buffer1;
            this.buffer2 = buffer2;
            front = buffer1;
            back = buffer2;
        }
        #endregion;

        /// <summary>
        /// Swap the front buffer with the back buffer.
        /// </summary>
        public void SwapBuffers()
        {
            T buffer = front;
            front = back;
            back = buffer;
        }

        #region Properties
        /// <summary>
        /// Get the current front buffer.
        /// </summary>
        public T Front
        { 
            get { return front; }
        }
        /// <summary>
        /// Get the current back buffer.
        /// </summary>
        public T Back
        {
            get { return back; }
        }
        #endregion

        #region Member variables
        private T front, back, buffer1, buffer2;
        #endregion
    }
}