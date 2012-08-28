using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using RacingEventsTrackSystem.Model;
using RacingEventsTrackSystem.Views;
using RacingEventsTrackSystem.DataAccess;
using RacingEventsTrackSystem.Presenters;
using System.Windows;
using System.Data.Objects;

using System.Data;
using System.Data.SqlClient;
using System.Data.Sql;
using Microsoft.Win32;
using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using Microsoft.SqlServer.Management.Common;
using System.Windows.Shapes;


namespace RacingEventsTrackSystem.Presenters
{
    public partial class AllEventsPresenter : PresenterBase<Shell>
    {
        private readonly ApplicationPresenter _applicationPresenter;
        // Keeps Events existing in DataBase
        private ObservableCollection<Event> _allEvents;
        private Event _currentEvent;

        // Keeps EventClasses for the CurrentEvent, any control changing CurrentEvent, has to reset that collection.
        private ObservableCollection<EventClass> _allEventClasses;
        // EventClass selected in _allEventClasses list.
        private EventClass _currentEventClass;                    
        private string _statusText;                              


        public AllEventsPresenter(ApplicationPresenter applicationPresenter, 
                                  Shell view
                                  ) : base(view)
        {
            try 
            {
                var hc = applicationPresenter.HardcardContext;
                _applicationPresenter = applicationPresenter;
                _allEventClasses = new ObservableCollection<EventClass>();
                _allEvents = new ObservableCollection<Event>(hc.Events);
                if (_allEvents.Count() > 0) //??? it should be in SetEventDependents
                { 
                    _currentEvent =_allEvents.First();
                    _allEventClasses = new ObservableCollection<EventClass>(_currentEvent.EventClasses);
                    if (_allEventClasses.Count() > 0)
                    {
                        _currentEventClass = _allEventClasses.First();
                    }
                 }
                SetEventDependents(_currentEvent);
                StatusText = ("AllEventsPresenter constructor no error");
            } 
            catch (Exception ex)
            {
                StatusText = "AllEventsPresenter constructor failed with error: " + ex.Message;
                MessageBox.Show(StatusText); // stop executable
            }
        }

        public ApplicationPresenter ApplicationPresenter
        {
            get { return _applicationPresenter; }
            set {}
        }

        public ObservableCollection<Event> AllEvents
        {
            get { return _allEvents; }
            set
            {
                _allEvents = value;
                OnPropertyChanged("AllEvents");
            }
        }

        public Event CurrentEvent
        {
            get { return _currentEvent; }
            set
            {
                _currentEvent = value;
                SetEventDependents(_currentEvent);
                OnPropertyChanged("CurrentEvent");
            }
        }
        public ObservableCollection<EventClass> AllEventClasses
        {
            get { return _allEventClasses; }
            set
            {
                _allEventClasses = value;
                OnPropertyChanged("AllEventClasses");
            }
        }

        public EventClass CurrentEventClass
        {
            get { return _currentEventClass; }
            set
            {
                _currentEventClass = value;
                OnPropertyChanged("CurrentEventClass");
            }
        }

        public string StatusText
        {
            get { return _statusText; }
            set
            {
                _statusText = value;
                OnPropertyChanged("StatusText");
            }
        }

        public void Search(string criteria)
        {
            if (!string.IsNullOrEmpty(criteria) && criteria.Length > 0)
            {
                AllEvents = new ObservableCollection<Event>(FindByLookup(criteria));
                StatusText = string.Format("{0} events found.", AllEvents.Count);
            }
            else
            {
                AllEvents = new ObservableCollection<Event>(_applicationPresenter.HardcardContext.Events);
                StatusText = "Displaying all events.";
            }
        }

        //
        // Search for substring in EventName or EventLocation
        //
        public List<Event> FindByLookup(string name)
        {
            IEnumerable<Event> found =
                from c in _applicationPresenter.HardcardContext.Events
                where (c.EventName.ToLower().Contains(name.ToLower())
                    || c.EventLocation.ToLower().Contains(name.ToLower()))
                select c;
            return found.ToList();
        }

