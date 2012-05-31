using System.Collections.Generic;

namespace HardCard.Scoring
{
    /// <summary>
    /// This class subscribes to the tag detected events and uses a pluggable
    /// strategy to determine when a tag passes the line. Many detections occur
    /// as the tag enters the antenna range, passes under or above the antenna and
    /// then exists the antenna range. This class is responsible for taking all of
    /// these readings (detections) and notifying subscribers of a "passing". It condenses
    /// each set of readings to a single pass per tag.
    /// </summary>
    public class TagPassing : ITagEventSubscriber, ITagEventPublisher
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public TagPassing(string name)
        {
            this.Name = name;
            TagDetected += NullListener.Instance.IgnoreEvent;
        }

        #region ITagEventPublisher Members
        public event TagEventHandler TagDetected;

        public string Name
        {
            get;
            set;
        }

        #endregion

        //public static IPassingStrategy PassingHandler { get; set; }

        /// <summary>
        /// Adds a new <typeparamref name="NetworkListener"/> to subscribe to and
        /// listen for TagDetected events. This is how the class knows about Tags 
        /// and when they are activated by the antenna.
        /// </summary>
        /// <param name="rfidReader">The <typeparamref name="NetworkListener"/> to listen to.</param>
        public void AddPublisher(ITagEventPublisher rfidReader)
        {
            rfidReaders.Add(rfidReader);
            rfidReader.TagDetected += new TagEventHandler(TagDetectedHandler);
        }

        /// <summary>
        /// Register a new Tag to listen for.
        /// </summary>
        /// <param name="tagId">The <typeparamref name="TagId"/> of the new Tag.</param>
        /// <remarks>If the tag already exists or is registered, no error is generated,
        /// but it may be logged.</remarks>
        public void AddTag(TagId tagId)
        {
            if (passingStrategy.ContainsKey(tagId))
            {
                // TODO: Log a message or error, throw an exception?
            }
            // TODO: We need a factory or someway to make this configurable.
            // TODO: Should we allow different strategies per tag? If so, how?
            passingStrategy[tagId] = new PassingTimeDumb();
        }

        /// <summary>
        /// A handler for the TagDetected events.
        /// </summary>
        /// <param name="sender">The object instance that fired the event.</param>
        /// <param name="tagId">The <typeparamref name="TagId"/> of the detected Tag.</param>
        /// <remarks>If the Tag has not been registered, then the detection will be ignored.</remarks>
        internal void TagDetectedHandler(object sender, TagReadEventArgs e)
        {
            TagInfo tagInfo = e.TagInfo;
            // TODO: Ignores the tag ID if it is not already registered.
            if (!passingStrategy.ContainsKey(tagInfo.ID))
            {
                AddTag(tagInfo.ID);
            }
            List<TagInfo> tagList = new List<TagInfo>(1);
            tagList.Add(tagInfo);
            TagInfo passingTime = passingStrategy[tagInfo.ID].HandlePassing(tagList);
            {
                // Fire the Passing event.
                TagDetected(this, new TagReadEventArgs(TagEventType.PassDetermined, passingTime));
            }
        }

        private static int maxReadersDefault = 4;
        private List<ITagEventPublisher> rfidReaders = new List<ITagEventPublisher>(maxReadersDefault);
        private List<TagInfo> tagRecords = new List<TagInfo>();
        private Dictionary<TagId, IPassingStrategy> passingStrategy = new Dictionary<TagId, IPassingStrategy>();
    }
}
