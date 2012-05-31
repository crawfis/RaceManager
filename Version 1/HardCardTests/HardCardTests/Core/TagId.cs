
using System;

namespace HardCard.Scoring
{
    /// <summary>
    /// A user-defined type for the Tag ID. If this was
    /// C++ I would have just typedef'd this to string.
    /// I want to be able to convert this to two longs or
    /// 4 ints for efficiency, verification purposes, etc.
    /// </summary>
    [Serializable()]
    public struct TagId
    {
        /// <summary>
        /// Get and set the tag ID as a string.
        /// </summary>
        public string Value { get; set; }

        public TagId(String v):this()
        {
            this.Value = v;
        }
        
        public override string ToString()
        {
            if(Value != null)
                return Value;

            return "0";
        }
    }
}