        //
        // Create new event. It doesn't add the event to collection or entity. 
        // 
        public void CreateNewEvent()
        {
            var hc = _applicationPresenter.HardcardContext; 
            Event newEvent = new Event();

            newEvent.Id = 0;
            long max_id = 0;
            if ((from c in hc.Events select c).Count() > 0) // not empty table
            {
                max_id = (from e in hc.Events select e.Id).Max();
                newEvent.Id = ++max_id;
            }

            newEvent.EventName = "Event" + newEvent.Id.ToString();
            newEvent.EventLocation = "USA";
            newEvent.StartDate = DateTime.Now;
            newEvent.EndDate = DateTime.Now;
            hc.Events.AddObject(newEvent);
            hc.SaveChanges();
            CurrentEvent = newEvent;
            AllEvents = UpdateAllEvents();

            //OpenEvent(newEvent);
        }
        
        //
        // Update existing Event or add new entry if Event is not in DataContext.Events
        //
        // If this Event is in Event table then it will be updated in DataContext, otherwise it will be added to 
        // the DataContext.
        // Same for Events Collection.
        // The method submits DataContext in the DataBase.
        //

        public void SaveEvent(Event myEvent)
        {
            if (myEvent == null) return;
            if (ValidateEvent(myEvent) == false) return; // not valid input parameters
            _applicationPresenter.HardcardContext.SaveChanges();
            StatusText = string.Format("Event '{0}' was saved.", myEvent.EventName);
        }

        //
        // Returns false if some input data for the session are not match the DataBase constraints
        // (!!!Replace with interective WPF validation during editing)
        public bool ValidateEvent(Event myEvent)
        {
            if (AllEvents.Count(ec => ec.Id == myEvent.Id) == 0)
            {
                MessageBox.Show("Event is not in the AllEvents list");
                return false;
            }
            return true;
        }


        // 
        // Delete myEvent DataContext.Events.
        //
        public void DeleteEvent(Event myEvent)
        {
            if (CurrentEvent == null) return;
            var hc = _applicationPresenter.HardcardContext;
            // Delete from DataContext
            if (CurrentEvent.EventClasses.Count() > 0)
            {
                // check if there is reference to this Event in EventClass table
                while (CurrentEvent.EventClasses.Count() > 0)
                {
                    DeleteEventClass(CurrentEvent.EventClasses.First());
                    //hc.EventClasses.DeleteObject(myEvent.EventClasses.First());
                }
            }
            hc.SaveChanges();
            hc.Events.DeleteObject(CurrentEvent);
            hc.SaveChanges();
        }

        // 
        // Delete myEvent DataContext.Events and AllEvents Collection.
        //
        public void DeleteCurrentEvent()
        {
            if (CurrentEvent == null) return;
            // Delete from DataContext
            if (CurrentEvent.EventClasses.Count() > 0)
            {
                string str = string.Format("All data : Sessions, Competitors, ect. will be deleted for this event Event = '{0}'",
                           CurrentEvent.EventName);
                System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox.Show(str, "Warning!",
                    System.Windows.Forms.MessageBoxButtons.YesNo);
                if (result == System.Windows.Forms.DialogResult.No) return;
            }

            string str1 = CurrentEvent.EventName;
            DeleteEvent(CurrentEvent);
            AllEvents = InitAllEvents();
            StatusText = string.Format("Event '{0}' was deleted.", str1);
        }


        // 
        // Delete eventClass from DataContext. 
        //
        public void DeleteEventClass(EventClass eventClass)
        {
            if (eventClass == null) return;
            var hc = _applicationPresenter.HardcardContext;
            hc.SaveChanges();
            if (eventClass.Competitors.Count() > 0)
            {
                while (eventClass.Competitors.Count() > 0)
                    ApplicationPresenter.AllCompetitorsPresenter.DeleteCompetitor(eventClass.Competitors.First());
            }

            if (eventClass.Sessions.Count() > 0)
            {
                while (eventClass.Sessions.Count() > 0)
                {
                    ApplicationPresenter.AllSessionsPresenter.DeleteSessionFromEvent(eventClass.Sessions.First());
                }
            }
            string str = eventClass.RaceClass.ClassName;
            hc.SaveChanges();
            hc.EventClasses.DeleteObject(eventClass);
            hc.SaveChanges();

            StatusText = string.Format("eventClass '{0}' was deleted.", str);
        }

