using System.Collections.Generic;
using OhioState.Collections;

namespace HardCard.Scoring
{
    /// <summary>
    /// This class subscribes to the tag detected events and buffers the readings
    /// for the <typeparamref name=""/> class to process them.
    /// </summary>
    public class BufferReadings : ITagEventSubscriber
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public BufferReadings(IPriorityCollection<TagInfo> readingsQueue)
        {
            this.readingsQueue = readingsQueue;
        }

        /// <summary>
        /// Adds a new <typeparamref name="NetworkListener"/> to subscribe to and
        /// listen for TagDetected events. This is how the class knows about Tags 
        /// and when they are activated by the antenna.
        /// </summary>
        /// <param name="rfidReader">The <typeparamref name="NetworkListener"/> to listen to.</param>
        public void AddPublisher(ITagEventPublisher rfidReader)
        {
            if (rfidReader != null)
            {
                rfidReaders.Add(rfidReader);
                rfidReader.TagDetected += new TagEventHandler(TagDetectedHandler);
            }
        }

        /// <summary>
        /// Register a new Tag to listen for.
        /// </summary>
        /// <param name="tagId">The <typeparamref name="TagId"/> of the new Tag.</param>
        /// <remarks>If the tag already exists or is registered, no error is generated,
        /// but it may be logged.</remarks>
        //public void AddTag(TagId tagId)
        //{
        //}

        /// <summary>
        /// A Handler to listen for a new Tag event.
        /// </summary>
        /// <param name="sender">The object instance that fired the event.</param>
        /// <param name="tagId">The <typeparamref name="TagId"/> of the new Tag.</param>
        /// <remarks>If the tag already exists or is registered, no error is generated,
        /// but it may be logged.</remarks>
        //internal void AddTagHandler(object sender, TagId tagId)
        //{
        //    AddTag(tagId);
        //}

        /// <summary>
        /// A handler for the TagDetected events.
        /// </summary>
        /// <param name="sender">The object instance that fired the event.</param>
        /// <param name="tagId">The <typeparamref name="TagId"/> of the detected Tag.</param>
        /// <remarks>If the Tag has not been registered, then the detection will be ignored.</remarks>
        internal void TagDetectedHandler(object sender, TagReadEventArgs e)
        {
            TagInfo tagInfo = e.TagInfo;
            // TODO: Should we make sure the Tag is registered here?
            readingsQueue.Put(tagInfo);
        }

        private static int maxReadersDefault = 4;
        private static int capacity = 1024;
        private List<ITagEventPublisher> rfidReaders = new List<ITagEventPublisher>(maxReadersDefault);
        private IPriorityCollection<TagInfo> readingsQueue = new PriorityCollectionBlocking<TagInfo>("Queue", capacity);
    }
}
