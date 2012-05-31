using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace HardCard.Scoring.Core
{
    /// <summary>
    /// This struct represents a competitor.
    /// </summary>
    [Serializable]
    public class Competitor
    {
        public delegate void PropertyChanged(Competitor source);
        [field: NonSerialized]
        public event PropertyChanged PropertyChangedEvent;

        private int id;
        public int ID 
        { 
            get { return id; }
            set { id = value; OnPropertyChanged(this); }
        }

        private String lastName;
        public String LastName 
        {
            get { return lastName; }
            set { lastName = value; OnPropertyChanged(this); }
        }
        private String firstName;
        public String FirstName 
        {
            get { return firstName; }
            set { firstName = value; OnPropertyChanged(this); }
        }

        private Address address;
        public Address Address 
        {
            get { return address; }
            set { address = value; OnPropertyChanged(this); }
        }

        private PhoneNumber phoneNumber;
        public PhoneNumber Phone 
        {
            get { return phoneNumber; }
            set { phoneNumber = value; OnPropertyChanged(this); }
        }

        private DateTime dob;
        public DateTime DOB 
        {
            get { return dob; }
            set { dob = value; OnPropertyChanged(this); }
        }

        private int age;
        public int Age 
        {
            get { return age; }
            set { age = value; OnPropertyChanged(this); }
        }

        private bool gender;
        public bool Gender 
        {
            get { return gender; }
            set { gender = value; OnPropertyChanged(this); }
        }

        private String sponsors;
        public String Sponsors 
        {
            get { return sponsors; }
            set { sponsors = value; OnPropertyChanged(this); }
        }

        private String bikeBrand;
        public String BikeBrand 
        {
            get { return bikeBrand; }
            set { bikeBrand = value; OnPropertyChanged(this); }
        }

        private String bikeNumber;
        [DisplayName("Comp Number")]
        public String BikeNumber 
        {
            get { return bikeNumber; }
            set { bikeNumber = value; OnPropertyChanged(this); }
        }

        private TagId tagNumber;
        public TagId TagNumber 
        {
            get { return tagNumber; }
            set { tagNumber = value; OnPropertyChanged(this); }
        }

        private TagId tagNumber2;
        public TagId TagNumber2
        {
            get { return tagNumber2; }
            set { tagNumber2 = value; OnPropertyChanged(this); }
        }

        public bool propagateChanges = false;

        public Competitor(int id, String lastName, String firstName, Address address, PhoneNumber phone,
                            DateTime dob, int age, bool gender, String sponsors, String bikeBrand, String bikeNumber, TagId tagNumber, TagId tagNumber2)
            : this()
        {
            this.ID = id;
            this.LastName = lastName;
            this.FirstName = firstName;
            this.Address = address;
            this.Phone = phone;
            this.DOB = dob;
            this.Age = age;
            this.Gender = gender;
            this.Sponsors = sponsors;
            this.BikeBrand = bikeBrand;
            this.BikeNumber = bikeNumber;
            this.TagNumber = tagNumber;
            this.TagNumber2 = tagNumber2;
        }

        public Competitor()
        {
            LastName = "unknown";
            FirstName = "unknown";
            Address = new Address();
            Phone = new PhoneNumber();
            DOB = DateTime.Now;
            Age = 21;
            Gender = true;
            Sponsors = "";
            BikeBrand = "";
            BikeNumber = "1234";
            TagNumber = new TagId();
            TagNumber2 = new TagId();
        }

        protected void OnPropertyChanged(Competitor source)
        {
            if (PropertyChangedEvent != null)
            {
                PropertyChangedEvent(source);
            }
        }

        public String ToStringListValues()
        {
            return 
                this.ID + "$" +
                this.LastName + "$" +
                this.FirstName + "$" +
                this.Address + "$" +
                this.Phone + "$" +
                this.DOB + "$" +
                this.Age + "$" +
                this.Gender + "$" +
                this.Sponsors + "$" +
                this.BikeBrand + "$" +
                this.BikeNumber + "$" +
                this.TagNumber + "$" +
                this.TagNumber2;
        }

        public String ToStringListValuesUpdated()
        {
            return
                ProcessField(this.ID) + "," +
                ProcessField(this.LastName) + "," +
                ProcessField(this.FirstName) + "," +
                ProcessField(this.Address) + "," +
                ProcessField(this.Phone) + "," +
                ProcessField(this.DOB) + "," +
                ProcessField(this.Age) + "," +
                ProcessField(this.Gender) + "," +
                ProcessField(this.Sponsors) + "," +
                ProcessField(this.BikeBrand) + "," +
                ProcessField(this.BikeNumber) + "," +
                ProcessField(this.TagNumber) + "," +
                ProcessField(this.TagNumber2);
        }

        private String ProcessField(object obj)
        {
            if(obj == null) return "";

            String stringRepresentation = obj.ToString();

            try
            {
                stringRepresentation = stringRepresentation.Replace("\"", "\"\"");
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.StackTrace);
            }

            if(stringRepresentation.Contains(","))
                return "\"" + stringRepresentation + "\"";

            return stringRepresentation;
        }
    }
}
