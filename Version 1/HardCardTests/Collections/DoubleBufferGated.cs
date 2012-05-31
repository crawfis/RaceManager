using System;

namespace OhioState.Collections
{
    /// <summary>
    /// Contains two buffers that can be swapped when needed.
    /// </summary>
    /// <typeparam name="T">Specifies the type of buffers.</typeparam>
    /// <remarks>This class is thread-safe, given the constraints that only
    /// one thread is a master and only one slave thread exists, and that
    /// both of the threads call Initialize appropriately before anything else.</remarks>
    /// <remarks>This is a <c>very dangerous</c> class and should be avoided unless you really
    /// know what you are doing.</remarks>
    /// <remarks>This class is thread-safe under the single use-case this class is designed for.</remarks>
    public class DoubleBufferGated<T>
    {
        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="buffer1">The first buffer. Initially the front buffer.</param>
        /// <param name="buffer2">The second buffer. Initially the back buffer.</param>
        DoubleBufferGated(T buffer1, T buffer2)
        {
            this.buffer1 = buffer1;
            this.buffer2 = buffer2;
            front = buffer1;
            back = buffer2;
        }
        #endregion;

        #region Public interface
        /// <summary>
        /// Swap the front buffer with the back buffer.
        /// </summary>
        /// <param name="isMaster"><c>True</c> if this is the master thread and should lock the front buffer.
        /// <c>False</c> if this is the slave thread and should lock the back buffer.</param>
        /// <param name="blockMaster">If <c>true,</c> and <paramref name="isMaster"/> is <c>true,</c>
        /// then the master thread will block and wait for the slave thread to call this routine.
        /// Otherwise, the master thread will attempt to swap the buffers. It will succeed if the slave 
        /// thread is already blocked waiting on SwapBuffers. Otherwise it will return without swapping
        /// buffers.</param>
        /// <remarks>
        /// The Master can swap buffers many times before the Slave is even
        /// initialized (calls Initialize(false)). However, the Slave
        /// can not call SwapBuffers before the Master calls Initialize or
        /// deadlock will occur.
        /// </remarks>
        /// <remarks>This routine is thread-safe under the single use-case this class is designed for.</remarks>
        public void SwapBuffers(bool isMaster, bool blockMaster)
        {
			if( isMaster )
			{
				// Debug statement:
				//    ASSERT_EQUAL(masterThreadID,currentThreadID);
				if( !blockMaster )
				{
					MasterSwap();
				}
				TryMasterSwap();
			}

			SlaveSwap();
        }

        /// <summary>
        /// Associate a master and a slave thread with this instance.
        /// </summary>
        /// <param name="isMaster">True if this is the thread that should be the master thread.</param>
        /// <remarks>
        /// This routine should be called by each thread at start-up,
        /// once with true for the Master thread and once with false
        /// for the slave thread. These calls lock the front buffer to
        /// the Master and the back buffer to the slave. The threads hold
        /// these locks until they both call SwapBuffers.
        /// </remarks>
        /// <remarks>This routine is thread-safe under the single use-case this class is designed for.</remarks>
        public void Initialize(bool isMaster)
        {
            if (isMaster)
            {
                System.Threading.Monitor.Enter(front);
                // Could add a debug statement that captures
                //    the Master thread's ID and subsequently 
                //    checks it.
                // masterThreadID = 
            }
            else
            {
                System.Threading.Monitor.Enter(back);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get the current front buffer.
        /// </summary>
        /// <remarks>This routine is thread-safe under the single use-case this class is designed for.</remarks>
        public T Front
        { 
            get { return front; }
        }
        /// <summary>
        /// Get the current back buffer.
        /// </summary>
        /// <remarks>This routine is thread-safe under the single use-case this class is designed for.</remarks>
        public T Back
        {
            get { return back; }
        }
        #endregion

        #region Implementation
        private bool TryMasterSwap()
        {
            if (!System.Threading.Monitor.TryEnter(back))
                return false;

            T buffer = front;
            front = back;
            back = buffer;
            System.Threading.Monitor.Exit(back);
            return true;
        }
        private T SlaveSwap()
        {
            System.Threading.Monitor.Exit(back);
            System.Threading.Monitor.Enter(front);
            return back;
        }
        private T MasterSwap()
        {
            System.Threading.Monitor.Enter(back);
            T tmp = front;
            front = back;
            back = tmp;
            System.Threading.Monitor.Exit(back);
            return front;
        }
        #endregion

        #region Member variables
        private T front, back, buffer1, buffer2;
        #endregion
    }
}