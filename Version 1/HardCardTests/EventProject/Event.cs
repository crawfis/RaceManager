using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Hardcard.Scoring;

namespace EventProject
{
    /// <summary>
    /// This class represents an event.
    /// </summary>
    [Serializable]
    public class Event
    {
        public int ID { get; set; }
        //should there be a list of EventEntries, not competitors?
        //one Competitor can compete in, say, two races at a single event,
        //thus it doesn't make much sense to associate an Event with a competitor,
        //but rather an Event with EventEntry
        public List<Competitor> competitors { get; set; }
        public BindingList<Class> classes { get; set; }
        public String name { get; set; }
        public String city { get; set; }
        public String state { get; set; }
        public BindingList<DateTime> dates { get; set; }
        public BindingList<Race> races { get; set; }

        public Event(String name, String city, String state, BindingList<DateTime> dates)
            : this()
        {
            this.name = name;
            this.city = city;
            this.state = state;
            this.dates = dates;
        }

        public Event()
        {
            ID = DataManager.getNextID();
            competitors = new List<Competitor>();
            dates = new BindingList<DateTime>();
            classes = new BindingList<Class>();
            races = new BindingList<Race>();
        }
        
        public void ExportEventInformation()
        {
            //TODO: implement
        }

        public override string ToString()
        {
            return name + " at " + city + ", " + state;
        }
    }
}
