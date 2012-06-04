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

namespace RacingEventsTrackSystem.Presenters
{
    public partial class AllRaceClassesPresenter : PresenterBase<Shell>
    {

        private readonly ApplicationPresenter _applicationPresenter;
        private ObservableCollection<RaceClass> _allRaceClasses;
        private RaceClass _currentRaceClass;
        private string _statusText;

        public AllRaceClassesPresenter( ApplicationPresenter applicationPresenter,
                                        Shell view
                                      ) : base(view)    
        {
            try 
            { 
                _applicationPresenter = applicationPresenter;
                _allRaceClasses = new ObservableCollection<RaceClass>(_applicationPresenter.HardcardContext.RaceClasses);
                if (_allRaceClasses.Count() > 0) 
                    _currentRaceClass = _allRaceClasses.First(); 
                StatusText = ("AllRaceClassesPresenter constructor no error");
            } 
            catch (Exception ex)
            {
                StatusText = "AllRaceClassesPresenter constructor failed with error: " + ex.Message;
                MessageBox.Show(StatusText); // stop executable
            }
        }


        public ObservableCollection<RaceClass> AllRaceClasses
        {
            get { return _allRaceClasses; }
            set
            {
                _allRaceClasses = value;
                OnPropertyChanged("AllRaceClasses");
            }
        }


        public RaceClass CurrentRaceClass
        {
            get { return _currentRaceClass; }
            set
            {
                _currentRaceClass = value;
                OnPropertyChanged("CurrentRaceClass");
            }
        }

        public RaceClass CurrentRaceClassForEvent
        {
            get { return _currentRaceClass; }
            set
            {
                _currentRaceClass = value;
                OnPropertyChanged("CurrentRaceClass");
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
                AllRaceClasses = new ObservableCollection<RaceClass>(FindByLookup(criteria));
                StatusText = string.Format("{0} Race Classes found.", AllRaceClasses.Count);
            }
            else
            {
                AllRaceClasses = new ObservableCollection<RaceClass>(_applicationPresenter.HardcardContext.RaceClasses);
                StatusText = "Displaying all Race Classes.";
            }
        }

        //
        // Search for substring in ClassName or VehicleType
        //
        public List<RaceClass> FindByLookup(string name)
        {
            IEnumerable<RaceClass> found =
                from c in _applicationPresenter.HardcardContext.RaceClasses
                where (c.ClassName.ToLower().Contains(name.ToLower())
                    || c.VehicleType.ToLower().Contains(name.ToLower()))
                select c;
            return found.ToList();
        }
 
        //
        // Create new RaceClass. It adds the Athlete to Collection and DataContext.RaceClasses. 
        // 
        public void CreateNewRaceClass()
        {
            RaceClass raceClass = new RaceClass();
            raceClass.ClassName = "Unknown";
            raceClass.MinAge = 21;
            raceClass.MaxAge = 90;
            raceClass.VehicleType = "Bike";
            CurrentRaceClass = raceClass;
            SaveRaceClass(raceClass);
            OpenRaceClass(raceClass);
        }

   
        //
        // Save DataContext changes in Database, if any
        //
        public void SaveRaceClasses()
        {
            _applicationPresenter.HardcardContext.SaveChanges();
        }

