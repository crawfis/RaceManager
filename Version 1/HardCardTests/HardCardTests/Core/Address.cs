using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hardcard.Scoring
{
    /// <summary>
    /// This struct represents an address.
    /// </summary>
    [Serializable]
    public struct Address
    {
        public String AddressLine { get; set; }

        public String City { get; set; }
        
        public String State { get; set; }

        public int Zip { get; set; }

        public Address(String addressLn, String ct, String st, int zp)
            : this()
        {
            this.AddressLine = addressLn;
            this.City = ct;
            this.State = st;
            this.Zip = zp;
        }

        public override String ToString()
        {
            String zipRepresentation = "" + Zip;
            try
            {
                if (zipRepresentation.Length == 9)
                    zipRepresentation = zipRepresentation.Substring(0, 5) + "-" + zipRepresentation.Substring(5);
            }
            catch
            { }

            return AddressLine + "; " + City + "; " + State + "; " + zipRepresentation;
        }
    }
}
