using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hardcard.Scoring;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace EventProject
{
    /// <summary>
    /// This struct represents an entry for an event.
    /// For example, when a person is going to participate in a 
    /// particular race, an EventEntry object is created; the same
    /// person may participate in different races at the same Event.
    /// </summary>
    [Serializable()]
    public class EventEntry : INotifyPropertyChanged
    {
        private Competitor cmp;
        public Competitor competitor
        {
            get { return cmp; }
            set
            {
                cmp = value;
                competitorID = cmp.ID;

                //Commented out so that changes to competitor are independent with
                //changes to CompetitorRace (they are no longer synched)
                //Corresponding lines in c_PropertyChangedEvent also should be commented out
                //tagNumber = cmp.TagNumber;
                //tagNumber2 = cmp.TagNumber2;
                //bikeNumber = cmp.BikeNumber;
                //bikeBrand = cmp.BikeBrand;
                if (cmp.propagateChanges)
                {
                    tagNumber = cmp.TagNumber;
                    tagNumber2 = cmp.TagNumber2;
                    bikeNumber = cmp.BikeNumber;
                    bikeBrand = cmp.BikeBrand;
                }

                sponsors = cmp.Sponsors;

                cmp.PropertyChangedEvent += new Competitor.PropertyChanged(c_PropertyChangedEvent);
            }
        }            

        public int ID { get; set; }
        public int eventID { get; set; }
        public int competitorID { get; set; }
        public int dayID { get; set; }

        private String clName;
        public String className 
        {
            get { return clName; }
            set
            {
                if (clName == null)
                {
                    clName = value;
                    return;
                }

                if (!clName.Equals(value))
                {
                    clName = value;
                    if (this.PropertyChanged == null) return;
                    PropertyChangedEventArgs args0 = new PropertyChangedEventArgs("className");
                    PropertyChanged(this, args0);
                }
            }
        }

        private String bNumber;
        [DisplayName("Comp Number")]
        public String bikeNumber 
        {
            get { return bNumber; }
            set
            {
                if (bNumber != value)
                {
                    bNumber = value;
                    if (this.PropertyChanged == null) return;
                    PropertyChangedEventArgs args0 = new PropertyChangedEventArgs("bikeNumber");
                    PropertyChanged(this, args0);
                }
            }
        }
        
        //if tags are changed, change corresponding competitor's information!
        private TagId tagN;
        public TagId tagNumber 
        { 
            get { return tagN; }
            set 
            { 
                tagN = value;
                //make sure we don't have circular updates that
                //result in a stack overflow
                //if (cmp != null && !cmp.TagNumber.Equals(tagN))
                //    cmp.TagNumber = tagN;
                if (this.PropertyChanged == null) return;
                PropertyChangedEventArgs args0 = new PropertyChangedEventArgs("tagNumber");
                PropertyChanged(this, args0);
            }
        }

        private TagId tagN2;
        public TagId tagNumber2 
        {
            get { return tagN2; }
            set
            {
                tagN2 = value;
                //make sure we don't have circular updates that
                //result in a stack overflow
                //if (cmp != null && !cmp.TagNumber2.Equals(tagN2))
                //    cmp.TagNumber2 = tagN2;
                if (this.PropertyChanged == null) return;
                PropertyChangedEventArgs args0 = new PropertyChangedEventArgs("tagNumber2");
                PropertyChanged(this, args0);
            }
        }
        
        public String bikeBrand { get; set; }
        public String sponsors { get; set; }

        public EventEntry(int ID, int eventID, int competitorID, int dayID, String className, String number, 
            TagId tagNumber, TagId tagNumber2, String bikeBrand, String sponsors)
            : this()
        {
            this.ID = ID;
            this.eventID = eventID;
            this.competitorID = competitorID;
            this.dayID = dayID;
            this.className = className;

            this.bikeNumber = number;
            this.tagNumber = tagNumber;
            this.tagNumber2 = tagNumber2;
            this.bikeBrand = bikeBrand;
            this.sponsors = sponsors;
        }

        public EventEntry()
        {
            this.ID = DataManager.getNextID();
        }

        private void c_PropertyChangedEvent(Competitor source)
        {
            if (source == null) return;

            if (!competitor.Equals(source))
            {
                if (this.PropertyChanged == null) return;
                PropertyChangedEventArgs args0 = new PropertyChangedEventArgs("competitor");
                PropertyChanged(this, args0);
            }

            if (competitorID != source.ID)
            {
                competitorID = source.ID;
                if (this.PropertyChanged == null) return;
                PropertyChangedEventArgs args1 = new PropertyChangedEventArgs("competitorID");
                PropertyChanged(this, args1);
            }

            //
            if (!tagNumber.Equals(source.TagNumber) && source.propagateChanges)
            {
                tagNumber = source.TagNumber;
                if (this.PropertyChanged == null) return;
                PropertyChangedEventArgs args2 = new PropertyChangedEventArgs("tagNumber");
                PropertyChanged(this, args2);
            }
            if (!tagNumber2.Equals(source.TagNumber2) && source.propagateChanges)
            {
                tagNumber2 = source.TagNumber2;
                if (this.PropertyChanged == null) return;
                PropertyChangedEventArgs args3 = new PropertyChangedEventArgs("tagNumber2");
                PropertyChanged(this, args3);
            }
            //if (bikeNumber != source.BikeNumber && source.propagateChanges)
            if (bikeNumber != null && !bikeNumber.Equals(source.BikeNumber) && source.propagateChanges)
            {
                bikeNumber = source.BikeNumber;
                if (this.PropertyChanged == null) return;
                PropertyChangedEventArgs args4 = new PropertyChangedEventArgs("bikeNumber");
                PropertyChanged(this, args4);
            }
            if (bikeBrand != null && !bikeBrand.Equals(source.BikeBrand) && source.propagateChanges)
            {
                bikeBrand = source.BikeBrand;
                if (this.PropertyChanged == null) return;
                PropertyChangedEventArgs args5 = new PropertyChangedEventArgs("bikeBrand");
                PropertyChanged(this, args5);
            }
            //
            
            if (!sponsors.Equals(source.Sponsors))
            {
                sponsors = source.Sponsors;
                if (this.PropertyChanged == null) return;
                PropertyChangedEventArgs args6 = new PropertyChangedEventArgs("sponsors");
                PropertyChanged(this, args6);
            }
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        //have to reset listeners after deserialization manually
        //works fine! has to do the same in other classes as well
        [OnDeserialized]
        private void OnDeserialized(StreamingContext c) 
        {
            Console.WriteLine("Deserializing EventEntry with ID: " + ID);
            DataManager.Log("Deserializing EventEntry with ID: " + ID);

            if (cmp == null) return;
            //c_PropertyChangedEvent(cmp);//not necessary

            cmp.PropertyChangedEvent += new Competitor.PropertyChanged(c_PropertyChangedEvent);
        }

        public String ToStringListValues()
        {
            return
                ProcessField(this.competitor != null ? competitor.ID + "" : "") + "," +
                ProcessField(this.competitor != null ? competitor.LastName : "") + "," +
                ProcessField(this.competitor != null ? competitor.FirstName : "") + "," +
                ProcessField(this.ID) + "," +
                ProcessField(this.bikeBrand) + "," +
                ProcessField(this.bikeNumber) + "," +
                ProcessField(this.className) + "," +
                ProcessField(this.tagNumber) + "," +
                ProcessField(this.tagNumber2);
        }

        private String ProcessField(object obj)
        {
            if (obj == null) return "";

            String stringRepresentation = obj.ToString();

            try
            {
                stringRepresentation = stringRepresentation.Replace("\"", "\"\"");
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.StackTrace);
            }

            if (stringRepresentation.Contains(","))
                return "\"" + stringRepresentation + "\"";

            return stringRepresentation;
        }
    }
}
