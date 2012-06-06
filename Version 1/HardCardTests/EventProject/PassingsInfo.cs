using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Hardcard.Scoring;
using System.Globalization;
using System.Text.RegularExpressions;

namespace EventProject
{
    /// <summary>
    /// Auxiliary class to be used to display data in passings data grid.
    /// Please note that we keep an instance of the competitor (when we
    /// can get such instance), which allows us to update it so that
    /// the changes are visible in all the views that (may be) associated
    /// with this Competitor object.
    /// </summary>
    [Serializable()]
    public class PassingsInfo : INotifyPropertyChanged
    {
        public TagId ID { get; set; }
        [DisplayName("Comp Number")]
        public String CompetitionNumber { get; set; }//previously bikeNumber
        public float Frequency { get; set; }
        public float SignalStrength { get; set; }
        public int Antenna { get; set; }
        public long Time { get; set; }
        public DateTime DateTime { get; set; }
        public int Hits { get; set; }

        public int competitorID { get; set; }
        [DisplayName("First Name")]
        public String firstName { get; set; }
        [DisplayName("Last Name")]
        public String lastName { get; set; }

        private String deleted;
        public String Deleted
        {
            get { return deleted; }
            set 
            {
                deleted = value;
                if (value == null)
                    return;

                PropertyChangedEventArgs args1 = new PropertyChangedEventArgs("Deleted");
                PropertyChanged(this, args1);
            }
        }

        private CompetitorRace competitorRace;
        public CompetitorRace CompetitorRace
        {
            get { return competitorRace; }
            set
            {
                if (competitorRace != null)
                    competitorRace.PropertyChanged -= competitorRace_PropertyChanged;

                competitorRace = value;
                if (value == null)
                    return;
                
                this.firstName = competitorRace.firstName;
                this.lastName = competitorRace.lastName;
                this.competitorID = competitorRace.competitorID;
                this.CompetitionNumber = competitorRace.bikeNumber;

                competitorRace.PropertyChanged += competitorRace_PropertyChanged;
            }
        }

        //note, string is because we need here a string representation
        //of the time
        private String lapTime;
        [DisplayName("Lap Time")]
        public String LapTime
        {
            get { return lapTime; }

            set 
            {
                lapTime = value;
                if (value == null)
                    return;

                PropertyChangedEventArgs args1 = new PropertyChangedEventArgs("LapTime");
                PropertyChanged(this, args1);
            }
        }

        private void competitorRace_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CompetitorRace source = sender as CompetitorRace;
            if (source == null || !source.Equals(competitorRace))
                return;

            if (firstName != null && source.firstName != null && !firstName.Equals(source.firstName))
            {
                firstName = source.firstName;
                if (PropertyChanged == null) return;
                PropertyChangedEventArgs args1 = new PropertyChangedEventArgs("firstName");
                PropertyChanged(this, args1);
            }
            if(lastName != null && source.lastName != null && !lastName.Equals(source.lastName))
            {
                lastName = source.lastName;
                if (PropertyChanged == null) return;
                PropertyChangedEventArgs args2 = new PropertyChangedEventArgs("lastName");
                PropertyChanged(this, args2);
            }
            if (competitorID != source.competitorID)
            {
                competitorID = source.competitorID;
                if (PropertyChanged == null) return;
                PropertyChangedEventArgs args3 = new PropertyChangedEventArgs("competitorID");
                PropertyChanged(this, args3);
            }
            if (CompetitionNumber != source.bikeNumber)
            {
                CompetitionNumber = source.bikeNumber;
                if (PropertyChanged == null) return;
                PropertyChangedEventArgs args4 = new PropertyChangedEventArgs("CompetitionNumber");
                PropertyChanged(this, args4);
            }
        }

        //please note that sometimes we may not know competitor,
        //so the field stays null
        //private Competitor competitor;
        //public Competitor Competitor
        //{
        //    get { return competitor; }
        //    set
        //    {
        //        if (value != null)
        //        {
        //            //remove listener from old competitor
        //            if (competitor != null) //don't do the very first time
        //                competitor.PropertyChangedEvent -= this.competitor_PropertyChangedEvent;

        //            competitor = value;

        //            this.firstName = value.FirstName;
        //            this.lastName = value.LastName;
        //            this.competitorID = value.ID;

