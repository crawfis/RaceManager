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
using System.IO;
using RacingEventsTrackSystem.UserControls;
using System.Windows.Threading;
using System.Windows.Forms; 


// EU new module
namespace RacingEventsTrackSystem.Presenters
{
    public partial class AllSessionsPresenter : PresenterBase<Shell>
    {
        private readonly ApplicationPresenter _applicationPresenter;
        public  readonly string REPORTFOLDER = @"C:\temp";


        // One particular EventClass can be split up into many sessions.

        //Keeps Competitors for EventClass.
        private ObservableCollection<Competitor> _competitorsForEventClass; 
        private Competitor _currentCompetitorForEventClass;

        // Keeps Entries  (and => Competitors) for Current Session
        private ObservableCollection<Entry> _entriesForSession; //Keeps Entries for CurrentSessionForEvent.
        private Entry _currentEntryForSession;                  // Entry/Competitor selected in _entriesForSession list.
        private Entry _tmpEntry = new Entry();                  // Kepps data from Entry fields while adding Competitor to Session

        // Keeps Standings  (and => Entry.Competitors nad => Entry.Session) for Current Session
        private ObservableCollection<Standing> _standingsForSession;

        // Keeps Passing data  (and => Entry.Competitors nad => Entry.Session) for Current Session
        private ObservableCollection<Passing> _passingsForSession;
        private Passing _currentPassingForSession;   // Passing selected in _passingForSession list.

        //Keeps all Sessions for CurrentEvent
        private ObservableCollection<Session> _sessionsForEvent; 
        private Session _currentSessionForEvent;
        private string _statusText = "";
        private Shell _view;

