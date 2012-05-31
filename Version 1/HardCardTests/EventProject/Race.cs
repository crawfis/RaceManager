using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Hardcard.Scoring;

namespace EventProject
{
    [Serializable()]
    public class Race
    {
        public int ID { get; set; }
        public String name { get; set; }
        public String type { get; set; }
        //public BindingList<CompetitorRace> competitorRaceList;
        public SortableBindingList<CompetitorRace> competitorRaceList;
        public BindingList<Class> validClasses;

        //keeps tags not associated with any CompetitorRace
        public BindingList<PassingsInfo> passings;
        private BindingList<DateTime> dates;
        public BindingList<DateTime> Dates
        {
            get
            {
                if (dates == null)
                {
                    dates = new BindingList<DateTime>();
                }
                return dates;
            }
            set
            {
                dates = value;
            }
        }

        public List<TagId> tagClashFormList = new List<TagId>();
        public Dictionary<TagId, CompetitorRace> disambiuationCRDict = new Dictionary<TagId, CompetitorRace>();

        public Race(String name)
            : this()
        {
            this.name = name;
        }
        
        public Race(String name, String type) : this()
        {            
            this.name = name;
            this.type = type;
        }

        public Race()
        {
            this.ID = DataManager.getNextID();
            this.type = "";
            this.competitorRaceList = new SortableBindingList<CompetitorRace>();//new BindingList<CompetitorRace>();
            //this.competitorRaceList = new BindingList<CompetitorRace>();
            this.validClasses = new BindingList<Class>();
            this.passings = new BindingList<PassingsInfo>();
            this.dates = new BindingList<DateTime>();
        }

        public override string ToString()
        {
            //return name;
            if (type.Length > 0)
                return name + " (" + type + ")";
            
            return name;
        }

        /// <summary>
        /// This method should be used for removing competitor race
        /// from race instead of using competitorRaceList directly,
        /// because this method properly handles passings info objects
        /// that are associated with the race.
        /// </summary>
        public bool RemoveCompetitorRace(CompetitorRace cr)
        {
            if (!competitorRaceList.Contains(cr))
                return false;

            //passings from the removed cr should
            //be copied to the passings list
            List<PassingsInfo> toRemove = new List<PassingsInfo>();

            foreach (PassingsInfo pi in cr.passings)
            {
                pi.competitorID = -1;
                pi.firstName = "";
                pi.lastName = "";
                pi.CompetitorRace = null;

                if (!toRemove.Contains(pi))
                    toRemove.Add(pi);
            }

            //make sure passings are sorted before they are added to the race
            toRemove.Sort(delegate(PassingsInfo p1, PassingsInfo p2)
            {
                if (p1.Time < p2.Time) return -1;
                else if (p1.Time > p2.Time) return 1;
                else return 0;
            });

            //
            CompetitorRace crNull = FindNullCR(cr.tagID);//, cr.tagID2);
            CompetitorRace crNull2 = FindNullCR(cr.tagID2);
            if (crNull == null)
            {
                crNull = new CompetitorRace();
                crNull.isNull = true;
                crNull.competitorID = (-1) * DataManager.getNextID();
                crNull.tagID = cr.tagID;
                crNull.tagID2 = cr.tagID;
                crNull.bikeNumber = cr.bikeNumber;
                crNull.firstName = "";
                crNull.lastName = "Unassigned";
                crNull.raceParent = this;
            }

            if (crNull2 == null)
            {
                crNull2 = new CompetitorRace();
                crNull2.isNull = true;
                crNull2.competitorID = (-1) * DataManager.getNextID();
                crNull2.tagID = cr.tagID2;
                crNull2.tagID2 = cr.tagID2;
                crNull2.bikeNumber = cr.bikeNumber;
                crNull2.firstName = "";
                crNull2.lastName = "Unassigned";
                crNull2.raceParent = this;
            }
            //
            
            foreach (PassingsInfo pi in toRemove)
            {
                cr.passings.Remove(pi);

                //OLD VERSION: copied to this.passings
                //if (!this.passings.Contains(pi))
                //    this.passings.Add(pi);

                //instead, add this to crNull:
                if (!crNull.passings.Contains(pi) && pi.ID.Equals(crNull.tagID))
                {
                    crNull.passings.Add(pi);
                    pi.competitorID = crNull.competitorID;
                    pi.firstName = crNull.firstName;
                    pi.lastName = crNull.lastName;
                    pi.CompetitorRace = crNull;
                }

                if (!crNull2.passings.Contains(pi) && pi.ID.Equals(crNull2.tagID))
                {
                    crNull2.passings.Add(pi);
                    pi.competitorID = crNull2.competitorID;
                    pi.firstName = crNull2.firstName;
                    pi.lastName = crNull2.lastName;
                    pi.CompetitorRace = crNull2;
                }
            }
            //add new crNull to the list of CRs:
            if(crNull.passings.Count > 0)
                competitorRaceList.Add(crNull);
            if(crNull2.passings.Count > 0)
                competitorRaceList.Add(crNull2);

            competitorRaceList.Remove(cr);

            
            if(disambiuationCRDict.Values.Contains(cr))
            {
                TagId foundKey;
                foreach (TagId k in disambiuationCRDict.Keys)
                {
                    if (disambiuationCRDict[k] == cr)
                        foundKey = k;
                }
                if(disambiuationCRDict.ContainsKey(foundKey))
                    disambiuationCRDict.Remove(foundKey);
            }

            return true;
        }

