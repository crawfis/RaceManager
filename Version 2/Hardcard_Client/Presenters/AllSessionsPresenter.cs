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

// EU 2012/02/19 new module
namespace RacingEventsTrackSystem.Presenters
{
    public partial class AllSessionsPresenter : PresenterBase<Shell>
    {
        private readonly ApplicationPresenter _applicationPresenter;

        // One particular EventClass can be split up into many sessions.

        //Keeps Competitors for EventClass.
        private ObservableCollection<Competitor> _competitorsForEventClass; 
        private Competitor _currentCompetitorForEventClass;

        // Keeps Entries  (and => Competitors) for Current Session
        private ObservableCollection<Entry> _entriesForSession;
        private Entry _currentEntryForSession;  // Entry/Competitor selected in _entriesForSession list.
        private Entry _tmpEntry = new Entry();  // Kepps data from Entry fields while adding Competitor to Session

        // Keeps Stendings  (and => Entry.Competitors nad => Entry.Session) for Current Session
        private ObservableCollection<Standing> _standingsForSession;
        private Standing _tmpStanding = new Standing();  // Kepps data from Entry fields while adding Competitor to Session

        //private ObservableCollection<Entry> _entriesForEventClass; //Keeps Entries for EventClass.
        //private ObservableCollection<Entry> _entriesForSession;    //Keeps Entries for CurrentSessionForEvent.
        //private Entry _currentEntry; //

        //Keeps all Sessions for CurrentEvent
        private ObservableCollection<Session> _sessionsForEvent; 
        private Session _currentSessionForEvent;
        private string _statusText = "";


        public AllSessionsPresenter(ApplicationPresenter applicationPresenter, 
                                   Shell view
                                   ) : base(view)
        {
            try 
            { 
                _applicationPresenter = applicationPresenter;

                // to populate SessionView with some data while openinig
                InitSessionView();
                StatusText = ("AllEventsPresenter constructor no error");
            } 
            catch (Exception ex)
            {
                StatusText = "AllSessionsPresenter constructor failed with error: " + ex.Message;
                MessageBox.Show(StatusText); // stop executable
            }
        }

        public void InitSessionView()
        {
            _sessionsForEvent = new ObservableCollection<Session>();
            _currentSessionForEvent = null;

            _entriesForSession = new ObservableCollection<Entry>();
            _currentEntryForSession = null;

            _competitorsForEventClass = new ObservableCollection<Competitor>();
            _currentCompetitorForEventClass = null;

            _standingsForSession = new ObservableCollection<Standing>();

            // if it called before AllEventsPresenter constructor
            if (_applicationPresenter.AllEventsPresenter == null) return;

            SessionsForEvent = InitSessionsForEvent(_applicationPresenter.AllEventsPresenter.CurrentEvent);
            if (_sessionsForEvent.Count() > 0)
            {
                CurrentSessionForEvent = _sessionsForEvent.First();

                CompetitorsForEventClass = InitCompetitorsForEventClass(_currentSessionForEvent.EventClass);
                if (_competitorsForEventClass.Count() > 0)
                    CurrentCompetitorForEventClass = _competitorsForEventClass.First();

                EntriesForSession = InitEntriesForSession(_currentSessionForEvent);
                if (_entriesForSession.Count() > 0)
                    CurrentEntryForSession = _entriesForSession.First();

                StandingsForSession = InitStandingsForSession(_currentSessionForEvent);

            }
            InitTmpEntry();
            InitTmpStanding();
        }

        public ApplicationPresenter ApplicationPresenter
        {
            get { return _applicationPresenter; }
            set {}
        }

        public Session CurrentSessionForEvent
        {
            get { return _currentSessionForEvent; }
            set
            {
                _currentSessionForEvent = value;
                SetSessionDependents(_currentSessionForEvent);
                OnPropertyChanged("CurrentSessionForEvent");
            }
        }

        public ObservableCollection<Session> SessionsForEvent
        {
            get {  return _sessionsForEvent; }
            set
            {
                _sessionsForEvent = value;
                if (_sessionsForEvent.Count() > 0)
                    CurrentSessionForEvent = _sessionsForEvent.First();//it will update Competitors and Entries
                else 
                    CurrentSessionForEvent = null;

                EntriesForSession = InitEntriesForSession(_currentSessionForEvent);
                StandingsForSession = InitStandingsForSession(_currentSessionForEvent);
                OnPropertyChanged("SessionsForEvent");
            }
        }

        public ObservableCollection<Competitor> CompetitorsForEventClass
        {
            get { return _competitorsForEventClass; }
            set
            {
                _competitorsForEventClass = value;
                OnPropertyChanged("CompetitorsForEventClass");
            }
        }

