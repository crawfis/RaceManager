using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hardcard.Scoring;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace EventProject
{
    [Serializable]
    public class CompetitorRace : INotifyPropertyChanged
    {
        //this field is used to check whether 
        //this CompetitorRace has any associated Competitor
        //TODO: change to a method that checks null/not null Competitor
        //(don't need separate field for this)
        public bool isNull = false;

        private Competitor cmp;
        public Competitor competitor
        {
            get { return cmp; }
            set 
            {
                if (cmp != null)
                    cmp.PropertyChangedEvent -= c_PropertyChangedEvent;

                cmp = value;
                if (cmp == null)//sometimes we may have null competitors
                    return;
                cmp.PropertyChangedEvent += c_PropertyChangedEvent;
                competitorID = cmp.ID;

                //Commented out so that changes to competitor are independent with
                //changes to CompetitorRace (they are no longer synched)
                //Corresponding lines in c_PropertyChangedEvent also should be commented out
                //tagID = cmp.TagNumber;
                //tagID2 = cmp.TagNumber2;
                //bikeNumber = cmp.BikeNumber;
                
                firstName = cmp.FirstName;
                lastName = cmp.LastName;

                //GetDataForCompetitor();               
                //cmp.PropertyChangedEvent += new Competitor.PropertyChanged(c_PropertyChangedEvent);
            }
        }

        public int competitorID { get; set; }
        //public List<DateTime> passings;
        public BindingList<PassingsInfo> passings;

        //data fields to be filled by corresponding competitor object
        //(have to do it so that grid view can utilize this data)
        private TagId tid;
        public TagId tagID 
        {
            get { return tid; }
            set 
            {
                if (!tid.Equals(value))
                {
                    tid = value;
                    RecheckPassings();

                    if (this.PropertyChanged == null) return;
                    PropertyChangedEventArgs args0 = new PropertyChangedEventArgs("tagID");
                    PropertyChanged(this, args0);
                }
            }
        }

        private TagId tid2;
        public TagId tagID2 
        {
            get { return tid2; }
            set
            {
                if (!tid2.Equals(value))
                {
                    tid2 = value;
                    RecheckPassings();

                    if (this.PropertyChanged == null) return;
                    PropertyChangedEventArgs args0 = new PropertyChangedEventArgs("tagID2");
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

        [DisplayName("First Name")]
        public String firstName { get; set; }
        [DisplayName("Last Name")]
        public String lastName { get; set; }

        private int curPlace;
        public int Position 
        {
            get { return curPlace; }
            set
            {
                if (curPlace == value) return;

                curPlace = value;
                if (this.PropertyChanged == null) return;
                PropertyChangedEventArgs args0 = new PropertyChangedEventArgs("currentName");
                PropertyChanged(this, args0);
            }
        }
        public double lastLap { get; set; }
        public double bestLap { get; set; }
        [DisplayName("Laps Completed")]
        public int lapsCompleted { get; set; }
        public Race raceParent { get; set; }
        private EventEntry eventEntry;
        public EventEntry EventEntry
        {
            get { return eventEntry; }
            set
            {
                if (eventEntry != null)
                    eventEntry.PropertyChanged -= eventEntryPropertyChangedHandler;

                eventEntry = value;
                if (eventEntry != null)
                {
                    this.className = eventEntry.className;
                    this.tagID = eventEntry.tagNumber;
                    this.tagID2 = eventEntry.tagNumber2;
                    this.bikeNumber = eventEntry.bikeNumber;
                    eventEntry.PropertyChanged += eventEntryPropertyChangedHandler;
                }
            }
        }

        private String bestLapStr;
        [DisplayName("Best Lap")]
        public String BestLap
        {
            get { return bestLapStr; }
            set { bestLapStr = value; }
        }

        private String lastLapStr;
        [DisplayName("Last Lap")]
        public String LastLap
        {
            get { return lastLapStr; }
            set { lastLapStr = value; }
        }

        [NonSerialized]
        public bool hasPropertyListener;
        //public double totalRunningTime { get; set; }

        /// <summary>
        /// Returns true if the given TagID is equal to one of the two tags for this
        /// competitor race. Please note that cases with trivial id ("0") are dismissed.
        /// </summary>
        public bool checkTag(TagId t)
        {
            if (t.Value == null || t.Value.Equals("0") || t.Value.Equals(""))
                return false;

            if (tagID.Value != null && !tagID.Value.Equals("0") && !tagID.Value.Equals("") && tagID.Equals(t))
                return true;

            if (tagID2.Value != null && !tagID2.Value.Equals("0") && !tagID2.Value.Equals("") && tagID2.Equals(t))
                return true;

            return false;
        }
        
        public CompetitorRace(int competitorID, BindingList<PassingsInfo> passings)//List<DateTime> passings)
            : this()
        {
            this.competitorID = competitorID;
            this.passings = passings;
            GetDataForCompetitor();
        }

        public CompetitorRace(int competitorID)
            : this()
        {
            this.competitorID = competitorID;
            GetDataForCompetitor();
        }

        public CompetitorRace()
        {
            passings = new BindingList<PassingsInfo>();
            lastLap = 0;
            bestLap = 0;
            BestLap = GetTimeString(bestLap);
            LastLap = GetTimeString(lastLap);
        }

        private void GetDataForCompetitor()
        {
            Competitor c = DataManager.Instance.GetCompetitorByID(competitorID);
            if (c != null)
            {
                tagID = c.TagNumber;
                tagID2 = c.TagNumber2;
                bikeNumber = c.BikeNumber;
                firstName = c.FirstName;
                lastName = c.LastName;

                cmp = c;
                c.PropertyChangedEvent += new Competitor.PropertyChanged(c_PropertyChangedEvent);
            }
        }

        private void eventEntryPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if(sender == null) return;

            this.className = (sender as EventEntry).className;

            //
            this.tagID = (sender as EventEntry).tagNumber;
            this.tagID2 = (sender as EventEntry).tagNumber2;
            this.bikeNumber = (sender as EventEntry).bikeNumber;

            if (this.PropertyChanged == null) return;
            PropertyChangedEventArgs args0 = new PropertyChangedEventArgs("tagID");
            PropertyChanged(this, args0);

            if (this.PropertyChanged == null) return;
            PropertyChangedEventArgs args1 = new PropertyChangedEventArgs("tagID2");
            PropertyChanged(this, args1);
            
            if (this.PropertyChanged == null) return;
            PropertyChangedEventArgs args2 = new PropertyChangedEventArgs("bikeNumber");
            PropertyChanged(this, args2);

            if (this.PropertyChanged == null) return;
            PropertyChangedEventArgs args3 = new PropertyChangedEventArgs("className");
            PropertyChanged(this, args3);
        }
        
        private void c_PropertyChangedEvent(Competitor source)
        {
            if (source == null) return;

            if (!competitor.Equals(source))//should not happen
            {
                competitor = source;
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
            //if (!tagID.Equals(source.TagNumber))
            //{
            //    //need this to pass old value
            //    PropertyUpdateEventArgs args2 = new PropertyUpdateEventArgs("tagID", tagID, source.TagNumber);
            //    tagID = source.TagNumber;
            //    if(this.PropertyUpdated != null)
            //        PropertyUpdated(this, args2);

            //    RecheckPassings();

            //    if (this.PropertyChanged == null) return;
            //    PropertyChangedEventArgs args2C = new PropertyChangedEventArgs("tagID");
            //    PropertyChanged(this, args2C);
            //}
            //if (!tagID2.Equals(source.TagNumber2))
            //{
            //    tagID2 = source.TagNumber2;
            //    RecheckPassings();
            //    if (this.PropertyChanged == null) return;
            //    PropertyChangedEventArgs args3 = new PropertyChangedEventArgs("tagID2");
            //    PropertyChanged(this, args3);
            //}
            //if (bikeNumber != source.BikeNumber)
            //{
            //    bikeNumber = source.BikeNumber;
            //    if (this.PropertyChanged == null) return;
            //    PropertyChangedEventArgs args4 = new PropertyChangedEventArgs("bikeNumber");
            //    PropertyChanged(this, args4);
            //}
            if (!firstName.Equals(source.FirstName))
            {
                firstName = source.FirstName;
                if (this.PropertyChanged == null) return;
                PropertyChangedEventArgs args5 = new PropertyChangedEventArgs("firstName");
                PropertyChanged(this, args5);
            }
            if (!lastName.Equals(source.LastName))
            {
                lastName = source.LastName;
                if (this.PropertyChanged == null) return;
                PropertyChangedEventArgs args6 = new PropertyChangedEventArgs("lastName");
                PropertyChanged(this, args6);
            }
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public delegate void PropertyUpdateHandler(object sender,  PropertyUpdateEventArgs data);
        [field: NonSerialized]
        public event PropertyUpdateHandler PropertyUpdated;
        protected void OnPropertyUpdate(object sender,  PropertyUpdateEventArgs data)
        {  
            // Check if there are any Subscribers  
            if (PropertyUpdated!= null)  
            {  
                PropertyUpdated(this, data);
            }  
        }

        //if one of the tags has changed, iterate over passings and un-associate
        //if the tags no longer match the ones in the passings
        //have to save to corresponding Race! but how? need race entity!
        private void RecheckPassings()
        {
            /////////////////
            //TODO: put this processing to raceParent!!!
            if (raceParent != null)
            {
                raceParent.ResetPassings(this);
                return;
            }
            /////////////////
            /////////////////
            /////////////////
            return;
            /*

            if (passings == null || raceParent == null || raceParent.passings == null) return;

            List<PassingsInfo> toRemove = new List<PassingsInfo>();
            foreach (PassingsInfo pi in passings)
            {
                if (!checkTag(pi.ID))
                {
                    pi.competitorID = -1;
                    pi.firstName = "";
                    pi.lastName = "";
                    pi.CompetitorRace = null;
                    toRemove.Add(pi);
                }
            }

            foreach (PassingsInfo pi in toRemove)
            {
                passings.Remove(pi);
                if(!raceParent.passings.Contains(pi))
                    raceParent.passings.Add(pi);
            }

            //what if some passings in the race (raceParent.passings) can now be re-associated?
            toRemove = new List<PassingsInfo>();
            foreach (PassingsInfo pi in raceParent.passings)
            {
                if (this.checkTag(pi.ID))
                {
                    pi.competitorID = this.competitorID;
                    pi.firstName = this.firstName;
                    pi.lastName = this.lastName;
                    pi.CompetitorRace = this;
                    if (!this.passings.Contains(pi))
                    {
                        this.passings.Add(pi);
                        toRemove.Add(pi);
                    }
                }
            }

            foreach (PassingsInfo pi in toRemove)
            {
                raceParent.passings.Remove(pi);
            }
            */
        }

        //maybe automatically calculate times when there are updates to
        //passings list?
        public int GetNumLaps()
        {
            //if there is only one passing, does this means one lap,
            //or no finished lap yet? Probably second, as we need two
            //time stamps to calculate time
            if (passings == null || passings.Count < 2)
            {
                lapsCompleted = 0;
                return 0;
            }

            //return (passings.Count - 1);
            return lapsCompleted;
        }

        public double GetLastLapTime()
        {
            if (passings == null || passings.Count < 2)
            {
                bestLap = 0;
                lapsCompleted = 0;
                BestLap = GetTimeString(bestLap);
                LastLap = GetTimeString(lastLap);
                return 0;
            }

            //make sure they are sorted!
            List<PassingsInfo> validPassings = new List<PassingsInfo>();
            foreach (PassingsInfo pi in passings)
            {
                if (pi.Deleted != null && pi.Deleted.Length == 0)
                    validPassings.Add(pi);
            }
            if (validPassings == null || validPassings.Count < 2)
            {
                bestLap = 0;
                lapsCompleted = 0;
                BestLap = GetTimeString(bestLap);
                LastLap = GetTimeString(lastLap);
                return 0;
            }

            //long latestTime = passings[passings.Count - 1].Time;
            //long nextToLatestTime = passings[passings.Count - 2].Time;
            long latestTime = validPassings[validPassings.Count - 1].Time;
            long nextToLatestTime = validPassings[validPassings.Count - 2].Time;

            double lapTime = (latestTime - nextToLatestTime) / 1000.0;

            if (bestLap == 0)//overwrite default value
                bestLap = lapTime;

            if (bestLap > lapTime)
                bestLap = lapTime;

            lapsCompleted = validPassings.Count - 1;

            BestLap = GetTimeString(bestLap);
            LastLap = GetTimeString(lastLap);

            return lapTime;
        }

        //we could have put sorting and re-calculation of best laps
        //into GetLastLapTime method, but this would most likely add
        //unnecessary re-calculations, esp. with large number of 
        //competitors and passings; thus this method should be called
        //only once a new competitor is added to/removed from a race

        /// <summary>
        /// This method sorts the passings of this CompetitorRace
        /// object and calculates the best lap and last lap time
        /// based on the passings.
        /// </summary>
        public void SortPassings()
        {
            if (passings == null) return;//should not happen
            
            List<PassingsInfo> passingsList = new List<PassingsInfo>();

            foreach (PassingsInfo pi in passings)
            {
                passingsList.Add(pi);
            }

            passingsList.Sort(delegate(PassingsInfo p1, PassingsInfo p2)
            {
                if (p1.Time < p2.Time) return -1;
                else if (p1.Time > p2.Time) return 1;
                else return 0;
            });

            passings.Clear();
            foreach (PassingsInfo pi in passingsList)
            {
                passings.Add(pi);
            }

            //some passings may have "DELETED" label, don't use them
            List<PassingsInfo> validPassings = new List<PassingsInfo>();
            foreach (PassingsInfo pi in passingsList)
            {
                if(pi.Deleted != null && pi.Deleted.Length == 0)
                    validPassings.Add(pi);
            }

            //if (passings.Count < 2)
            if (validPassings.Count < 2)
            {
                bestLap = 0;
                lastLap = 0;
                lapsCompleted = 0;
                BestLap = GetTimeString(bestLap);
                LastLap = GetTimeString(lastLap);
                return;
            }

            bestLap = 0;

            //for (int i = 1; i < passings.Count; i++)
            for (int i = 1; i < validPassings.Count; i++)
            {
                long latestTime = validPassings[i].Time;//passings[i].Time;
                long nextToLatestTime = validPassings[i - 1].Time;//passings[i - 1].Time;
                double lapTime = (latestTime - nextToLatestTime) / 1000.0;
                if (bestLap == 0 || bestLap > lapTime)
                    bestLap = lapTime;

                //
                validPassings[i].LapTime = GetTimeString(lapTime);
            }

            long latestTime2 = validPassings[validPassings.Count - 1].Time;//passings[passings.Count - 1].Time;
            long nextToLatestTime2 = validPassings[validPassings.Count - 2].Time;//passings[passings.Count - 2].Time;
            lastLap = (latestTime2 - nextToLatestTime2) / 1000.0;
            lapsCompleted = validPassings.Count - 1;

            //
            BestLap = GetTimeString(bestLap);
            LastLap = GetTimeString(lastLap);
        }

        public string ToStringListValues()
        {
            //return ProcessField(firstName) + "," + ProcessField(lastName) + "," + tagID + "," + tagID2 + "," + competitorID + "," + bikeNumber + "," +
            //    ProcessField(className) + "," + currentPlace + "," + bestLapStr + "," + lastLapStr + "," + lapsCompleted;
            String bikeBrand = (eventEntry != null) ? eventEntry.bikeBrand : "";

            return Position  + "," + bikeNumber  + "," + ProcessField(firstName) + "," + ProcessField(lastName) + "," + ProcessField(bikeBrand) + "," +
                ProcessField(className) + "," + lapsCompleted + "," + lastLapStr + "," + bestLapStr + "," + tagID + "," + tagID2 + "," + competitorID;
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

        private String GetTimeString(double time)
        {
            //time is in seconds
            int hours = (int)Math.Floor(time / 3600.0);            
            //int minutes = (int)Math.Floor(time / 60.0);
            int minutes = (int)Math.Floor((time - hours * 3600)/60.0);
            double seconds = (time - minutes * 60 - hours * 3600);

            //return "" + minutes + ":" + Math.Round(seconds, 3);
            String hoursStr = String.Format("{0:00}", hours);
            String minutesStr = String.Format("{0:00}", minutes);
            String secondsStr = String.Format("{0:00.000}", seconds);

            return hoursStr + ":" + minutesStr + ":" + secondsStr;
        }

        //
        [OnDeserialized]
        private void OnDeserialized(StreamingContext c)
        {
            Console.WriteLine("Deserializing CompetitorRace");
            DataManager.Log("Deserializing CompetitorRace");

            if (eventEntry == null)
                return;

            //this.className = eventEntry.className;
            eventEntry.PropertyChanged += eventEntryPropertyChangedHandler;
        }

    }
}