        //
        // Update existing RaceClass or add new entry if RaceClass is not in DataContext.RaceClasses
        //
        // If this object is in RaceClass table then it will be updated in DataContext, otherwise it will be added to 
        // the DataContext.
        // Same for Race Class Collection.
        // The method submits DataContext in the DataBase.
        //
        public void SaveRaceClass(RaceClass raceClass)
        {
            if (raceClass == null) return;
            var hc = _applicationPresenter.HardcardContext;

            // If raceClass is in RaceClasses DataContext then just update it
            if (IsInRaceClass(raceClass))
            {
                RaceClass dbRaceClass = hc.RaceClasses.Single(r => r.Id == raceClass.Id);
                hc.ApplyCurrentValues(dbRaceClass.EntityKey.EntitySetName, raceClass);
                StatusText = string.Format("RaceClass '{0}' was updated.", raceClass.ToString());
            }
            else
            {
                //Create new raceClass 
                long max_id = 0;
                if ((from c in hc.RaceClasses select c).Count() > 0) // not empty table
                {
                    max_id = (from c in hc.RaceClasses
                              select c.Id).Max();
                }
                raceClass.Id = ++max_id;
                hc.RaceClasses.AddObject(raceClass);
                StatusText = string.Format("RaceClass '{0}' was added to RaceClase table.", raceClass.ToString());
            }

            // Update AllRaceClasses Collection  
            int i = AllRaceClasses.IndexOf(raceClass);
            if ( i >= 0 )
            {
                // If raceClass is in AllRaceClasses Collection then just update it
                AllRaceClasses.RemoveAt(i);
                AllRaceClasses.Insert(i, raceClass);
                CurrentRaceClass = raceClass; // Current Rase class was just deleted before
            }
            else
            {
                AllRaceClasses.Add(raceClass);
            }

            hc.SaveChanges();
            OpenRaceClass(raceClass); 
            StatusText = string.Format("Race Class '{0}' was saved.", raceClass.ClassName);
        }

        //
        // Exclude CurrentEventClass.RaceClass from the CurrentEvent
        //
        public void ExcludeClassFromEvent()
        {
            AllEventsPresenter esp = _applicationPresenter.AllEventsPresenter;
            Event currEvent = esp.CurrentEvent;
            EventClass currEventClass = esp.CurrentEventClass;
            if (currEvent == null || currEventClass == null) return;
            string status = string.Format("RaceClass '{0}' was deleted from current Event.", esp.CurrentEventClass.RaceClass.ToString());

            var hc = _applicationPresenter.HardcardContext;
            int numEventClasses = hc.EventClasses.Count(c => c.Id == currEventClass.Id);
            if (numEventClasses == 1)
            {
                // If EventClass is in Competitor or Session tables, delete records with FK from those tables first.
                if ( hc.Competitors.Count(c => c.EventClassId == currEventClass.Id) > 0 
                  || hc.Sessions.Count(c => c.EventClassId == currEventClass.Id ) > 0)
                {
                    MessageBox.Show("Remove Competoros and Sessions for this class first.");
                    return;
                }

                hc.EventClasses.DeleteObject(currEventClass);
                hc.SaveChanges();
                StatusText = status;
            }
            else if (numEventClasses > 1)
            {
                MessageBox.Show("Error:EventClass table constraint violation."); 
            }
            if (esp.AllEventClasses.Contains(currEventClass)) esp.AllEventClasses.Remove(currEventClass);
        }

        //
        // Returns true if EventClass table has row for given Event+RaceClass
        //
        private bool isRaceClassInEventClass(Event myEvent, RaceClass raceClass)
        {
            ObjectSet<EventClass> eventClasses = this._applicationPresenter.HardcardContext.EventClasses;
            long isInEventClasses = (from ec in eventClasses
                                     where ec.EventId == myEvent.Id
                                     && ec.ClassId == raceClass.Id
                                     select ec).Count();
            return (isInEventClasses == 0) ? false : true; 
        }

