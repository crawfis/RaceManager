using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Hardcard.Scoring
{
    // TODO: Convert this over to TagSubscriberBase
    class LoggerXML : ITagEventSubscriber, IDisposable
    {
        public LoggerXML(string filename)
        {
            FileName = filename;
        }

        public string FileName { get; private set; }

        public void AddPublisher(ITagEventPublisher rfidReader)
        {
            rfidReaders.Add(rfidReader);
            rfidReader.TagDetected += new TagEventHandler(LogTag);
        }

        internal void LogTag(object sender, TagReadEventArgs tagInfo)
        {
            NetworkListener network = sender as NetworkListener;
            if (network != null)
                tagRecords.Add(tagInfo);
        }

        public void Serialize()
        {
            XmlSerializer serializer = new XmlSerializer(tagRecords.GetType());
            using (TextWriter writer = new StreamWriter(FileName))
            {
                serializer.Serialize(writer, tagRecords);
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
                }
                // Release unmanaged resources. If disposing is false, 
                // only the following code is executed.
                // Persist any data here as well.
                Serialize();
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
        ~LoggerXML()      
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion

        private static int maxReadersDefault = 4;
        private List<ITagEventPublisher> rfidReaders = new List<ITagEventPublisher>(maxReadersDefault);
        private List<TagReadEventArgs> tagRecords = new List<TagReadEventArgs>();
        private bool disposed;
    }
}