        public AllSessionsPresenter(ApplicationPresenter applicationPresenter, 
                                   Shell view
                                   ) : base(view)
        {
            try 
            { 
                _applicationPresenter = applicationPresenter;
                _view = view;

                // to populate SessionView with some data while openinig
                InitSessionView();
                StatusText = ("AllEventsPresenter constructor no error");
            } 
            catch (Exception ex)
            {
                StatusText = "AllSessionsPresenter constructor failed with error: " + ex.Message;
                System.Windows.MessageBox.Show(StatusText); // stop executable
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
            _passingsForSession = new ObservableCollection<Passing>();
                  

            // if it called before AllEventsPresenter constructor
            if (_applicationPresenter.AllEventsPresenter == null) return;

            InitTmpEntry();

            /*//???SessionsForEvent = InitSessionsForEvent(_applicationPresenter.AllEventsPresenter.CurrentEvent);*/
            SessionsForEvent = InitSessionsForEvent(_applicationPresenter.AllEventsPresenter.CurrentEvent);
            if (SessionsForEvent != null && _sessionsForEvent.Count() > 0)
            {
                CurrentSessionForEvent = _sessionsForEvent.First();
                CompetitorsForEventClass = InitCompetitorsForEventClass(_currentSessionForEvent.EventClass);
                EntriesForSession = InitEntriesForSession(_currentSessionForEvent);
                PassingsForSession = InitPassingsForSession(_currentSessionForEvent);
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
                //_sessionsForEvent = UpdateSessionsForEvent(_applicationPresenter.AllEventsPresenter.CurrentEvent);
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

        public ObservableCollection<Passing> PassingsForSession
        {
            get { return _passingsForSession; }
            set
            {
                _passingsForSession = value;
                OnPropertyChanged("PassingsForSession");
            }
        }

         public Passing CurrentPassingForSession
        {
            get { return _currentPassingForSession; }
            set
            {
                _currentPassingForSession = value;
                OnPropertyChanged("CurrentPassingForSession");
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
                //max_id = hc.Sessions.Max(s => s.Id);
                newSession.Id = ++max_id;
            }

            newSession.StartTime = DateTime.Now;
            //newSession.StartTime = DateTime.Now;
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
            //eventClass.Sessions.Add(newSession);
            hc.Sessions.AddObject(newSession);
            hc.SaveChanges();
            //View.sessionView.cmbxEventClasses.
            //MessageBox.Show(string.Format("newSession.EventClass.Id = {0}", newSession.EventClass.Id));//it works

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
            _tmpEntry.CompetitionNo = "123abs";
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

           /*
            Session dbSession = null;
            bool newSession = IsInSession(session)? false : true ;
            // If newSession is in DataContext.Sessions then just update it
            if (newSession)
            {
                //Create new Session in DataContext.Sessions
               
            }
            else
            {
                //update DataContext
                dbSession = hc.Sessions.Single(e => e.Id == session.Id);
                UpdateSession(session, dbSession);
                //hc.ApplyCurrentValues(dbSession.EntityKey.EntitySetName, session);
                StatusText = string.Format("Session '{0}' was updated.", session.ToString());
            }

            if (newSession)
            {
                ////Create new myEvent in AllEvents Collection
                SessionsForEvent.Add(session);
            }
            else
            {
                //Update in Collection
                int i = SessionsForEvent.IndexOf(dbSession);
                SessionsForEvent.RemoveAt(i);
                SessionsForEvent.Insert(i, dbSession);
                CurrentSessionForEvent = dbSession; // Current Session was just deleted before
            }
            */

            //catch (Exception ex)
            //{
            //  if (value < 0) throw new ArgumentException("Can not be < 0"); 
            //       MessageBox.Show(ex.ToString());
            //}

        }

        //
        // Returns false if some input data for the session are not match the DataBase constraints
        // (!!!Replace with interective WPF validation during editing)
        // TODO!
        public bool ValidateSession(Session session)
        {
            if (ApplicationPresenter.AllEventsPresenter.CurrentEvent.EventClasses.Count(ec => ec.Id ==  session.EventClassId) == 0)
            {
                System.Windows.MessageBox.Show("EventClassId field is not in the list of EventClasses");
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
            //????
            SessionsForEvent = InitSessionsForEvent(_applicationPresenter.AllEventsPresenter.CurrentEvent);
            if (SessionsForEvent != null && _sessionsForEvent.Count() > 0)
            {
                CurrentSessionForEvent = _sessionsForEvent.First();
                CompetitorsForEventClass = InitCompetitorsForEventClass(_currentSessionForEvent.EventClass);
                EntriesForSession = InitEntriesForSession(_currentSessionForEvent);
                PassingsForSession = InitPassingsForSession(_currentSessionForEvent);
                //???StandingsForSession = InitStandingsForSession(_currentSessionForEvent);
            }

            /*//???
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
                //CompetitorsForEventClass = InitCompetitorsForEventClass(_currentSessionForEvent.EventClass);
            }
            else 
            {
                OpenSession(new Session());
            }
            */

        }


        //
        // Set data depending on Current session
        //
        public void SetSessionDependents(Session session)
        {
            //???
             if (session == null) return;
             if (_applicationPresenter.AllEventsPresenter == null) return;
             if (_currentSessionForEvent == null) return;
             var hc = _applicationPresenter.HardcardContext;
             CompetitorsForEventClass = InitCompetitorsForEventClass(_currentSessionForEvent.EventClass);
             hc.SaveChanges();
             EntriesForSession = InitEntriesForSession(_currentSessionForEvent);
             PassingsForSession = InitPassingsForSession(_currentSessionForEvent);
             hc.SaveChanges();
             //???
             // Data for Standing are calculated only if press button "update Standings"
             //StandingsForSession = InitStandingsForSession(_currentSessionForEvent);
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
            if (CurrentSessionForEvent == null) return;
            
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
                System.Windows.MessageBox.Show(str);
                return false;
            }

            // The CompetitionNo can not be used twice in the same session
            if (EntriesForSession.Count(efs => efs.CompetitionNo == entry.CompetitionNo) != 0)
            {
                String str = String.Format("CompetitionNo {0} is already in this session", entry.CompetitionNo);
                System.Windows.MessageBox.Show(str);
                return false;
            }

            // The RFID can not be used twice in the same session
            if (EntriesForSession.Count(efs => efs.RFID == entry.RFID) != 0)
            {
                String str = String.Format("RFID {0} is already in this session", entry.RFID);
                System.Windows.MessageBox.Show(str);
                return false;
            }

            // Valid values for Status are 'a'(active)  or 'n'(nonactive)
            if (entry.Status == null || entry.Status.Count() != 1)
            {
                String str = String.Format("Status should be one character, e.g. 'a' (active)");
                System.Windows.MessageBox.Show(str);
                return false;
            }

            if (entry.CompetitionNo == null || entry.CompetitionNo.Count() > 10)
            {
                String str = String.Format("Number of chars in CompetitionNo should be <= 10 , e.g. '102a'");
                System.Windows.MessageBox.Show(str);
                return false;
            }

            if (entry.Equipment == null || entry.Equipment.Count() > 50)
            {
                String str = String.Format("Number of chars in Equipment should be <= 50 , e.g. 'Car'");
                System.Windows.MessageBox.Show(str);
                return false;
            }

            if (entry.Sponsors == null || entry.Sponsors.Count() > 300)
            {
                String str = String.Format("Number of chars in Sponsors should be less < 300 , e.g. 'IBM'");
                System.Windows.MessageBox.Show(str);
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
            hc.SaveChanges();
            hc.Entries.DeleteObject(entry);
            hc.SaveChanges();

            // or 
            /*
            while (entry.Standings.Count() > 0)
                    ApplicationPresenter.AllSessionsPresenter.DeleteStanding(entry.Standings.First());
            */
            // or
            /*
            List<Standing> query = (from c in hc.Standings
                                    select c).ToList();
            if (query.Count > 0)
            {
                //Remove data from Standing table
                foreach (Standing e in query) hc.Standings.DeleteObject(e);
                hc.SaveChanges();
            }
            */
            // or
            /*
            // delete FK records from Standings first
            if (entry.Standings.Count > 0)
            {
                List<Standing> query = (from c in entry.Standings
                                     select c).ToList();
                //foreach (Standing s in query) entry.Standings.Remove(s);
                foreach (Standing s in query) hc.Standings.DeleteObject(s);
            } 
            */
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
        // Create Sessions collection for Event and set default CurrentSession
        //
        public ObservableCollection<Session> InitSessionsForEvent(Event myEvent)
        {
            //???
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
            //???
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
        }       // 


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
        // Create Passing collection for Session
        // and set default CurrentPassingForSession
        // It is used to filter competitors for Timing screen
        //
        public void FilterCompetitorsInTiming(Standing standing, bool isChecked )
        {
            Session session = CurrentSessionForEvent;
            var hc = _applicationPresenter.HardcardContext;
            if (session == null) return;
            if (standing == null) return;
            long rfid = standing.Entry.RFID;
            long sessionId = session.Id;
            if (isChecked == true)
            {
                List<Passing> new_passings_list = (from p in hc.Passings
                                                  where p.SessionId == sessionId
                                                     && p.RFID == rfid
                                                 select p).ToList();
                List<Passing> orig_passings_list = PassingsForSession.ToList();
                new_passings_list.AddRange(orig_passings_list);
                PassingsForSession = new ObservableCollection<Passing>(new_passings_list.OrderBy(p => p.RaceTime).OrderBy(p => p.RFID)); 
            }
            else
            {
                //Remove data from Passing collection
                List<Passing> tmp = new List<Passing>();
                foreach (Passing p in PassingsForSession) 
                {
                    if (p.RFID == rfid) tmp.Add(p);
                }
                foreach (Passing p in tmp)
                    PassingsForSession.Remove(p);
            }
            if (PassingsForSession.Count() > 0)
                CurrentPassingForSession = PassingsForSession.First();
            else
                CurrentPassingForSession = null;
        }

        // 
        // Includes all competitors into Passing collection for Session
        //
        public void AllCompetitorsInTiming(Session session)
        {
            var hc = _applicationPresenter.HardcardContext;
            foreach (Standing st in hc.Standings)
                st.DisplayInPassing = true;
            PassingsForSession = InitPassingsForSession(session);
        }

        // 
        // Excludes all competitors into Passing collection for Session
        //
        public void NoCompetitorsInTiming(Session session)
        {
            if (session == null) return;
            var hc = _applicationPresenter.HardcardContext;
            foreach (Standing st in hc.Standings)
                st.DisplayInPassing = false;
            PassingsForSession = new ObservableCollection<Passing>();
            CurrentPassingForSession = null;
        }

        // 
        // Populate SessionId and Deleted fields in Passing table.
        //
        // Fetch from Passing table all records with 
        // Session.RaceStartTime <= Passing.RaceTime <= Session.RaceSchedStopTime
        // and set SessionId for each record.
        // 
        // It doesn't sort Passing collection. To sort it call SetLapTimeForPassingTable() 
        //
        public void SetSessionIdForPassingTable(Session session)
        {
            var hc = _applicationPresenter.HardcardContext;
            long sessionId = session.Id;

            List<Passing> query = (from p in hc.Passings
                         from e in hc.Entries
                         where e.SessionId == sessionId
                            && p.SessionId == null      // set if SessionId is NULL only
                            && e.RFID == p.RFID
                            && session.RaceStartTime <= p.RaceTime
                            && p.RaceTime <= session.RaceSchedStopTime
                               select p).ToList();
            if (query.Count > 0)
            {
                foreach (Passing p in query)
                {
                    // set SessionId, Deleted, and Invalid
                    p.SessionId = sessionId;
                    //change value only if it is null
                    if (p.Deleted == null) p.Deleted = false;
                    if (p.Invalid == null) p.Invalid = false;

                }
                hc.SaveChanges();
            }
        }
        // 
        // Populate LapNo, LapTime, and Invalid (with 'true' if  LapTime < MinLapTime) fields in Passing table.
        //
        // SessionId must be populated in Passing Table.
        // 
        public void SetLapTimeForPassingTable(Session session)
        {
            var hc = _applicationPresenter.HardcardContext;
            hc.SaveChanges();
            long sessionId = session.Id;

            // TODO: Select only rows with p.SessionId = null 
            List<Passing> query = (from p in hc.Passings
                                   where p.SessionId == sessionId
                                   &&    p.Deleted == false
                                   select p).ToList();
            if (query.Count > 0)
            {
                List<Passing> passingsSorted = query.OrderBy(p => p.RaceTime).OrderBy(p => p.RFID).ToList();

                long prevRFID = -1; // ???? can be char!!!
                long currRFID = -1;
                long prevRaceTime = 0;
                short lapNo = 0;

                foreach (Passing p in passingsSorted)
                {
                    // set SessionId and Deleted
                    p.SessionId = sessionId;
                    if (p.Deleted == null) p.Deleted = false;

                    // set LapNo, LapTime, and Invalid (based on MinLapTime)
                    currRFID = p.RFID; 
                    if (currRFID == prevRFID) // same competitor
                    {
                        p.LapTime = p.RaceTime - prevRaceTime;
                        if (p.LapTime >= session.MinLapTime)
                        {
                            p.Invalid = false;
                            ++lapNo;
                        }
                        else
                        {
                            p.Invalid = true;
                        }
                        p.LapNo = lapNo;
                    }
                    else // new competitor
                    {
                        lapNo = 0;
                        p.LapNo = lapNo;
                        p.Invalid = false;
                        p.LapTime = 0;
                    }
                    prevRaceTime = p.RaceTime;
                    prevRFID = currRFID;
                }
                hc.SaveChanges();
            }
        }

        // Create sorted Passing collection for Session
        // and set default CurrentPassingForSession
        //
        // SessionId, Deleted, LapNo and LapTime fields should be populated before
        // (call SetSessionId and SetLapTimeForPassingTable())
        //
        public ObservableCollection<Passing> InitPassingsForSession(Session session)
        {
            ObservableCollection<Passing> Tmp;
            if (session == null)
                Tmp = new ObservableCollection<Passing>();
            else
                SetSessionIdForPassingTable(session);
            Tmp = new ObservableCollection<Passing>(session.Passings.OrderBy(p => p.RaceTime).OrderBy(p => p.RFID).ToList());
            if (Tmp.Count() > 0)
                CurrentPassingForSession = Tmp.First();
            else
                CurrentPassingForSession = null;
            return Tmp;
        }

        // 
        // Populate SessionId, Deleted, LapNo and LapTime fields in Passing table based on RaceTime
        // Call InitPassingsForSession() after those fields are set.
        //
        public void SetSessionIdAndLapTimeForPassingTable(Session session)
        {
            if (session == null) return;
            SetSessionIdForPassingTable(session);
            SetLapTimeForPassingTable(session);
            //PassingsForSession = InitPassingsForSession(session);
        }
        // 
        // After updating of Deleted or RaceTime fields or adding new Passing row  to table or changing Minimum Lap Time,
        // this method  will recalculate such values as: LapNo and LapTime fields in Passing table and will update data in standing table.
        //
        public void ReCalculatePassingsAndStandings(Session session)
        {
            if (session == null) return;
            var hc = _applicationPresenter.HardcardContext;
            hc.SaveChanges();
            SetLapTimeForPassingTable(session);
            hc.SaveChanges();
            PassingsForSession = InitPassingsForSession(session);
            UpdateStandingTable(session);
            hc.SaveChanges();
        }

        // 
        // Removes passing Row from Database forever.
        // 
        public void RemovePassingRow(Session session)
        {
            if (session == null) return;
            var hc = _applicationPresenter.HardcardContext;
            hc.Passings.DeleteObject(CurrentPassingForSession);
            hc.SaveChanges();
            ReCalculatePassingsAndStandings(session);

        }

        // 
        // Insert new row into Standings table before the Current Passing row.
        // Algorithm should be approved.
        // 
        public void InsertPassingRow(Session session)
        {
            if (session == null) return;
            if (CurrentPassingForSession == null) return;
            long passingId = 0; 
            var hc = _applicationPresenter.HardcardContext;
            //Get max passingId in Passing table
            if ((from p in hc.Passings select p).Count() > 0)
                passingId = (long)(from p in hc.Passings select p.Id).Max() + 1;

            Passing pas = new Passing();
            pas.Id = passingId;
            pas.RFID = CurrentPassingForSession.RFID;
            if (CurrentPassingForSession.LapNo == 0) //it is a start time!???
            {
                pas.RaceTime = CurrentPassingForSession.RaceTime;
            }
            else
            {
                List<Passing> query = (from p in hc.Passings
                                       where p.SessionId == session.Id
                                          && p.RFID == CurrentPassingForSession.RFID
                                          && p.Deleted == false
                                          && p.Invalid == false
                                          && p.LapNo == CurrentPassingForSession.LapNo - 1
                                       select p).ToList();
                if (query.Count != 1)
                {
                    System.Windows.MessageBox.Show("Error during insertion");
                }
                Passing prev = query.First();
                pas.RaceTime = prev.RaceTime + (long)(0.5*(CurrentPassingForSession.RaceTime - prev.RaceTime));
            }
            
            hc.Passings.AddObject(pas);

            hc.SaveChanges();
            SetSessionIdAndLapTimeForPassingTable(session);
            hc.SaveChanges();
            UpdateStandingTable(session);
            hc.SaveChanges();
            ReCalculatePassingsAndStandings(session);
            if (pas != null) CurrentPassingForSession = pas;
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
                //SetSessionIdForPassingTable(session);
                Tmp = GetStandingsForSession(session);
            }
            return Tmp;
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
                      && p.Deleted != true
                      && p.Invalid != true
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
                    st.DisplayInPassing = true;

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
        // Update list of Standings for Session: 
        // Delete records from Standing 
        // and Recalculate records for Standing based on Passing table.
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
            //EntriesForSession = InitEntriesForSession(session); // Entry table is used. ???
            //InitEntriesForSession(session);
            //PassingsForSession = InitPassingsForSession(session);
            StandingsForSession = InitStandingsForSession(session);
            return;
       }

        
        // TEST ONLY
        // Create records from tag for each competitor (Entry) and for each lap.
        // Write it into Passing collection.
        // It is used for test only
        //
        // 2012-08-27 Results gives 0.12 sec/row
        public void WriteTestDataToPassing()
        {
            if (CurrentSessionForEvent == null) return;
            Session session = CurrentSessionForEvent;
            var hc = _applicationPresenter.HardcardContext;
            long sessionId = CurrentSessionForEvent.Id;


            // Remove all records from Passings 
            if ((hc.Passings.Count() > 0))
            {
                List<Passing> query_for_sess = (from p in hc.Passings
                                                where p.SessionId == sessionId
                                                select p).ToList();
                if (query_for_sess.Count > 0)
                    foreach (Passing p in query_for_sess) hc.Passings.DeleteObject(p);
                hc.SaveChanges();
            }

            int NumOfLaps = (int)session.SchedLaps;
            long passingId = 0;

            //Get max passingId in Passing table
            if ( (from p in hc.Passings select p).Count() > 0)
            passingId = (long)(from p in hc.Passings select p.Id).Max() + 1;
            Random random = new Random();
            
            //init RaseTime for each Competitor for Session. Use Array[competitor] to keep TaceTime
            long startTime = (long)session.RaceStartTime;
            int entryCount = EntriesForSession.Count();
            const int bnd = 1000;
            long[] raceTime =  new long[bnd];
            if (entryCount > bnd)
            {
                System.Windows.MessageBox.Show("Array raceTime[] too small"); // exit
                return;
            }

            for (int i = 0; i < entryCount; i++)
             raceTime[i] = (startTime);
            //long entryRaceTime = raceTime[j_e];


            for (short lap = 0; lap < NumOfLaps; ++lap)
            {
                if (EventAndSessionSearchBar.RaceStarted == false) break;
                int j_e = 0;
                foreach (Entry e in EntriesForSession)
                {
                    Passing pas = new Passing();
                    pas.Id = passingId++;
                    pas.RFID = e.RFID;
                    pas.RaceTime = raceTime[j_e];
                    hc.Passings.AddObject(pas);
                    
                    hc.SaveChanges();
                    SetSessionIdAndLapTimeForPassingTable(session);
                    hc.SaveChanges();
                    UpdateStandingTable(session);
                    hc.SaveChanges();

                    // just nice view in real time. Replace with Threads and timer later

                    System.Windows.Forms.Application.DoEvents();
                    System.Threading.Thread.Sleep(1000); 

                    raceTime[j_e] += random.Next(20000, 30000);//lapTime ~ 20 . . . 30 sec
                    j_e++;
                }
                /*
                if ((lap % 3) == 0)
                {
                    //View.TimingTab.InvalidateVisual();
                    PassingsForSession = InitPassingsForSession(session);
                    StandingsForSession = InitStandingsForSession(session);
                }
                 */ 

            }
            ReCalculatePassingsAndStandings(session);
        }
        //
        // Save Standings for Session into file
        //
        public void WriteStandingReport(Session session)
        {
            if (session == null) return;
            var hc = _applicationPresenter.HardcardContext;
            if ((hc.Standings.Count() > 0))
            {
                System.Windows.Forms.SaveFileDialog ofd = new System.Windows.Forms.SaveFileDialog();

                ofd.InitialDirectory = REPORTFOLDER;
                ofd.Filter = "All events (*.rpt)|*.rpt";
                //ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                System.Windows.Forms.DialogResult res = ofd.ShowDialog();
                string reportFileName = "";
                

                if (res == System.Windows.Forms.DialogResult.OK)
                {
                    reportFileName = ofd.FileName;
                    List<Standing> query = (from c in hc.Standings
                                            select c).ToList();

                    using (StreamWriter writer = new StreamWriter(reportFileName, false))
                    {
                        writer.WriteLine("");
                        writer.WriteLine("Hardcard Standing Report:                           Time:{0}", System.DateTime.Now);
                        writer.WriteLine("");
                        writer.WriteLine("");
                        writer.WriteLine("Last Name           " + "First Name          " +
                                          "Comp No " + "Position " + "Laps Completed " + "Completed Time " + 
                                          "Best Lap Time  "  +  "Avg Lap Time   " + "Worst Lap Time " +  "Class          " );
                        string a = "-";
                        writer.WriteLine(a.PadRight(147, '-'));

                        string firstName = "First1";
                        string lastName = "Last1";
                        string  competitionNumber = "a111";
                        string fmtTimeSpan = @"hh\:mm\:ss\.FFFFF";
                        foreach (Standing e in query)
                        {
                            firstName = e.Entry.Competitor.Athlete.FirstName; 
                            writer.Write(firstName.PadRight(20, ' '));
                            lastName = e.Entry.Competitor.Athlete.LastName;
                            writer.Write(lastName.PadRight(20, ' '));
                            competitionNumber = e.Entry.CompetitionNo;
                            writer.Write(competitionNumber.PadRight(8, ' '));
                            writer.Write((e.Position).ToString().PadRight(9, ' '));
                            writer.Write((e.LapsCompleted).ToString().PadRight(15,' '));
                            writer.Write(string.Format(TimeSpan.FromMilliseconds((double)e.CompletedTime).ToString(fmtTimeSpan)).PadRight(15, ' '));
                            writer.Write(string.Format(TimeSpan.FromMilliseconds((double)e.BestLapTime).ToString(fmtTimeSpan)).PadRight(15, ' '));
                            writer.Write(string.Format(TimeSpan.FromMilliseconds((double)e.AvgLapTime).ToString(fmtTimeSpan)).PadRight(15, ' '));
                            writer.Write(string.Format(TimeSpan.FromMilliseconds((double)e.WorstLapTime).ToString(fmtTimeSpan)).PadRight(14, ' '));
                            writer.WriteLine("");
                        }
                    }                     
                }          
                else                        
                {
                    return;
                }
            }
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

        /*
        // Copy Value from new event to event in Db
        public void AddCompetitorToTiming(Standing standing)
        {
            if (standing == null) return;



        }
        // Copy Value from new event to event in Db
        public void UpdateSession(Session newSession, Session dbSession)
        {
                    Presenter.RemoveCompetitorFromTiming(chbx.DataContext as Standing);
        */
     
    }
}
