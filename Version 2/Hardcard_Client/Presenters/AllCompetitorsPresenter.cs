using System;
using System.Collections.ObjectModel;
using RacingEventsTrackSystem.Model;
using RacingEventsTrackSystem.Views;
using RacingEventsTrackSystem.DataAccess;
using RacingEventsTrackSystem.Presenters;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq.Mapping;
using System.Data.Linq; 
using System.Linq.Expressions;
using System.Data.Linq.SqlClient;
using System.Data.Objects; 

namespace RacingEventsTrackSystem.Presenters
{
    public class CompetitorInfo
    {
        public EventClass EventClass;
        public string status; // = bool Deleted in Competitor class
        public string vehicleType = "Ford";
        public string vehicleModel = "Ford150";
        public int vehicleCC = 150;
    }

    public partial class AllCompetitorsPresenter : PresenterBase<Shell>
    {
        private readonly ApplicationPresenter _applicationPresenter;

        // Keeps Competitors for the CurrentEvent
        // any control changing CurrentEvent has to reset that collection.
        private ObservableCollection<Competitor> _allCompetitorsForEvent;
        // Competitor selected in _allCompetitorsForEvent list.
        private Competitor _currentCompetitorForEvent;

        //Keeps information from GUI about Competitor (see "Competitor Info" box)
        private CompetitorInfo _competitorInfo;
        private string _statusText;

        public AllCompetitorsPresenter( ApplicationPresenter applicationPresenter, 
                                        Shell view
                                      ) : base(view)
        {
            try
            {
                _applicationPresenter = applicationPresenter;
                _allCompetitorsForEvent = InitCompetitorsForEvent(_applicationPresenter.AllEventsPresenter.CurrentEvent);

                StatusText = ("AllCompetitorsPresenter constructor no error");

            }
            catch (Exception ex)
            {
                StatusText = "EventPresenter constructor failed with error: " + ex.Message;
                System.Console.WriteLine("EventPresenter constructor failed with error: " + ex.Message);
            }
        }

        public ObservableCollection<Competitor> AllCompetitors
        {
            get { return _allCompetitorsForEvent; }
            set
            {
                _allCompetitorsForEvent = value;
                OnPropertyChanged("AllCompetitors");
            }
        }

        public Competitor CurrentCompetitorForEvent
        {
            get { return _currentCompetitorForEvent; }
            set
            {
                _currentCompetitorForEvent = value;
                if (_currentCompetitorForEvent != null && _currentCompetitorForEvent.Athlete != null)
                      _applicationPresenter.AllAthletesPresenter.CurrentAthlete = _currentCompetitorForEvent.Athlete;
                OnPropertyChanged("CurrentCompetitorForEvent");
            }
        }

