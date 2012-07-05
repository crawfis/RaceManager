using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RacingEventsTrackSystem.Model;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Data.EntityClient;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace RacingEventsTrackSystem.DataAccess
{
    public partial class Competitor : EntityObject
    {
        public global::System.String DataToDisplay
        {
            get
            {
                return string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8}",
                       Id, AthleteId, EventClassId, Athlete.LastName, Athlete.FirstName,
                       Athlete.City, Athlete.State, Athlete.Country, VehicleType);
            }
        }
    }
    

    public partial class Event : EntityObject
    {
        public global::System.String DataToDisplay
        {
            get
            {
                return string.Format("{0} {1}", EventName, EventLocation);
            }
        }
    }

    public partial class EventClass : EntityObject
    {
        public global::System.String DataToDisplay
        {
            get
            {
                return string.Format("{0} {1} {2} {3} {4} {5} {6} {7}",
                   Id, RaceClass.ClassName, RaceClass.MinAge, RaceClass.MaxAge,
                   RaceClass.Gender, RaceClass.VehicleType, RaceClass.VehicleModel, RaceClass.VehicleCC);
            }
        }

        public override string ToString()
        {
             return string.Format("{0} {1} {2} {3} {4} {5} {6} {7}",
                Id, RaceClass.ClassName, RaceClass.MinAge, RaceClass.MaxAge,
                RaceClass.Gender, RaceClass.VehicleType, RaceClass.VehicleModel, RaceClass.VehicleCC);
        }
    }
   
    public partial class Athlete : EntityObject
    {
        public global::System.String DataToDisplay
        {
            get
            {
                return string.Format("{0} {1} {2} {3} {4} {5} {6} ",
                       Id, LastName, FirstName,City, State, Country, Id);
            }
        }
    }


    public partial class RaceClass : EntityObject
    {
        public global::System.String DataToDisplay
        {
            get
            {
                //return string.Format("{0} {1} ", RaceClass.ClassName, RaceClass.MinAge);
                return string.Format("{0} {1} {2} {3} {4} {5} {6} {7}",
                   Id, ClassName, MinAge, MaxAge,
                   Gender, VehicleType, VehicleModel, VehicleCC);
            }
        }
    }

    public partial class Session : EntityObject
    {
        public global::System.String DataToDisplay
        {
            get
            {
                return this.ToString();
            }
        }
 
        public override string ToString()
        {
            if (EventClass == null || EventClass.RaceClass == null) return "";
            return string.Format("{0} {1} {2} {3} {4}",
                    Id, EventClass.RaceClass.ClassName, StartTime, SchedStopTime, SchedLaps);
        }
 
    }

}
