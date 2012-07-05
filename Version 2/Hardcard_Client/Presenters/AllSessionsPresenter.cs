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
using System.Data.Linq.Mapping;
using System.Data.Linq; 
using System.Linq.Expressions;
using System.Data.Linq.SqlClient; 


// EU new module
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
        private ObservableCollection<Entry> _entriesForSession; //Keeps Entries for CurrentSessionForEvent.
        private Entry _currentEntryForSession;                  // Entry/Competitor selected in _entriesForSession list.
        private Entry _tmpEntry = new Entry();                  // Kepps data from Entry fields while adding Competitor to Session

        // Keeps Stendings  (and => Entry.Competitors nad => Entry.Session) for Current Session
        private ObservableCollection<Standing> _standingsForSession;

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

            if (_applicationPresenter.AllEventsPresenter == null) return;
            InitTmpEntry();

            SessionsForEvent = InitSessionsForEvent(_applicationPresenter.AllEventsPresenter.CurrentEvent);
            if (SessionsForEvent != null && _sessionsForEvent.Count() > 0)
            {
                CurrentSessionForEvent = _sessionsForEvent.First();
                CompetitorsForEventClass = InitCompetitorsForEventClass(_currentSessionForEvent.EventClass);
                EntriesForSession = InitEntriesForSession(_currentSessionForEvent);
            }
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
            var hc = _applicationPresenter.HardcardContext; 
            Session newSession = new Session();
    
            newSession.Id = 0;
            long max_id = 0;
            if ((from c in hc.Sessions select c).Count() > 0) // not empty table
            {
                max_id = (from e in hc.Sessions select e.Id).Max();
                newSession.Id = ++max_id;
            }
           
            newSession.StartTime = DateTime.Now;
            newSession.SchedStopTime = DateTime.Now;
            newSession.RollingStart = false;
            newSession.SchedLaps = 10;

            // populate RaceSchedStopTime and RaceStartTime in milliseconds
            newSession.RaceSchedStopTime = (long)ConvertToUnixTime((DateTime)newSession.SchedStopTime) * 1000;
            newSession.RaceStartTime = (long)ConvertToUnixTime((DateTime)newSession.StartTime) * 1000;
            
            //Assign sesssion to first available eventClass
            Event currEvent = _applicationPresenter.AllEventsPresenter.CurrentEvent;
            if (currEvent == null) return;
            if (currEvent.EventClasses.Count() <= 0 ) return;
            EventClass eventClass = currEvent.EventClasses.First();
            newSession.EventClassId = eventClass.Id;
            hc.Sessions.AddObject(newSession);
            hc.SaveChanges();

            SessionsForEvent =  InitSessionsForEvent(currEvent);// updatesessionsforevent()
            CurrentSessionForEvent = newSession;
            //MessageBox.Show(string.Format("CurrentSessionForEvent.EventClass.Id = {0}", CurrentSessionForEvent.EventClass.Id));//it works
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
            hc.SaveChanges();
            StatusText = string.Format("Session '{0}' was saved.", session.Id);
        }

        //
        // Returns false if some input data for the session are not match the DataBase constraints
        // TODO!
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
        public void DeleteSessionFromEvent(Session session)
        {
            if (session == null) return;
            var hc = _applicationPresenter.HardcardContext;
            string status = string.Format("Session '{0}' was deleted.", session.ToString());
            
            // Delete from Entry
            if (session.Entries.Count() > 0)
            {
                while (session.Entries.Count() > 0)
                    DeleteEntry(session.Entries.First());
            }
            // Delete from Passing
            if (session.Passings.Count() > 0)
            {
                while (session.Passings.Count() > 0)
                    hc.Passings.DeleteObject(session.Passings.First());
            }
            // Delete from Penality
            if (session.Penalities.Count() > 0)
            {
                while (session.Penalities.Count() > 0)
                    hc.Penalities.DeleteObject(session.Penalities.First());
            }
            hc.SaveChanges();
            hc.Sessions.DeleteObject(session);
            StatusText = status;
            SessionsForEvent = InitSessionsForEvent(_applicationPresenter.AllEventsPresenter.CurrentEvent);
            if (SessionsForEvent != null && _sessionsForEvent.Count() > 0)
            {
                CurrentSessionForEvent = _sessionsForEvent.First();
                CompetitorsForEventClass = InitCompetitorsForEventClass(_currentSessionForEvent.EventClass);
                EntriesForSession = InitEntriesForSession(_currentSessionForEvent);
            }
        }


        //
        // Set data depending on Current session
        //
        public void SetSessionDependents(Session session)
        {
             if (session == null) return;
             if (_applicationPresenter.AllEventsPresenter == null) return;
             if (_currentSessionForEvent == null) return;
             var hc = _applicationPresenter.HardcardContext;
             CompetitorsForEventClass = InitCompetitorsForEventClass(_currentSessionForEvent.EventClass);
             hc.SaveChanges();
             EntriesForSession = InitEntriesForSession(_currentSessionForEvent);
             hc.SaveChanges();
             hc.SaveChanges();
        }

        //
        // Creates record in Entry table.
        // Creates record for Competitor in Entry table. CurrentSessionForEvent and Entry-data from screen are used.
        // Input: competitor - Competitor to be used to create Entry
        // Input: entry - data from the screen to create Entry for Competitor
        // Use it to add competitor to session
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
            EntriesForSession = InitEntriesForSession(CurrentSessionForEvent);
            CurrentEntryForSession = newEntry;
        }

        //
        // This method deletes records from Entry table, not from Competitor table !!!
        // This method uses input parameter even delete Current competitor because  there is no Current Entry in the class
        //
        public void ExcludeCurrentCompetitorFromSession(Entry entry)
        {
            DeleteEntry(entry);
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
            
            // The CompetitionNo can not be used twice in the same session
            if (EntriesForSession.Count(efs => efs.CompetitionNo == entry.CompetitionNo) != 0)
            {
                String str = String.Format("CompetitionNo {0} is already in this session", entry.CompetitionNo);
                MessageBox.Show(str);
                return false;
            }

            // The RFID can not be used twice in the same session
            if (EntriesForSession.Count(efs => efs.RFID == entry.RFID) != 0)
            {
                String str = String.Format("RFID {0} is already in this session", entry.RFID);
                MessageBox.Show(str);
                return false;
            }

            // Valid values for Status are 'a'(active)  or 'n'(nonactive)
            if (entry.Status == null || entry.Status.Count() != 1)
            {
                String str = String.Format("Status should be one character, e.g. 'a' (active)");
                MessageBox.Show(str);
                return false;
            }

            if (entry.CompetitionNo == null || entry.CompetitionNo.Count() > 10)
            {
                String str = String.Format("Number of chars in CompetitionNo should be <= 10 , e.g. '102a'");
                MessageBox.Show(str);
                return false;
            }

            if (entry.Equipment == null || entry.Equipment.Count() > 50)
            {
                String str = String.Format("Number of chars in Equipment should be <= 50 , e.g. 'Car'");
                MessageBox.Show(str);
                return false;
            }

            if (entry.Sponsors == null || entry.Sponsors.Count() > 300)
            {
                String str = String.Format("Number of chars in Sponsors should be less < 300 , e.g. 'IBM'");
                MessageBox.Show(str);
                return false;
            }
            return true;
        }
        
        //
        // 
        //
        public void DeleteEntry(Entry entry)
        {
            if (entry == null) return;
            var hc = _applicationPresenter.HardcardContext;
            hc.SaveChanges();
            // Delete from Standings
            if (entry.Standings.Count() > 0)
            {
                while (entry.Standings.Count() > 0)
                    hc.Standings.DeleteObject(entry.Standings.First());
            }
            hc.Entries.DeleteObject(entry);
            hc.SaveChanges();
        }


        //
        // 
        //
        public void DeleteStanding(Standing standing)
        {
            if (standing == null) return;
            var hc = _applicationPresenter.HardcardContext;
            hc.SaveChanges();
            hc.Standings.DeleteObject(standing);
            hc.SaveChanges();
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
            View.ShowSession(new SessionPresenter(this, new SessionView(), session),
                            View.sessionView);
        }

        // 
        // Create Sessions collection for Event and set default CurrentSession
        //
        public ObservableCollection<Session> InitSessionsForEvent(Event myEvent)
        {
            ObservableCollection<Session> Tmp;
            if (myEvent == null)
                Tmp = new ObservableCollection<Session>();
            else
                Tmp = new ObservableCollection<Session>(myEvent.EventClasses.SelectMany(eventClass => eventClass.Sessions));

            if (Tmp.Count() > 0)
                CurrentSessionForEvent = Tmp.First();
            else
                CurrentSessionForEvent = null;

            return Tmp;
        }  
        
        // 
        // Update Sessions collection for Event 
        //
        public ObservableCollection<Session> UpdateSessionsForEvent(Event myEvent)
        {
            ObservableCollection<Session> Tmp;
            if (myEvent == null)
                Tmp = new ObservableCollection<Session>();
            else
                Tmp = new ObservableCollection<Session>(myEvent.EventClasses.SelectMany(eventClass => eventClass.Sessions));
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
        // Create Standings collection for Session.
        // 
        // This method uses Passing.RaceTime to select records from Passing table. Then it 
        // populates sessionId and create records for Standing table.
        // 
        // This method is called during the session. When session was completed and all records in Passing 
        // table have sessionId populated, then call GetStandingsForSession(Session session) it will 
        // save time for selects.
        //
        public ObservableCollection<Standing> InitStandingsForSession(Session session)
        {
            ObservableCollection<Standing> Tmp;
            if (session == null)
            {
                Tmp = new ObservableCollection<Standing>();
                return Tmp;
            }
            else
            {
                SetSessionIdForPassingTable(session);
                Tmp = GetStandingsForSession(session);
            }
            return Tmp;
        }

        // 
        // Populate SessionId in Passing table.
        //
        // Fetch from Passing table all records with 
        // Session.RaceStartTime <= Passing.RaceTime <= Session.RaceSchedStopTime
        // and set SessionId for each record.
        // 
        public void SetSessionIdForPassingTable(Session session)
        {
            var hc = _applicationPresenter.HardcardContext;
            long sessionId = session.Id;

            // 
            List<Passing> query = (from p in hc.Passings
                                   from e in hc.Entries
                                   where e.SessionId == sessionId             // use RaceTime instead of PassingTime. 
                                       && e.RFID == p.RFID
                                       && session.RaceStartTime <= p.RaceTime
                                       && p.RaceTime <= session.RaceSchedStopTime
                                   select p).ToList();
            if (query.Count > 0)
            {
                foreach (Passing p in query) p.SessionId = sessionId;
                hc.SaveChanges();
            }
        }

        // 
        // Create Standings collection for Session.
        // 
        // This method uses SessionId to select records from Passing table 
        // and create records for Standing table.
        //
        public ObservableCollection<Standing> GetStandingsForSession(Session session)
        {
            ObservableCollection<Standing> Tmp;
            var hc = _applicationPresenter.HardcardContext;
            if (session == null)
            {
                Tmp = new ObservableCollection<Standing>();
            }
            else
            {
                long sessionId = session.Id;
                var passings =
                     (from p in hc.Passings
                      from e in hc.Entries
                      where e.SessionId == sessionId
                      && p.SessionId == sessionId
                      && e.RFID == p.RFID
                      orderby p.RaceTime
                      group p by p.RFID into p_group
                      orderby p_group.Key
                      select p_group);

                // Calculate field values for Standing table.
                int i = 0;
                List<Standing> lstStandings = new List<Standing>();
                foreach (var p_group in passings)
                {
                    ++i;
                    Standing st = new Standing();
                    st.Id = i;
                    st.EntryId = hc.Entries.First(a => a.RFID == p_group.Key).Id;


                    // Compute Best, Average, and Worst Loop Time
                    long bestLapTime = 0;
                    long avgLapTime = 0;
                    long worstLapTime = 0;
                    long delta = 0;
                    if (p_group.Count() <= 1)
                    {
                        st.LapsCompleted = 0;
                    }
                    else
                    {
                        st.LapsCompleted = (short)(p_group.Count() - 1);
                        avgLapTime = (long)((p_group.Last().RaceTime - p_group.First().RaceTime) / st.LapsCompleted);
                        bestLapTime = avgLapTime;
                        worstLapTime = avgLapTime;
                        List<Passing> tmpList = p_group.ToList();
                        for (int j = 1; j < tmpList.Count(); ++j)
                        {
                            delta = tmpList[j].RaceTime - tmpList[j - 1].RaceTime;
                            if (bestLapTime > delta) bestLapTime = delta;
                            if (worstLapTime < delta) worstLapTime = delta;
                        }

                    }

                    st.CompletedTime = p_group.Last().RaceTime - p_group.First().RaceTime;
                    st.BestLapTime = bestLapTime;
                    st.AvgLapTime = avgLapTime;
                    st.WorstLapTime = worstLapTime;

                    lstStandings.Add(st);
                }

                List<Standing> srtLstStandings = new List<Standing>();
                srtLstStandings = lstStandings.OrderBy(c => c.AvgLapTime).ToList();
                int pos = 0;
                foreach (var st in srtLstStandings)
                {
                    st.Position = (short)++pos;
                }

                Tmp = new ObservableCollection<Standing>(srtLstStandings);
            }

            //Delete/Populate data in Standing table for session
            List<Standing> query = (from c in hc.Standings
                                    select c).ToList();
            if ( query.Count > 0 )
            {
                //Remove data from Standing table
                foreach (Standing e in query) hc.Standings.DeleteObject(e);
                hc.SaveChanges();
            }

            if ( Tmp.Count > 0 )
            {
                //Populate Standing table
                foreach (Standing st in Tmp)
                    hc.Standings.AddObject(st);
                hc.SaveChanges();
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
            }
                InitEntriesForSession(session);
                StandingsForSession = InitStandingsForSession(session);
                return;
            }

        //Converts Seconds to DateTime
        internal static DateTime ConvertFromUnixTime(double unixTime)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(unixTime);
        }
        
        //Converts DateTime to Seconds
        internal static double ConvertToUnixTime(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        public void SessionSelected()
        {
        }

        // Copy Value from new event to event in Db
        public void UpdateSession(Session newSession, Session dbSession)
        {
            if (newSession == null || dbSession == null) return;
            dbSession.EventClassId = newSession.EventClassId;
            dbSession.StartTime = newSession.StartTime;
            dbSession.SchedStopTime = newSession.SchedStopTime;
            dbSession.RaceStartTime = newSession.RaceStartTime;
            dbSession.RaceSchedStopTime = newSession.RaceSchedStopTime;
            dbSession.SchedLaps = newSession.SchedLaps;
            dbSession.RollingStart = newSession.RollingStart;
        }
    
    }
}