        public Competitor CurrentCompetitorForEventClass
        {
            get { return _currentCompetitorForEventClass; }
            set
            {
                _currentCompetitorForEventClass = value;
                OnPropertyChanged("CurrentCompetitorForEventClass");
            }
        }

        public Entry CurrentEntryForSession
        {
            get { return _currentEntryForSession; }
            set
            {
                _currentEntryForSession = value;
                OnPropertyChanged("CurrentEntryForSession");
            }
        }


        public ObservableCollection<Entry> EntriesForSession
        {
            get { return _entriesForSession; }
            set
            {
                _entriesForSession = value;
                OnPropertyChanged("EntriesForSession");
            }
        }
        
        public Entry TmpEntry
        {
            get { return _tmpEntry; }
            set
            {
                _tmpEntry = value;
                OnPropertyChanged("TmpEntry");
            }
        }

        public ObservableCollection<Standing> StandingsForSession
        {
            get { return _standingsForSession; }
            set
            {
                _standingsForSession = value;
                OnPropertyChanged("StandingsForSession");
            }
        }

        public Standing TmpStanding
        {
            get { return _tmpStanding; }
            set
            {
                _tmpStanding = value;
                OnPropertyChanged("TmpStanding");
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
                SessionsForEvent = new ObservableCollection<Session>(FindByLookup(criteria));
                StatusText = string.Format("{0} events found.", SessionsForEvent.Count);
            }
            else
            {
                SessionsForEvent = new ObservableCollection<Session>(_applicationPresenter.HardcardContext.Sessions);
                StatusText = "Displaying all events.";
            }
        }

        //
        // Search for session by SessionId
        //
        public List<Session> FindByLookup(string sessionId)
        {
            IEnumerable<Session> found =
                from c in _applicationPresenter.HardcardContext.Sessions
                where (c.Id == Int32.Parse(sessionId))
                select c;
            return found.ToList();
        }

        //
        // Create new Session. It doesn't add the Session to collection or DataContext. 
        // 
        public void CreateNewSession()
        {
            Session newSession = new Session();
            newSession.Id = 0;
            newSession.EventClassId = 0;
            newSession.StartTime = DateTime.Now;
            newSession.SchedStopTime = DateTime.Now;
            newSession.RollingStart = false;
            newSession.SchedLaps = -1;
            CurrentSessionForEvent = newSession;
            //SaveSession(newSession);
            //OpenSession(newSession);
        }


        //
        // Create new Entry. 
        // 
        public void InitTmpEntry()
        {
            _tmpEntry.Id = 0;
            _tmpEntry.CompetitionNo = "3a";
            _tmpEntry.RFID = 7;
            _tmpEntry.Equipment = "Red Car";
            _tmpEntry.Sponsors = "No Sponsors";
            _tmpEntry.Status = "A";   //Active/Nonactive
            _tmpEntry.EntryDate = DateTime.Now;
        }

        //
        // Create new Standing. 
        // 
        public void InitTmpStanding()
        {
            _tmpStanding.Id = 1;
            _tmpStanding.EntryId = 2;
            _tmpStanding.Position = 3;
            _tmpStanding.LapsCompleted = 4;
            _tmpStanding.CompletedTime = 5;
            _tmpStanding.BestLapTime = 6;
            _tmpStanding.AvgLapTime = 7;
            _tmpStanding.WorstLapTime = 8;
            _tmpStanding.PassingTime = 9;
        }


        //
        // Update existing Session or add new entry if Session is not in DataContext.Session
        //
        // If this Session is in Session table then it will be updated in DataContext, otherwise it will be added to 
        // the DataContext.
        // Same for Session Collection.
        // The method submits DataContext to the DataBase.
        //