        public CompetitorInfo CompetitorInfo
        {
            get { return _competitorInfo; }
            set
            {
                _competitorInfo = value;
                OnPropertyChanged("CompetitorInfo");
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
            
        // 
        // Add Current Athlete to List of Competitors
        //
        public void AddAtleteToCompetitorsList(Athlete athlete,
                                              EventClass eventClass,
                                              string status,
                                              string vehicleType,
                                              string vehicleModel,
                                              int vehicleCC)
        {
            if (athlete == null || eventClass == null || eventClass.Id == 0) return; //Athlete not chosen
            var hc = _applicationPresenter.HardcardContext;
            // add convertor constructor to Competitor    
            Competitor competitor = CreateNewCompetitor(athlete, eventClass.Id);
            //competitor.EventClassId = eventClass.Id;
            competitor.VehicleType = vehicleType;
            competitor.VehicleModel = vehicleModel;
            competitor.VehicleCC = vehicleCC;
            if (status == "Active")
                competitor.Deleted = false;
            else
                competitor.Deleted = true;
 
            // Check if Athlete is in Competitor Table
            // if Athlete not found in Competitor Table then create new record in Competitor table
            if (!IsCompetitorInCompetitor(athlete, eventClass))
            {
                long max_id = 0;
                if ((from c in hc.Competitors select c).Count() > 0) // not empty table
                {
                    max_id = (from c in hc.Competitors select c.Id).Max();
                }
                competitor.Id = ++max_id;
                competitor.Athlete = athlete;
                athlete.Competitors.Add(competitor);
                //hc.Competitors.AddObject(competitor);
                hc.SaveChanges();
            }
            else
            {
                String str = String.Format("{0} {1} {2} {3} {4} {5} {6} {7}",
                     athlete.FirstName,
                     athlete.LastName,
                     athlete.AddressLine1,
                     athlete.City,
                     athlete.State,
                     athlete.Country,
                     eventClass.Id,
                     eventClass.RaceClass.ClassName);
                MessageBox.Show("Warning!!! This Athlete/EventClass is already in the Competitors List:\n" + str);
            }

            if (!_applicationPresenter.AllCompetitorsPresenter.AllCompetitors.Contains(competitor))
            {
                _applicationPresenter.AllCompetitorsPresenter.AllCompetitors.Add(competitor);
            }
        }
        //
        // Create new Competitor based on Athlete object
        //
        public Competitor CreateNewCompetitor(Athlete athlete, long EventClassId)
        {
            return new Competitor()
            {
                AthleteId = (long)athlete.Id,
                EventClassId = EventClassId, //todo will be constraint violation !!!!!!!!!!!! move to competitorsPresenter
                VehicleType = "n/a",
                VehicleModel = "n/a",
                VehicleCC = 0,
                Deleted = false
            };
        }

        //
        // Remove Competitor from Competitors
        //
        public void DeleteCompetitor(Competitor competitor)
        {
            if (competitor == null) return;
            var hc = _applicationPresenter.HardcardContext;
            string status = string.Format("Competitor '{0}' was deleted.", competitor.ToString());

            if (competitor.Entries.Count() > 0)
            {
                while (competitor.Entries.Count() > 0)
                    ApplicationPresenter.AllSessionsPresenter.DeleteEntry(competitor.Entries.First());
            }

            /* if (IsCompetitorInCompetitor(competitor))
             {
                 if (IsCompetitorInEntry(competitor))
                 {   // Remove record from Entry first for the Competitor
                     List<Entry> query = (from c in competitor.Entries 
                                                select c).ToList();
                     foreach (Entry e in query ) hc.Entries.DeleteObject(e);
                 }
                 Athlete athlete = competitor.Athlete;
                 //athlete.Competitors.Remove(competitor);//???diassociate from athlete
             }
             */
            hc.SaveChanges();
            hc.Competitors.DeleteObject(competitor);
            hc.SaveChanges();
             
            /*
                       if (AllCompetitors.Contains(competitor))
                       {
                           AllCompetitors.Remove(competitor);
                           StatusText = status;
                       }
             */ 
            //    OpenNewCompetitor();

        }


        //
        // Remove CurrentCompetitor from Competitors for Event
        //
        public void DeleteCurrentCompetitorForEvent()
        {
            DeleteCompetitor(CurrentCompetitorForEvent);

            AllEventsPresenter aep = ApplicationPresenter.AllEventsPresenter;
            if (aep != null && aep.CurrentEvent != null)
                AllCompetitors = InitCompetitorsForEvent(aep.CurrentEvent);

            //???
            AllSessionsPresenter asp = ApplicationPresenter.AllSessionsPresenter;
            if (asp != null && asp.CurrentSessionForEvent != null)
                asp.CompetitorsForEventClass = asp.InitCompetitorsForEventClass(asp.CurrentSessionForEvent.EventClass);

        }

        //
        // Remove CurrentCompetitor from Competitors for EventClass
        //
        public void DeleteCurrentCompetitorForEventClass()
        {
            DeleteCompetitor(CurrentCompetitorForEvent);

            AllSessionsPresenter asp = ApplicationPresenter.AllSessionsPresenter;
            if (asp != null && asp.CurrentSessionForEvent != null)
                asp.CompetitorsForEventClass = asp.InitCompetitorsForEventClass(asp.CurrentSessionForEvent.EventClass);
            //???
            AllEventsPresenter aep = ApplicationPresenter.AllEventsPresenter;
            if (aep != null && aep.CurrentEvent != null)
                AllCompetitors = InitCompetitorsForEvent(aep.CurrentEvent);

        }


        // Returns true if competitor with athlete.Id and eventClass.Id is in Competitor table
        // Use this method to add new competitor
        private bool IsCompetitorInCompetitor(Athlete athlete, EventClass eventClass)
        {
            var hc = _applicationPresenter.HardcardContext;
            return (hc.Competitors.Count(c => c.EventClassId == eventClass.Id
                                            && c.AthleteId == athlete.Id) == 0) ?  false : true;
        }
       
        // Returns true if competitor with that Id is in Competitor table
        // Use this method for competitor update
        private bool IsCompetitorInCompetitor(Competitor competitor)
        {
            var hc = _applicationPresenter.HardcardContext;
            return (hc.Competitors.Count(c => c.Id == competitor.Id) == 0) ? false : true;
        }


        // Returns true if competitor has referenses in Entry table (PK Competitor.Id = FKs Entry.CompetitorIds)
        // Use this method when remove competitor
        private bool IsCompetitorInEntry(Competitor competitor)
        {
            var hc = _applicationPresenter.HardcardContext;
            return (hc.Entries.Count(e => e.CompetitorId == competitor.Id) == 0) ? false : true;
        }

        // It might be DisplaycurrentCompetitorForEvent(){OpenCompetitor(Competitor competitor)}
        public void DisplayCurrentCompetitor()
        {
            OpenCompetitor(_currentCompetitorForEvent);
        }
       
        public void OpenNewCompetitor()
        {
            OpenCompetitor(new Competitor());
        }

        public void OpenCompetitor(Competitor competitor)
        {
            if (competitor == null) return;
            View.ShowCompetitor(new CompetitorPresenter(this, new CompetitorView(), competitor),
                                View.competitorView);
        }

        public ApplicationPresenter ApplicationPresenter
        {
            get { return _applicationPresenter; }
            set { }
        }
        // 
        // Create Competitors collection for Current Event
        //
        public ObservableCollection<Competitor> InitCompetitorsForEvent(Event myEvent)
        {
            ObservableCollection<Competitor> Tmp;
            if (myEvent == null)
                Tmp = new ObservableCollection<Competitor>();
            else
            {
                //???
                Tmp = new ObservableCollection<Competitor>(myEvent.EventClasses.SelectMany(eventClass => eventClass.Competitors).ToList());
            }

            if (Tmp.Count() > 0)
                CurrentCompetitorForEvent = Tmp.First();
            else
                CurrentCompetitorForEvent = null;

            return Tmp;
        }
    }
}