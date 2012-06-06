using System;
using System.Collections.Generic;
using System.IO;

namespace Hardcard.Scoring
{
    /// <summary>
    /// A simple class to send all log messages to a text file.
    /// </summary>
    internal class LoggerText : TagSubscriberBase, IDisposable
    {
        public LoggerText(string filename)
        {
            Filename = filename;
            textWriter = new StreamWriter(Filename);
            textWriter.WriteLine("New Logger created: " + System.DateTime.Now.ToShortDateString() + " at " +
                System.DateTime.Now.ToShortTimeString());
        }
        /// <summary>
        /// Get the Filename associated with this instance of LoggerText.
        /// </summary>
        public string Filename
        {
            get;
            private set;
        }

        /// <summary>
        /// A <typeparamref name="TagReadEventArgs.TagMessageDelegate"/> method that is used to 
        /// handle the events and write to the text file.
        /// </summary>
        /// <param name="sender">The object that sent the event (also the object that was
        /// subscribed to).</param>
        /// <param name="tagInfo">The <typeparamref name="TagReadEventArgs"/> for the current reading.</param>
        internal override void LogTag(object sender, TagReadEventArgs e)
        {
            TagInfo tagInfo = e.TagInfo;
            ITagEventPublisher network = sender as ITagEventPublisher;
            if (network != null)
            {
                textWriter.WriteLine("  Tag ID: {0}  Antenna: {1}  Signal Strength: {2} Frequency: {3}  Time: {4}",
                    tagInfo.ID.Value, tagInfo.Antenna, tagInfo.SignalStrenth, tagInfo.Frequency, tagInfo.Time);
            }
        }

        #region IDisposable Members
        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // Take yourself off the Finalization queue 
            // to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if(!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if(disposing)
                {
                    // Dispose managed resources.
                    // Close the file and make sure all data was written to the file.
                    textWriter.Close();
                }
                // Release unmanaged resources. If disposing is false, 
                // only the following code is executed.
                // Persist any data here as well.
                

                // Note that this is not thread safe.
                // Another thread could start disposing the object
                // after the managed resources are disposed,
                // but before the disposed flag is set to true.
                // If thread safety is necessary, it must be
                // implemented by the client.

            }
            disposed = true;         
        }

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method 
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~LoggerText()      
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion

        private static int maxReadersDefault = 4;
        private List<ITagEventPublisher> rfidReaders = new List<ITagEventPublisher>(maxReadersDefault);
        private List<ITagEventPublisher> tagPassings = new List<ITagEventPublisher>(maxReadersDefault);
        TextWriter textWriter;
        private bool disposed = false;
    }
}