        public void SaveSession(Session session)
        {
            if (session == null) return;
            if (ValidateSession(session) == false) return; // not valid input parameters

            var hc = _applicationPresenter.HardcardContext;

            // If session is in DataContext.Sessions then just update it
            if (IsInSession(session))
            {
                //update DataContext
                Session dbSession = hc.Sessions.Single(e => e.Id == session.Id);
                hc.ApplyCurrentValues(dbSession.EntityKey.EntitySetName, session);
                StatusText = string.Format("Session '{0}' was updated.", session.ToString());
            }
            else
            {
                //Create new Session in DataContext.Sessions
                long max_id = 0;
                if ((from c in hc.Sessions select c).Count() > 0) // not empty table
                {
                    //long max_id = (from e in hc.Sessions select e.Id).Max();
                    max_id = hc.Sessions.Max(s => s.Id);
                }
                session.Id = ++max_id;
                hc.Sessions.AddObject(session);
            }

            int i = SessionsForEvent.IndexOf(session);
            if (i >= 0)
            {
                //Update in Collection
                SessionsForEvent.RemoveAt(i);
                SessionsForEvent.Insert(i, session);
                CurrentSessionForEvent = session; // Current Session was just deleted before
            }
            else
            {
                ////Create new myEvent in AllEvents Collection
                SessionsForEvent.Add(session);
            }

            hc.SaveChanges();
            StatusText = string.Format("Event '{0}' was saved.", session.Id);

            //catch (Exception ex)
            //{
            //  if (value < 0) throw new ArgumentException("Can not be < 0"); 
            //       MessageBox.Show(ex.ToString());
            //}

        }

        //
        // Returns false if some input data for the session are not match the DataBase constraints
        // (!!!Replace with interective WPF validation during editing)
        public bool ValidateSession(Session session)
        {
            if (ApplicationPresenter.AllEventsPresenter.CurrentEvent.EventClasses.Count(ec => ec.Id ==  session.EventClassId) == 0)
            {
                MessageBox.Show("EventClassId field is not in the list of EventClasses");
                return false;
            }
            return true;
        }

        // 
        // Delete session from DataContext.Sessions and SessionsForEvent Collection. Resets CurrentSessionForEvent.
        //
        public void ExcludeSessionFromEvent(Session session)
        {
            if (session == null) return;
            var hc = _applicationPresenter.HardcardContext;
            string status = string.Format("Session '{0}' was deleted.", session.ToString());
            
            // Delete from DataContext
            // Check if session is in DataContext.Sessions
            if (IsInSession(session))
            {
                if (IsSessionInEntry(session))
                {   // Remove records from Entry first for the Competitor
                    List<Entry> query = (from c in session.Entries
                                         select c).ToList();
                    foreach (Entry e in query) hc.Entries.DeleteObject(e);
                }
                if (IsSessionInPassing(session))
                {   // Remove records from Entry first for the Competitor
                    List<Passing> query = (from p in session.Passings
                                         select p).ToList();
                    foreach (Passing p in query) hc.Passings.DeleteObject(p);
                }
                // check if (IsSessionInPenalties(session){. . .})

                hc.Sessions.DeleteObject(session);
                hc.SaveChanges();
                StatusText = status;
            }
                        
            // Delete from Collection            
            if (SessionsForEvent.Contains(session))
            {
                SessionsForEvent.Remove(session); //???
                StatusText = status;
            }

            // ReSet Current session or open empty screen if list of sessions is empty
            if (SessionsForEvent.Count() > 0)
            {
                CurrentSessionForEvent = _sessionsForEvent.First();
                //EntriesForSession = InitEntriesForSession(_currentSessionForEvent);
                CompetitorsForEventClass = InitCompetitorsForEventClass(_currentSessionForEvent.EventClass);
            }
            else 
            {
                OpenSession(new Session());
            }

        }


        //
        // Set data depending on Current session
        //
        public void SetSessionDependents(Session session)
        {
            if (session == null) return;
            if (_applicationPresenter.AllEventsPresenter == null) return;

            CompetitorsForEventClass = InitCompetitorsForEventClass(_currentSessionForEvent.EventClass);
             if (_competitorsForEventClass.Count() > 0)
                CurrentCompetitorForEventClass = _competitorsForEventClass.First();
            else
                CurrentCompetitorForEventClass = null;

            EntriesForSession = InitEntriesForSession(_currentSessionForEvent);
            if (_entriesForSession.Count() > 0)
                CurrentEntryForSession = _entriesForSession.First();
            else
                CurrentEntryForSession = null;

            StandingsForSession = InitStandingsForSession(_currentSessionForEvent);
        }