        // 
        // Delete Current eventClass from DataContext and Collection. 
        //
        public void DeleteCurrentEventClass()
        {
            if (CurrentEventClass == null) return;
            if (CurrentEvent == null) return;

            var hc = _applicationPresenter.HardcardContext;
            hc.SaveChanges();
            DeleteEventClass(CurrentEventClass);
            hc.SaveChanges();
            AllEventClasses = InitAllEventClasses(CurrentEvent);
        }
        //
        // Returns true if myEvent.Id exists in the Event table
        //
        private bool IsInEvent(Event myEvent)
        {
            var hc = _applicationPresenter.HardcardContext;
            return (hc.Events.Count(s => s.Id == myEvent.Id) == 0) ? false : true;
        }

        //
        // Returns true if EventClass table has FK for input myEvent
        //
        private bool IsEventInEventClass(Event myEvent)
        {
            //???var hc = _applicationPresenter.HardcardContext;
            //???return (hc.EventClasses.Count(ec => ec.EventId == myEvent.Id) == 0) ? false : true;
            return myEvent.EventClasses.Count() > 0; 

        }

        public void OpenEvent(Event myEvent)
        {
            if (myEvent == null) return;
            View.ShowEvent( new EventPresenter( this, new EventView(), myEvent),
                            View.eventView);
        }

        public void DisplayCurrentEvent()
        {
            if(_currentEvent != null)
                OpenEvent(_currentEvent);
        }

        // 
        // Create Events collection and set default CurrentEvent 
        //
        public ObservableCollection<Event> InitAllEvents()
        {
            ObservableCollection<Event> Tmp = new ObservableCollection<Event>();
            CurrentEvent = null;

            var hc = _applicationPresenter.HardcardContext;
            List<Event>  query = (from c in hc.Events select c).ToList();
            if (query.Count() > 0)
            {
                Tmp = new ObservableCollection<Event>(query);
                CurrentEvent = Tmp.First();
            }

            return Tmp;
        }
        // 
        // Update Events collection 
        //
        public ObservableCollection<Event> UpdateAllEvents()
        {
            ObservableCollection<Event> Tmp = new ObservableCollection<Event>();
            var hc = _applicationPresenter.HardcardContext;
            var query = (from c in hc.Events select c);
            if (query.Count() > 0)
                Tmp = new ObservableCollection<Event>(query);

            return Tmp;
        }
  
        public void  SetEventDependents(Event myEvent)
        {
            // Set Competitors, EventClasses, and Sessions for CurrentEvent
            // Actually it should reset Sessions only when Session view is called??!!!!!!!!!!!!!
            if (myEvent == null) return;

            AllEventClasses = InitAllEventClasses(myEvent);
            
            var allCompetitorsPresenter = _applicationPresenter.AllCompetitorsPresenter;
            if (allCompetitorsPresenter != null)
            allCompetitorsPresenter.AllCompetitors = allCompetitorsPresenter.InitCompetitorsForEvent(myEvent);

            // Set Sessions for myEvent
            var sessionsPresenter = _applicationPresenter.AllSessionsPresenter;
            if (sessionsPresenter != null)
            sessionsPresenter.SessionsForEvent = sessionsPresenter.InitSessionsForEvent(myEvent);
        }

        // 
        // Create EventClasses collection for Current Event
        //
        public ObservableCollection<EventClass> InitAllEventClasses(Event myEvent)
        {
            ObservableCollection<EventClass> Tmp;
            if (myEvent == null)
                Tmp = new ObservableCollection<EventClass>();
            else
                Tmp = new ObservableCollection<EventClass>(myEvent.EventClasses.ToList());
  
            if (Tmp.Count() > 0)
                CurrentEventClass = Tmp.First();
            else
                CurrentEventClass = null;

            return Tmp;
        }


        // Copy Value from new event to event in Db
        public void UpdateEvent(Event newEvent, Event dbEvent)
        {
            if (newEvent == null || dbEvent == null) return;
            dbEvent.EventName = newEvent.EventName;
            dbEvent.EventLocation = newEvent.EventLocation;
            dbEvent.StartDate = newEvent.StartDate;
            dbEvent.EndDate = newEvent.EndDate;
            dbEvent.Deleted = newEvent.Deleted;
        }
    }
}
