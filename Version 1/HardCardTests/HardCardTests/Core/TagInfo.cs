using System;

namespace HardCard.Scoring
{
    /// <summary>
    /// A data structure holding information about a tag reading.
    /// </summary>
    [Serializable]
    public struct TagInfo
    {
        /// <summary>
        /// Constructor requiring data for each field.
        /// </summary>
        /// <param name="id">The <typeparamref name="TadId"/>.</param>
        /// <param name="frequency">The signals frequency as a float.</param>
        /// <param name="signalStrength">The signal strength in XXX units as a float.</param>
        /// <param name="antenna">The antenna id or reference that detected the read.</param>
        /// <param name="time">The time the tag was detected. Typically encoded as the
        /// number of milliseconds from a past reference date in time (e.g., 1/1/1990)</param>
        public TagInfo(TagId id, float frequency, float signalStrength, int antenna, long time, int hits = 1) : this()
        {
            ID = id;
            Frequency = frequency;
            SignalStrenth = signalStrength;
            Antenna = antenna;
            Time = time;
            Hits = hits;
        }

        public int Hits { get; set; }

        /// <summary>
        /// Get or set the TagId.
        /// </summary>
        public TagId ID { get; set; }

        /// <summary>
        /// Get or set the frequency.
        /// </summary>
        public float Frequency { get; set; }

        /// <summary>
        /// Get or set the strength of the signal.
        /// </summary>
        public float SignalStrenth { get; set; }

        /// <summary>
        /// Get of set the antenna identifier.
        /// </summary>
        public int Antenna { get; set; }

        /// <summary>
        /// Get of set the time in milliseconds since a past reference date and time.
        /// </summary>
        public long Time { get; set; }

        /// <summary>
        /// Get or set the time in a <typeparamref name="DateTime"/> structure.
        /// </summary>
        public DateTime DateTime
        {
            get
            {
                // TODO; Check this. Not sure it is correct.
                return referenceTime.AddTicks(Time * TicksToMillisecondsFactor);
            }
            set
            {
                // TODO; Check this. Not sure if this is correct.
                Time = (value.Ticks - referenceTime.Ticks) / TicksToMillisecondsFactor;
            }
        }

        /// <summary>
        /// Get the <typeparamref name="DateTime"/> of the reference time used to the Time property 
        /// to a current date and time.
        /// </summary>
        public static DateTime ReferenceTime
        {
            get
            {
                return referenceTime;
            }
        }

        public static bool FloatsEqual(float arg1, float arg2)
        {
            return Math.Abs(arg1 - arg2) < 0.0001F;
        }
        public static bool AreEqual(TagInfo tag1, TagInfo tag2)
        {
            if (tag1.ID.Value != tag2.ID.Value)
                return false;
            if (FloatsEqual(tag1.Frequency, tag2.Frequency))
                return false;
            if (tag1.Antenna != tag2.Antenna)
                return false;
            if (FloatsEqual(tag1.SignalStrenth, tag2.SignalStrenth))
                return false;
            if (tag1.Time != tag2.Time)
                return false;

            return true;
        }

        private static DateTime referenceTime = new DateTime(1970, 1, 1);
        private static long TicksToMillisecondsFactor = 10000;
    }
}