        //
        // Creates record for Competitor in Entry table. CurrentSessionForEvent and Entry-data from screen are used.
        // Input: competitor - Competitor to be used to create Entry
        // Input: entry - data from the screen to create Entry for Competitor
        // Use it to add competitor to session
        // Remove ???
        //
        public void AddCompetitorToSession(Competitor competitor, Entry entry)
        {
            if (competitor == null || entry == null) return;
            
            // To save Competitor as an Entry for this Session, screen fields for Entry should be populated properly. 
            if (ValidateEntry(competitor, entry) == false) return;
            var hc = _applicationPresenter.HardcardContext;
         
            Entry newEntry = new Entry();
            long max_id = 0;
            if ((from c in hc.Entries select c).Count() > 0) // not empty table
            {
                max_id = (from c in hc.Entries select c.Id).Max();
            }
            newEntry.Id = ++max_id;
            newEntry.CompetitionNo = entry.CompetitionNo;
            newEntry.RFID = entry.RFID;
            newEntry.Status = entry.Status;
            newEntry.Sponsors = entry.Sponsors;
            newEntry.EntryDate = entry.EntryDate;
            newEntry.Equipment = entry.Equipment;
            newEntry.CompetitorId = competitor.Id;
            newEntry.SessionId = CurrentSessionForEvent.Id;

            hc.Entries.AddObject(newEntry);
            hc.SaveChanges();

            if (!_entriesForSession.Contains(newEntry))
            {
                _entriesForSession.Add(newEntry);
            }
            EntriesForSession = InitEntriesForSession(CurrentSessionForEvent);
            CurrentEntryForSession = newEntry;

        }

        public void ExcludeCompetitorFromSession(Entry entry)
        {
            if (entry == null) return;
            var hc = _applicationPresenter.HardcardContext;
            string status = string.Format("entry '{0}' was deleted.", entry.ToString());
            long entryId = entry.Id;
            
            if (EntriesForSession.Contains(entry))
            {
                EntriesForSession.Remove(entry);
                StatusText = status;
            }
            
            if (IsEntryInEntry(entryId))
            {
                Entry dbEntry = hc.Entries.Single(e => e.Id == entry.Id);
                if (dbEntry != null) hc.Entries.DeleteObject(dbEntry);
                hc.SaveChanges();
                StatusText = status;
            }
            EntriesForSession = InitEntriesForSession(CurrentSessionForEvent);
        }

        //
        // Returns false if some input data for the entry are not match the DataBase constraints
        // (!!!Replace it with interective WPF validation during editing)
        public bool ValidateEntry(Competitor competitor, Entry entry)
        {
            // The same Athlete can not have two entries for the same session
            if (EntriesForSession.Count(efs => efs.Competitor.AthleteId == competitor.AthleteId) != 0)
            {
                String str = String.Format("This Athlete: {0} {1} Competitorid = {2} is already in this session",
                                competitor.Athlete.FirstName, competitor.Athlete.LastName, competitor.Id);
                MessageBox.Show(str);
                return false;
            }

            // The CompetitionNo can not be used twice
            if (EntriesForSession.Count(efs => efs.CompetitionNo == entry.CompetitionNo) != 0)
            {
                String str = String.Format("CompetitionNo {0} is already in this session", entry.CompetitionNo);
                MessageBox.Show(str);
                return false;
            }

            // The RFID can not be used twice
            if (EntriesForSession.Count(efs => efs.RFID == entry.RFID) != 0)
            {
                String str = String.Format("RFID {0} is already in this session", entry.RFID);
                MessageBox.Show(str);
                return false;
            }

            // Valid values for Status are 'a'(active)  or 'n'(nonactive)
            if (entry.Status.Count() != 1)
            {
                String str = String.Format("Status should be one character, e.g. 'a' (active)");
                MessageBox.Show(str);
                return false;
            }
            return true;
        }
        
        //
        // Returns true if sesion.SessionID exists in the Session table
        //
        private bool IsInSession(Session session)
        {
            var hc = _applicationPresenter.HardcardContext;
            return (hc.Sessions.Count(s => s.Id == session.Id) == 0) ? false : true;
        }

        // Returns true if session has referenses in Entry table (PK Session.Id = FKs Entry.SessionIds)
        // Use this method when remove session
        private bool IsSessionInEntry(Session session)
        {
            var hc = _applicationPresenter.HardcardContext;
            return (hc.Entries.Count(e => e.SessionId == session.Id) == 0) ? false : true;
        }

        // Returns true if session has referenses in Passing table (PK Session.Id = FKs Passing.SessionIds)
        // Use this method when remove session
        private bool IsSessionInPassing(Session session)
        {
            var hc = _applicationPresenter.HardcardContext;
            return (hc.Passings.Count(p => p.SessionId == session.Id) == 0) ? false : true;
        }

        // Returns true if entry found in Entry table
        private bool IsEntryInEntry(long entryId)
        {
            var hc = _applicationPresenter.HardcardContext;
            return (hc.Entries.Count(e => e.Id == entryId) == 0) ? false : true;
        }

