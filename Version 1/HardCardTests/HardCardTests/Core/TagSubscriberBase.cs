using System.Collections.Generic;

namespace Hardcard.Scoring
{
    /// <summary>
    /// This class is fairly dumb. It requires the user to send in a delegate 
    /// which is called to handle the actual tag message. This is useful for 
    /// keeping the data bindings or Sql connections separate from the tag logging.
    /// </summary>
    public abstract class TagSubscriberBase : ITagEventSubscriber
    {
        public TagSubscriberBase()
        {
            this.Count = 0;
        }

        public int Count { get; private set; }

        /// <summary>
        /// Register a <typeparamref name="NetworkListener"/> to this logger and subscribe
        /// to the TagDetected event which sends a message everytime a tag is detected in the
        /// cloud.
        /// </summary>
        /// <param name="rfidReader">The <typeparamref name="NetworkListener"/> to register.</param>
        public void AddPublisher(ITagEventPublisher rfidReader)
        {
            rfidReaders.Add(rfidReader);
            rfidReader.TagDetected += new TagEventHandler(LogTagController);
        }

        private void LogTagController(object sender, TagReadEventArgs tagInfo)
        {
            Count++;
            LogTag(sender, tagInfo);
        }

        /// <summary>
        /// A <typeparamref name="TagReadEventArgs.TagMessageDelegate"/> method that is used to 
        /// handle the events.
        /// </summary>
        /// <param name="sender">The object that sent the event (also the object that was
        /// subscribed to).</param>
        /// <param name="tagInfo">The <typeparamref name="TagReadEventArgs"/> for the current reading.</param>
        internal abstract void LogTag(object sender, TagReadEventArgs tagInfo);

        private static int maxReadersDefault = 4;
        private List<ITagEventPublisher> rfidReaders = new List<ITagEventPublisher>(maxReadersDefault);
    }
}