        /// <summary>
        /// This method should be used for adding competitor race
        /// from race instead of using competitorRaceList directly,
        /// because this method properly handles passings info objects
        /// that are associated with the race.
        /// </summary>
        public bool AddCompetitorRace(CompetitorRace cr)
        {
            if (competitorRaceList.Contains(cr))
                return false;

            competitorRaceList.Add(cr);
            cr.raceParent = this;

            //List<PassingsInfo> toRemove = new List<PassingsInfo>();
            
            //foreach (PassingsInfo pi in this.passings)
            //{
            //    if (cr.checkTag(pi.ID))
            //    {
            //        pi.competitorID = cr.competitorID;
            //        pi.firstName = cr.firstName;
            //        pi.lastName = cr.lastName;
            //        pi.CompetitorRace = cr;

            //        toRemove.Add(pi);
            //    }
            //}

            ////make sure passings are sorted before they are added
            //toRemove.Sort(delegate(PassingsInfo p1, PassingsInfo p2)
            //{
            //    if (p1.Time < p2.Time) return -1;
            //    else if (p1.Time > p2.Time) return 1;
            //    else return 0;
            //});

            //foreach (PassingsInfo pi in toRemove)
            //{
            //    cr.passings.Add(pi);
            //    this.passings.Remove(pi);
            //}

            //instead:
            CompetitorRace crNull = FindNullCR(cr.tagID);//, cr.tagID2);
            CompetitorRace crNull2 = FindNullCR(cr.tagID2);
            if (crNull != null)
            {
                //copy passings from crNull to cr
                List<PassingsInfo> toRemove = new List<PassingsInfo>();

                foreach (PassingsInfo pi in crNull.passings)
                {
                    if (cr.checkTag(pi.ID))
                    {
                        pi.competitorID = cr.competitorID;
                        pi.firstName = cr.firstName;
                        pi.lastName = cr.lastName;
                        pi.CompetitorRace = cr;

                        if (!toRemove.Contains(pi))
                            toRemove.Add(pi);
                    }
                }

                //make sure passings are sorted before they are added
                toRemove.Sort(delegate(PassingsInfo p1, PassingsInfo p2)
                {
                    if (p1.Time < p2.Time) return -1;
                    else if (p1.Time > p2.Time) return 1;
                    else return 0;
                });

                foreach (PassingsInfo pi in toRemove)
                {
                    cr.passings.Add(pi);
                    if(crNull.passings.Contains(pi))
                        crNull.passings.Remove(pi);
                }
                //finally, remove crNull altogether:
                this.competitorRaceList.Remove(crNull);
            }
            else
            {
                //do nothing
            }

            if (crNull2 != null)
            {
                //copy passings from crNull to cr
                List<PassingsInfo> toRemove = new List<PassingsInfo>();

                foreach (PassingsInfo pi in crNull2.passings)
                {
                    if (cr.checkTag(pi.ID))
                    {
                        pi.competitorID = cr.competitorID;
                        pi.firstName = cr.firstName;
                        pi.lastName = cr.lastName;
                        pi.CompetitorRace = cr;

                        if(!toRemove.Contains(pi))
                            toRemove.Add(pi);
                    }
                }

                //make sure passings are sorted before they are added
                toRemove.Sort(delegate(PassingsInfo p1, PassingsInfo p2)
                {
                    if (p1.Time < p2.Time) return -1;
                    else if (p1.Time > p2.Time) return 1;
                    else return 0;
                });

                foreach (PassingsInfo pi in toRemove)
                {
                    cr.passings.Add(pi);
                    if (crNull2.passings.Contains(pi))
                        crNull2.passings.Remove(pi);
                }
                //finally, remove crNull altogether:
                this.competitorRaceList.Remove(crNull2);
            }


            cr.SortPassings();

            return true;
        }