        public void OpenSession(Session session)
        {
            if (session == null) return;

            // Set CurrentSessionForEvent (see OpenEvent())
           /* 
            if (_applicationPresenter.AllSessionsPresenter.CurrentSessionForEvent == null)
            {
                MessageBox.Show("Error!!! Some Session has to be choosen.");
                return;
            } 
            */
            View.ShowSession(new SessionPresenter(this, new SessionView(), session),
                            View.sessionView);
        }

        // 
        // Create Session collection for Event and set default CurrentSession
        //
        public ObservableCollection<Session> InitSessionsForEvent(Event myEvent)
        {

            ObservableCollection<Session> Tmp;
            if (myEvent == null)
                Tmp = new ObservableCollection<Session>();
            else
            {
                var hc = _applicationPresenter.HardcardContext;
                long eventId = myEvent.Id;
                IQueryable<Session> sessions =
                     from s in hc.Sessions
                     from ec in hc.EventClasses
                     where ec.EventId == eventId
                     && s.EventClassId == ec.Id
                     select s;
                Tmp = new ObservableCollection<Session>(sessions.ToList());
            }
            
            if (Tmp.Count() > 0)
                CurrentSessionForEvent = Tmp.First();
            else
                CurrentSessionForEvent = null;

            return Tmp;
        }

        // 
        // Create Entries collection for Session
        // and set default CurrentEntryForSession
        //
        public ObservableCollection<Entry> InitEntriesForSession(Session session)
        {
            ObservableCollection<Entry> Tmp;
            if (session == null)
                Tmp = new ObservableCollection<Entry>();
            else
                Tmp = new ObservableCollection<Entry>(session.Entries.ToList());
            if (Tmp.Count() > 0)
                CurrentEntryForSession = Tmp.First();
            else
                CurrentEntryForSession = null;

            return Tmp;
        }

        // 
        // Create Standings collection for Session sorted by LapsCompleted, BestLapTime
        //
        public ObservableCollection<Standing> InitStandingsForSession(Session session)
        {
            ObservableCollection<Standing> Tmp;
            if (session == null)
                Tmp = new ObservableCollection<Standing>();
            else
            {
                var hc = _applicationPresenter.HardcardContext;
                long sessionId = session.Id;
                IQueryable<Standing> standings =
                     from e in hc.Entries
                     from s in hc.Standings
                     where e.SessionId == sessionId
                     && s.EntryId == e.Id
                     orderby s.LapsCompleted, s.BestLapTime
                     select s;
                Tmp = new ObservableCollection<Standing>(standings.ToList());
            }
            return Tmp;
        }

        // 
        // Create Competitors collection for EventClass related to EventClass
        // and set default CurrentCompetitorForEventClass
        //
        public ObservableCollection<Competitor> InitCompetitorsForEventClass(EventClass eventClass)
        {
            ObservableCollection<Competitor> Tmp;
            if (eventClass == null) 
                Tmp = new ObservableCollection<Competitor>();
            else
                Tmp = new ObservableCollection<Competitor>(eventClass.Competitors.ToList());
            if (Tmp.Count() > 0)
                CurrentCompetitorForEventClass = Tmp.First();
            else
                CurrentCompetitorForEventClass = null;

            return Tmp;
        }

        //
        // Update list of Standings for Session
        //
        public void UpdateStandingTable(Session session)
        {
            if (session == null) return;
            var hc = _applicationPresenter.HardcardContext;
            if ((hc.Standings.Count() > 0))
            {
                // Remove records from Standings 
                List<Standing> query = (from c in hc.Standings
                                     select c).ToList();
                foreach (Standing e in query) hc.Standings.DeleteObject(e);
                hc.SaveChanges();
                InitEntriesForSession(session);
                InitStandingsForSession(session);
                return;
            }

            
            int NumOfLaps = (int)session.SchedLaps;
            int standingId = 0;
            Random random = new Random();

            for ( short lap = 0; lap < NumOfLaps; ++lap)
            {
                
                foreach (Entry e in EntriesForSession)
                {
                    Standing st = new Standing();
                    st.Id = standingId++;
                    st.EntryId = e.Id;
                    st.Position = 3;
                    st.LapsCompleted = (short)(lap + 1);
                    st.CompletedTime = st.LapsCompleted * 100 ;
                    st.BestLapTime = random.Next(90,110);
                    st.AvgLapTime = st.BestLapTime + random.Next(5,10);
                    st.WorstLapTime = st.AvgLapTime + random.Next(10,20);
                    st.PassingTime = 9;

                    hc.Standings.AddObject(st);
                }
            }
            hc.SaveChanges(); 
            InitEntriesForSession(session);
            InitStandingsForSession(session);
        }

        public void SessionSelected()
        {
        }
    
    }
}
