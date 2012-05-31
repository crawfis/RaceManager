using System;

namespace HardCard.Scoring
{
    /// <summary>
    /// A data structure holding information about a tag reading.
    /// </summary>
    [Serializable]
    public class TagReadEventArgs : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>The information contained in the instance will be garbage until set.</remarks>
        /// <remarks>The default constructor is needed for XML serialization and declarative programming.</remarks>
        public TagReadEventArgs() : this(TagEventType.None, new TagInfo())
        {
            // TODO; Are there some better defaults we could use?
            // TODO; Should we make all of the properties Nullable?
        }

        /// <summary>
        /// Constructor requiring data for each field.
        /// </summary>
        /// <param name="id">The <typeparamref name="TadId"/>.</param>
        /// <param name="frequency">The signals frequency as a float.</param>
        /// <param name="signalStrength">The signal strength in XXX units as a float.</param>
        /// <param name="antenna">The antenna id or reference that detected the read.</param>
        /// <param name="time">The time the tag was detected. Typically encoded as the
        /// number of milliseconds from a past reference date in time (e.g., 1/1/1990)</param>
        public TagReadEventArgs(TagEventType eventType, TagInfo tagInfo)
        {
            this.EventType = eventType;
            this.TagInfo = tagInfo;
        }

        /// <summary>
        /// Get or set the TagId.
        /// </summary>
        public TagEventType EventType { get; set; }

        /// <summary>
        /// Get or set the TagId.
        /// </summary>
        public TagInfo TagInfo { get; set; }


    }
}
