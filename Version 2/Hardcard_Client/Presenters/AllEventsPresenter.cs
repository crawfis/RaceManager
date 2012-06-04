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
                if (_allEvents.Count() > 0 ) 
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
            _applicationPresenter.HardcardContext.SaveChanges();
            Event newEvent = new Event();
            newEvent.EventName = "Unknown";
            newEvent.EventLocation = "USA";
            SaveEvent(newEvent);
            CurrentEvent = newEvent;
            OpenEvent(newEvent);
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
            var hc = _applicationPresenter.HardcardContext;
            if (myEvent == null) return;
            //hc.SaveChanges();
            Event dbEvent = null;
            // If myEvent is in DataContext.Events then just update it
            if (IsInEvent(myEvent))
            {
                //update DataContext.RaceClasses
                dbEvent = hc.Events.Single(e => e.Id == myEvent.Id);
                hc.ApplyCurrentValues(dbEvent.EntityKey.EntitySetName, myEvent);
                StatusText = string.Format("Event '{0}' was updated.", myEvent.ToString());
            }
            else
            {
                long max_id = 0;
                if ((from c in hc.Events select c).Count() > 0) 
                    max_id = (from e in hc.Events select e.Id).Max();
                myEvent.Id = ++max_id;
                hc.Events.AddObject(myEvent);
            }
            
            int i = AllEvents.IndexOf(myEvent);
            if (i >= 0)
            {
                //Update in Collection
                AllEvents.RemoveAt(i);
                //dbEvent = hc.Events.Single(e => e.Id == myEvent.Id);//
                //AllEvents.Insert(i, dbEvent);
                AllEvents.Insert(i, myEvent);
                CurrentEvent = myEvent;
            }
            else
            {
                AllEvents.Add(myEvent);
            }

            hc.SaveChanges();
            OpenEvent(myEvent);
            StatusText = string.Format( "Event '{0}' was saved.", myEvent.EventName);
        }


        // 
        // Delete myEvent DataContext.Events and AllEvents Collection. Don't reset Current Event.
        //
        public void DeleteEvent(Event myEvent)
        {
            if (myEvent == null) return;
            var hc = _applicationPresenter.HardcardContext;
            hc.SaveChanges();
            // Delete from DataContext
            if (IsInEvent(myEvent))
            {
                // check if there is reference to this Athlete in Competitor table
                if (IsEventInEventClass(myEvent))
                {
                    string str = string.Format("Remove record from EventClass first for Event.Id = {0}",
                           myEvent.Id);
                    MessageBox.Show(str);
                    return;
                }
                else
                {
                    //delete from DataContext
                    hc.Events.DeleteObject(hc.Events.Single(e => e.Id == myEvent.Id));
                    StatusText = string.Format("Athlete '{0}' was deleted.", myEvent.ToString());
                }

            }

            // Delete from Collection            
            if (AllEvents.Contains(myEvent))
            {
                AllEvents.Remove(myEvent);
                OpenEvent(new Event());
            }
            hc.SaveChanges();
            StatusText = string.Format("Event '{0}' was deleted.", myEvent.EventName);
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
            var hc = _applicationPresenter.HardcardContext;
            return (hc.EventClasses.Count(ec => ec.EventId == myEvent.Id) == 0) ? false : true;
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

        
        public void  SetEventDependents(Event myEvent)
        {
            // Set Competitors, EventClasses, and Sessions for CurrentEvent
            // Actually it should reset Sessions only when Session view is called??!!!!!!!!!!!!!
            if (myEvent == null) return;

            var allCompetitorsPresenter = _applicationPresenter.AllCompetitorsPresenter;
            if (allCompetitorsPresenter == null) return;
            allCompetitorsPresenter.AllCompetitors = allCompetitorsPresenter.InitCompetitorsForEvent(myEvent);

            AllEventClasses = InitAllEventClasses(myEvent);

            // Set Sessions for myEvent
            var sessionsPresenter = _applicationPresenter.AllSessionsPresenter;
            if (sessionsPresenter == null) return;
            sessionsPresenter.SessionsForEvent = sessionsPresenter.InitSessionsForEvent(myEvent);
        }

        // 
        // Create EventClasses collection for Current Event
        //
        public ObservableCollection<EventClass> InitAllEventClasses(Event myEvent)
        {
            if (myEvent == null)
            {
                CurrentEventClass = null;
                return new ObservableCollection<EventClass>(); 
            }
            else if (myEvent.EventClasses.Count() == 0)
            {
                CurrentEventClass = null;
                return new ObservableCollection<EventClass>(); 
            }
            CurrentEventClass = myEvent.EventClasses.First();
            return new ObservableCollection<EventClass>(myEvent.EventClasses.ToList());
        }

    
    }
}