        // 
        // Add Current Race Class to Current Event
        //
        public void AddRaceClassToEvent(RaceClass raceClass)
        {
            if (raceClass == null) return;
            var hc = _applicationPresenter.HardcardContext;
            Event currEvent = _applicationPresenter.AllEventsPresenter.CurrentEvent;
            if (currEvent == null)
            {
                MessageBox.Show("AddRaceClassToEvent().Warning!!! Some Event has to be choosen.");
                return;
            }
            if (raceClass == null)
            {
                MessageBox.Show("AddRaceClassToEvent().Warning!!! Some Race Class has to be choosen.");
                return;
            }


            //Save RaceClass if it was changed under new classId in RaceClass collection and RaceClass entity
            SaveRaceClass(raceClass);
            
            
            //Add new entry into EventClasses collection and EventClasses entity
            //ObjectSet<RaceClass> raceClasses = hc.RaceClasses;
            //ObjectSet<EventClass> eventClasses = hc.EventClasses;
            
            // Check if this Event+RaceClass is in EventClass Table
            // if Event+RaceClass not found in EventClass Table then create new record in the table
            if (!isRaceClassInEventClass(currEvent, raceClass))
            {
                long max_id = 0;
                if ((from c in hc.EventClasses select c.Id).Count() > 0) // not empty table
                {
                    max_id = (from c in hc.EventClasses
                     select c.Id).Max();
                }
                EventClass eventClass = new EventClass() { Id = ++max_id, EventId = currEvent.Id, ClassId = raceClass.Id };
                hc.EventClasses.AddObject(eventClass);
                hc.SaveChanges();
                //_applicationPresenter.AllEventsPresenter.InitAllEventClasses(currEvent);
                _applicationPresenter.AllEventsPresenter.AllEventClasses.Add(eventClass);
            }

            _applicationPresenter.AllEventsPresenter.InitAllEventClasses(currEvent);
            this.CurrentRaceClass = raceClass;// to keep data for current Race Class on the screen
            OpenRaceClass(raceClass);
            StatusText = string.Format("Race Class '{0}' was added.", raceClass.ClassName);
        }

        // 
        // Delete Race Class from DataContext and Collection. Don't reset Current Race Class.
        //
        public void DeleteRaceClass(RaceClass raceClass)
        {
            if (raceClass == null) return;
            var hc = _applicationPresenter.HardcardContext;

            // Check if raceClass is in RaceClass table
            if (IsInRaceClass(raceClass))
            {
                // check if there is reference to this RaceClass in EventClass table
                if (IsRaceClassInEventClass(raceClass))
                {
                    string str = string.Format("Remove record from EventClass first for RaceClass.Id = {0}. Check all Events.",
                           raceClass.Id);
                    MessageBox.Show(str);
                    return;
                }
                else
                {
                    //delete from DataContext
                    hc.RaceClasses.DeleteObject(hc.RaceClasses.Single(p => p.Id == raceClass.Id));
                    StatusText = string.Format("Athlete '{0}' was deleted.", raceClass.ToString());
                }
            }

            // Delete from Collection            
            if (AllRaceClasses.Contains(raceClass))
            {
                AllRaceClasses.Remove(raceClass);
                OpenRaceClass(new RaceClass());
            }

            StatusText = string.Format("Class '{0}' was deleted.", raceClass.ClassName);
        }

        public void OpenRaceClass(RaceClass raceClass)
        {
            if (raceClass == null) return; 
            if (_applicationPresenter.AllEventsPresenter.CurrentEvent == null)
            {
                MessageBox.Show("Error!!! Some Event has to be choosen.");
                return;
            } 
            View.ShowRaceClass(new RaceClassPresenter(this, new RaceClassView(), raceClass),
                               View.raceClassView);
        }

        public void DisplayCurrentRaceClass()
        {
            if (_currentRaceClass != null)
                OpenRaceClass(_currentRaceClass);
        }

        public ApplicationPresenter ApplicationPresenter
        {
            get { return _applicationPresenter; }
            set { }
        }

        // Returns true if raceClass.Id exists in the RaceClass table
        private bool IsInRaceClass(RaceClass raceClass)
        {
            var hc = _applicationPresenter.HardcardContext;
            return (hc.RaceClasses.Count(c => c.Id == raceClass.Id) == 0) ? false : true;
        }

        // Returns true if EventClass table has FK for input raceClass
        private bool IsRaceClassInEventClass(RaceClass raceClass)
        {
            var hc = _applicationPresenter.HardcardContext;
            long count = (from a in hc.EventClasses
                          where a.ClassId == raceClass.Id
                          select a).Count();
            return (count == 0) ? false : true;
        }
        
        public void RaceClassSelected()
        {
        }

    }
}
