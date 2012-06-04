using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using RacingEventsTrackSystem.Model;
using RacingEventsTrackSystem.DataAccess;
using System.Windows;

// UE 2012/02/19 new module for events
namespace RacingEventsTrackSystem.Model
{
    public class EventRepository
    {
        private HardcardEntities hardcardContext;
        private List<Event> _eventStore;

        public EventRepository(HardcardEntities i_hardcardContext)
        {
            try
            {
                hardcardContext =i_hardcardContext;
                //hardcardContext = new HardcardEntities();  
                IQueryable<Event> events = 
                    from p in hardcardContext.Events 
                    where p.Deleted == false 
                    orderby p.EventLocation
                    select p;
                _eventStore = events.ToList();
            }
            catch (Exception ex) {
                /*Need better error logging or display*/
                //System.Console.WriteLine(ex.Message);
                MessageBox.Show("Exception in EventRepository():" + ex.Message);
            }
       }

        public void Update()
        {
            try
            {
                IQueryable<Event> events =
                    from p in hardcardContext.Events
                    where p.Deleted == false
                    orderby p.EventLocation
                    select p;
                _eventStore = events.ToList();
            }
            catch (Exception ex)
            {
                /*Need better error logging or display*/
                //System.Console.WriteLine(ex.Message);
                MessageBox.Show("Exception in Event.Update():" + ex.Message);
            }
        }

        public void Save(Event i_event)
        {
            if (!_eventStore.Contains(i_event))
            {
                /*auto-increment field for competitorID did not seem to 
                 work with LinQ for Data Entity Model*/
                long newid = 1;
                if ((from p in hardcardContext.Events
                     select p.EventID).Count() > 0)
                {
                    newid = (from p in hardcardContext.Events
                             select p.EventID).Max() + 1;

                }

                i_event.EventID = newid;
                
                // fixed to save new events
                i_event.Deleted = false;

                hardcardContext.AddToEvents(i_event);
                hardcardContext.SaveChanges();
                _eventStore.Add(i_event);
            }
            else
            {
                hardcardContext.SaveChanges();
            }
        }

        public void Delete(Event i_event)
        {
            i_event.Deleted = true;
            hardcardContext.SaveChanges();
            _eventStore.Remove(i_event);
        }

        /// <summary>
        /// EU Insensitive search for substring in First or Last name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>

        public List<Event> FindByLookup(string name)
        {
            IEnumerable<Event> found =
                from c in _eventStore
                where (c.EventName.ToLower().Contains(name.ToLower())
                    || c.EventLocation.ToLower().Contains(name.ToLower()))
                select c;
            return found.ToList();
        }
    
        /*
        public List<Event> FindByLookup(string name)
        {
            IEnumerable<Event> found =
                from c in _eventStore
                where ( c.EventName.StartsWith( name, StringComparison.OrdinalIgnoreCase) 
                     || c.EventLocation.StartsWith(name, StringComparison.OrdinalIgnoreCase))
                select c;
            return found.ToList();
        }
        */
        public List<Event> FindAll()
        {
            return new List<Event>(_eventStore);
        }
    }
}