        //when a cr's tag id (s) change, try to re-match passings
        //1. if some nullCRs exist, look if their passings can be remapped to cr and remove nullCR
        //2. if passings in cr become unmapped (cr's tag id no longer matches passings'), create null CRs 
        //and copy passings there; first check if such nullCR exists already
        public void ResetPassings(CompetitorRace cr)
        {
            //1. Find, whether exising nullCRs' passings can be reassociated with the new cr,
            //and if yes, perform this reassociation
            List<PassingsInfo> toRemove = new List<PassingsInfo>();//possibly removed from nullCRs
            foreach (CompetitorRace cmpRace in competitorRaceList)
            {
                if (!cmpRace.isNull)
                    continue;

                foreach (PassingsInfo pi in cmpRace.passings)
                {
                    if (cr.checkTag(pi.ID))
                    {
                        toRemove.Add(pi);
                    }
                }
            }

            //now we have a list of passings that can be re-associated with cr:
            foreach (PassingsInfo pi in toRemove)
            {
                if (!cr.passings.Contains(pi))
                {
                    pi.competitorID = cr.competitorID;
                    pi.firstName = cr.firstName;
                    pi.lastName = cr.lastName;
                    pi.CompetitorRace = cr;
                    cr.passings.Add(pi);
                }
            }
            //finally, remove those passings from nullCRs
            foreach (CompetitorRace cmpRace in competitorRaceList)
            {
                if (!cmpRace.isNull)
                    continue;

                foreach (PassingsInfo pi in toRemove)
                {
                    if (cmpRace.passings.Contains(pi))
                        cmpRace.passings.Remove(pi);
                }
            }

            //remove nullCRs if their passings info is empty - that means they are not needed any longer
            List<CompetitorRace> nullCRsToRemove = new List<CompetitorRace>();
            foreach (CompetitorRace cmpRace in competitorRaceList)
            {
                if (!cmpRace.isNull)
                    continue;

                if (cmpRace.passings.Count == 0)
                    nullCRsToRemove.Add(cmpRace);
            }

            foreach (CompetitorRace cmpRace in nullCRsToRemove)
            {
                if(competitorRaceList.Contains(cmpRace))
                    competitorRaceList.Remove(cmpRace);
            }

            //2. on the other hand, if cr's tags no longer match its PassingsInfo,
            //create nullCRs for them and copy stuff over

            List<PassingsInfo> toAdd = new List<PassingsInfo>();
            TagId tagID1 = new TagId("-1");
            TagId tagID2 = new TagId("-1");
            foreach (PassingsInfo pi in cr.passings)
            {
                if (!cr.checkTag(pi.ID))
                {                    
                    toAdd.Add(pi);

                    //if there are two different ids in the tags for cr (which is possible),
                    //have to get both of them
                    if (tagID1.Value.Equals("-1"))
                        tagID1 = pi.ID;
                    else if (tagID1.Value.Equals(pi.ID.Value))
                        continue;
                    else if (tagID2.Value.Equals("-1"))
                        tagID2 = pi.ID;
                    else if (tagID2.Value.Equals(pi.ID.Value))
                        continue;
                }
            }

            if (toAdd.Count == 0)
                return;

            //search for existing nullCRs that may be used
            CompetitorRace nullCR = null;
            CompetitorRace nullCR2 = null;
            //foreach (CompetitorRace cmpRace in competitorRaceList)
            //{
            //    if (!cmpRace.isNull)
            //        continue;

            //    if (cmpRace.tagID.Equals(tagID1))
            //    {
            //        nullCR = cmpRace;
            //    }
            //}

            if (nullCR == null)
            {
                nullCR = new CompetitorRace();
                nullCR.isNull = true;
                nullCR.competitorID = (-1) * DataManager.getNextID();
                nullCR.tagID = tagID1;
                nullCR.tagID2 = tagID1;
                nullCR.bikeNumber = "0";
                nullCR.firstName = "";
                nullCR.lastName = "Unassigned";
                nullCR.raceParent = this;
            }

            if (nullCR2 == null)
            {
                nullCR2 = new CompetitorRace();
                nullCR2.isNull = true;
                nullCR2.competitorID = (-1) * DataManager.getNextID();
                nullCR2.tagID = tagID2;
                nullCR2.tagID2 = tagID2;
                nullCR2.bikeNumber = "0";
                nullCR2.firstName = "";
                nullCR2.lastName = "Unassigned";
                nullCR2.raceParent = this;
            }

            foreach (PassingsInfo pi in toAdd)
            {
                if (!nullCR.passings.Contains(pi) && pi.ID.Equals(nullCR.tagID))
                {
                    nullCR.passings.Add(pi);
                    pi.competitorID = nullCR.competitorID;
                    pi.firstName = nullCR.firstName;
                    pi.lastName = nullCR.lastName;
                    pi.CompetitorRace = nullCR;
                }

                if (!nullCR2.passings.Contains(pi) && pi.ID.Equals(nullCR2.tagID))
                {
                    nullCR2.passings.Add(pi);
                    pi.competitorID = nullCR2.competitorID;
                    pi.firstName = nullCR2.firstName;
                    pi.lastName = nullCR2.lastName;
                    pi.CompetitorRace = nullCR2;
                }
                
                if (cr.passings.Contains(pi))
                    cr.passings.Remove(pi);
            }

            if (!competitorRaceList.Contains(nullCR) && nullCR.passings.Count > 0)
                competitorRaceList.Add(nullCR);

            if (!competitorRaceList.Contains(nullCR2) && nullCR2.passings.Count > 0)
                competitorRaceList.Add(nullCR2);

        }

