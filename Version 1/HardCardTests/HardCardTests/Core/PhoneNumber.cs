using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hardcard.Scoring
{
    /// <summary>
    /// This struct represents a phone number.
    /// </summary>
    [Serializable]
    public struct PhoneNumber
    {
        public String Number { get; set; }

        public PhoneNumber(String pn)
            : this()
        {
            this.Number = pn;
        }

        public override String ToString()
        {
            return Number;
        }
    }
}