        //            //competitor.PropertyChangedEvent +=
        //            //    new Hardcard.Scoring.Core.Competitor.PropertyChanged(competitor_PropertyChangedEvent);
        //            competitor.PropertyChangedEvent += this.competitor_PropertyChangedEvent;
        //        }
        //        else
        //        {
        //            //doesn't seem to remove the handler
        //            //competitor.PropertyChangedEvent -= 
        //            //    new Hardcard.Scoring.Core.Competitor.PropertyChanged(competitor_PropertyChangedEvent);
        //            if (competitor != null)
        //                competitor.PropertyChangedEvent -= this.competitor_PropertyChangedEvent;
        //            competitor = null;
        //        }
        //    }
        //}

        //private void competitor_PropertyChangedEvent(Competitor source)
        //{
        //    if (source == null) return;

        //    //prevents "resetting" properties when we have already re-associated 
        //    //this PassingInfo object to some other competitor object
        //    //proper way of doing this - remove competitor listener at all, but
        //    //it doesn't seem to work all the times, some kind of bug
        //    if (competitorID != source.ID) return;

        //    this.firstName = source.FirstName;
        //    this.lastName = source.LastName;
        //    this.competitorID = source.ID;//should not change, actually

        //    //necessary to send those events so that UI is updated
        //    PropertyChangedEventArgs args1 = new PropertyChangedEventArgs("firstName");
        //    PropertyChanged(this, args1);
        //    PropertyChangedEventArgs args2 = new PropertyChangedEventArgs("lastName");
        //    PropertyChanged(this, args2);
        //    PropertyChangedEventArgs args3 = new PropertyChangedEventArgs("competitorID");
        //    PropertyChanged(this, args3);
        //}

        public PassingsInfo(TagInfo tagInfo, int competitorID, String firstName, String lastName, String competitionNum)
        {
            this.ID = tagInfo.ID;
            this.Frequency = tagInfo.Frequency;
            this.SignalStrength = tagInfo.SignalStrenth;
            this.Antenna = tagInfo.Antenna;
            this.Time = tagInfo.Time;
            this.DateTime = tagInfo.DateTime;
            this.Hits = tagInfo.Hits;

            this.competitorID = competitorID;
            this.firstName = firstName;
            this.lastName = lastName;
            this.CompetitionNumber = competitionNum;
            this.deleted = "";
            this.lapTime = "-1";
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            String fullPattern = DateTimeFormatInfo.CurrentInfo.FullDateTimePattern;
            fullPattern = Regex.Replace(fullPattern, "(:ss|:s)", "$1.fff");

            return 
                "TagID=" + ID + ",Frequency=" + Frequency + ",SignalStrength=" + SignalStrength + 
                ",Antenna=" + Antenna + ",Time=" + new DateTime(Time).ToString(fullPattern) + ",DateTime=" + DateTime.ToString(fullPattern) + 
                ",Hits=" + Hits + ",competitorID=" + competitorID + ",firstName=" + firstName + ",lastName=" + lastName;
        }

        public string ToStringListValues()
        {
            String fullPattern = DateTimeFormatInfo.CurrentInfo.FullDateTimePattern;
            fullPattern = Regex.Replace(fullPattern, "(:ss|:s)", "$1.fff");

            return ID + "," + Frequency + "," + SignalStrength + "," + Antenna + ",\"" + new DateTime(Time).ToString(fullPattern) + "\",\"" +
                DateTime.ToString(fullPattern) + "\"," + Hits + "," + competitorID + "," + CompetitionNumber + "," + ProcessField(firstName) + "," + ProcessField(lastName) + "," + ProcessField(lapTime) +
                "," + ProcessField(deleted);
        }

        public string ToStringListValuesHTML()
        {
            String fullPattern = DateTimeFormatInfo.CurrentInfo.FullDateTimePattern;
            fullPattern = Regex.Replace(fullPattern, "(:ss|:s)", "$1.fff");

            return "<td>" + ID + "</td><td>" + Frequency + "</td><td>" + SignalStrength + "</td><td>" + Antenna + "</td><td>" + new DateTime(Time).ToString(fullPattern) + "</td><td>" +
                DateTime.ToString(fullPattern) + "</td><td>" + Hits + "</td><td>" + competitorID + "</td><td>" + ProcessField(firstName) + "</td><td>" + ProcessField(lastName) + "</td><td>" + 
                ProcessField(lapTime) + "</td><td>" + ProcessField(deleted) + "</td>";
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

        private String GetTimeRepresentation(long time)
        {
            return "" + time;
        }
    }
}