        public void ReassociatePassings(CompetitorRace newCR)
        {
            //use carefully! this should be used only when deambiguating
            //CRs with same tags, and the newCR is given all passings from
            //other (fully legitimate) CRs
            
            //this method should be called after resetPassings(), which
            //moves passings from null-CRs (if any of them exist)

            //newCR should be already in the list!
            //iterate through all non-null CRs and move their passings to newCR
            List<PassingsInfo> toAdd = new List<PassingsInfo>();
            foreach (CompetitorRace cr in competitorRaceList)
            {
                if (cr.isNull || cr.Equals(newCR)) 
                    continue;

                if (newCR.checkTag(cr.tagID) || newCR.checkTag(cr.tagID2))
                {
                    foreach (PassingsInfo pi in cr.passings)
                    {
                        if (!newCR.passings.Contains(pi))
                        {
                            toAdd.Add(pi);
                        }
                    }
                }
            }

            //remove from "old" CRs
            foreach (CompetitorRace cr in competitorRaceList)
            {
                if (cr.isNull || cr.Equals(newCR))
                    continue;

                foreach (PassingsInfo pi in toAdd)
                {
                    if (cr.passings.Contains(pi))
                        cr.passings.Remove(pi);
                }
            }

            //finally, reassociate all "toAdd" passings
            foreach (PassingsInfo pi in toAdd)
            {
                pi.competitorID = newCR.competitorID;
                pi.firstName = newCR.firstName;
                pi.lastName = newCR.lastName;
                pi.CompetitorRace = newCR;
            }

        }

        public String ToStringListValuesUpdated()
        {
            return
                ProcessField(name) + "," +
                ProcessField(type) + "," +
                ProcessField(((Dates.Count > 0) ? Dates[0].ToString() : "")) + "," +
                ProcessField(GetClasses());
        }

        public String GetClasses()
        {
            String result = "";
            foreach(Class c in validClasses)
            {
                result += (c.name + ";" + c.description) + ";";
            }

            return result;
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

        private CompetitorRace FindNullCR(TagId tagID)
        {
            foreach (CompetitorRace cr in competitorRaceList)
            {
                if (cr.checkTag(tagID) && cr.isNull)
                    return cr;
            }

            return null;
        }

        private CompetitorRace FindNullCR(TagId tagID, TagId tagID2)
        {
            foreach (CompetitorRace cr in competitorRaceList)
            {
                if (cr.checkTag(tagID) && cr.isNull)
                    return cr;

                if (cr.checkTag(tagID2) && cr.isNull)
                    return cr;
            }

            return null;
        }
    }
}
