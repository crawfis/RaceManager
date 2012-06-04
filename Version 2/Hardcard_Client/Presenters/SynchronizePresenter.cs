using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RacingEventsTrackSystem.Model;
using RacingEventsTrackSystem.Views;
using RacingEventsTrackSystem.DataAccess;
using System.Collections.ObjectModel;
using System.Threading;

namespace RacingEventsTrackSystem.Presenters 
{
    public class SynchronizePresenter : Notifier
    {
        private SynchronizeView _synchronizeView;
        private SynchronizeModel _synchronizeModel;
        private String _statusText;
        private ObservableCollection<String> _eventsList;
        private HardcardEntities _hardcardContext;

        private Thread synchronizeThread;
        private bool threadEnabled = false;

        public SynchronizePresenter(SynchronizeView view, SynchronizeModel syncrhonize)
        {
            _synchronizeView = view;
            _synchronizeModel = syncrhonize;
            _hardcardContext = new HardcardEntities();
            IQueryable<String> events = 
                from p in _hardcardContext.Events  
                orderby p.EventName
                select p.EventName;
            List<String> tmpEventsList = events.ToList();
            tmpEventsList.Insert(0, "All");
            EventsList = new ObservableCollection<String>(tmpEventsList);
        }

        public SynchronizeModel SynchronizeModel
        {
            get { return _synchronizeModel; }
        }

        public ObservableCollection<String> EventsList
        {
            get { return _eventsList;}
            set {
                _eventsList = value;
                OnPropertyChanged("EventsList"); 
            }
        }

        public void Synchronize()
        {
            bool result = true;
            hardcardserver.SyncService syncService = new hardcardserver.SyncService();

            if (_synchronizeModel.sync_competitors)
            {
                result = Synchronize_Competitors(syncService);
                if (!result)
                {
                    return;
                }
            }
            if (_synchronizeModel.sync_events)
            {
                result = Synchronize_Events(syncService);
                if (!result)
                {
                    return;
                }
                result = Synchronize_RaceClasses(syncService);
                if (!result)
                {
                    return;
                }
                result = Synchronize_EventClasses(syncService);
                if (!result)
                {
                    return;
                }
                result = Synchronize_Sessions(syncService);
                                if (!result)
                {
                    return;
                }
                result = Synchronize_Passings(syncService);
                if (!result)
                {
                    return;
                }
            }

            LogSynchronization();

            if (threadEnabled == true) {
                StatusText = "Synchronization service running successfully.";
            } else {
                StatusText = "Synchronization completed successfully";
            }
        }

        public bool Synchronize_Competitors(hardcardserver.SyncService ss)
        {
            bool result = true;

            List<hardcardserver.Competitor> crPushList = new List<hardcardserver.Competitor>();
            hardcardserver.Competitor[] crPullArray = null;
            DateTime lastSync;
            try
            {
                    /*Get the last synchronized timestamp*/
                    lastSync = GetLastSynchronized();

                    /*Get customer records to be syncrhonized to the server*/
                    IEnumerable<Competitor> cfound =
                        from c in _hardcardContext.Competitors
                        where (DateTime.Compare(c.LastUpdated.Value, lastSync) > 0)
                        select c;

                    foreach (var clocal in cfound)
                    {
                        hardcardserver.Competitor cnew = new hardcardserver.Competitor();
                        /*TODO: is there a better way to manage this assigment, maybe an 
                         overloaded operator*/
                 /*EU       cnew.CompetitorID = clocal.CompetitorID;
                        cnew.FirstName = clocal.FirstName;
                        cnew.LastName = clocal.LastName;
                        cnew.Gender = clocal.Gender;
                        cnew.DOB = clocal.DOB;
                        cnew.VehicleType = clocal.VehicleType;
                        cnew.VehicleModel = clocal.VehicleModel;
                        cnew.VehicleCC = (int) clocal.VehicleCC;
                        cnew.ClassID = clocal.ClassID;
                        cnew.AddressLine1 = clocal.AddressLine1;
                        cnew.AddressLine2 = clocal.AddressLine2;
                        cnew.AddressLine3 = clocal.AddressLine3;
                        cnew.City = clocal.City;
                        cnew.State = clocal.State;
                        cnew.Country = clocal.Country;
                        cnew.PostalCode = clocal.PostalCode;
                        cnew.Phone = clocal.Phone;
                        crPushList.Add(cnew);
                  */
                    }
                    result = ss.SynchronizeCompetitorDataWithClient(lastSync, crPushList.ToArray(), out crPullArray);
            }
            catch (Exception ex)
            {
                StatusText = "Syncrhonization of Competitors failed with error: " + ex.Message;
                System.Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        public bool Synchronize_Events(hardcardserver.SyncService ss)
        {
            bool result = true;

            List<hardcardserver.Event> eventsPushList = new List<hardcardserver.Event>();
            DateTime lastSync;
            try
            {
                /*Get the last synchronized timestamp*/
                lastSync = GetLastSynchronized();

                /*Get customer records to be syncrhonized to the server*/
                IEnumerable<Event> evfound =
                    from ev in _hardcardContext.Events
                    select ev;

                foreach (var evlocal in evfound)
                {
                    hardcardserver.Event newEvent = new hardcardserver.Event();
                    /*TODO: is there a better way to manage this assigment, maybe an 
                     overloaded operator*/

                    newEvent.EventID = (int)evlocal.Id;
                    newEvent.EventName = evlocal.EventName;
                    newEvent.EventLocation = evlocal.EventLocation;
                    eventsPushList.Add(newEvent);
                }
                result = ss.SynchronizeEventsDataWithClient(lastSync, eventsPushList.ToArray());
            }
            catch (Exception ex)
            {
                StatusText = "Syncrhonization of Events failed with error: " + ex.Message;
                System.Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        public bool Synchronize_RaceClasses(hardcardserver.SyncService ss)
        {
            bool result = true;

            List<hardcardserver.RaceClass> raceclassPushList = new List<hardcardserver.RaceClass>();
            DateTime lastSync;
            try
            {
                /*Get the last synchronized timestamp*/
                lastSync = GetLastSynchronized();

                /*Get customer records to be syncrhonized to the server*/
                IEnumerable<RaceClass> rcfound =
                    from rc in _hardcardContext.RaceClasses
                    select rc;

                foreach (var rclocal in rcfound)
                {
                    hardcardserver.RaceClass newRaceClass = new hardcardserver.RaceClass();
                    /*TODO: is there a better way to manage this assigment, maybe an 
                     overloaded operator*/

                    newRaceClass.ClassID = (int)rclocal.Id;
                    newRaceClass.ClassName = rclocal.ClassName;
                    newRaceClass.VehicleCC = (int) rclocal.VehicleCC;
                    newRaceClass.Deleted = rclocal.Deleted;
                    newRaceClass.MaxAge = rclocal.MaxAge;
                    newRaceClass.MinAge = rclocal.MinAge;
                    newRaceClass.LastUpdated = rclocal.LastUpdated;
                    raceclassPushList.Add(newRaceClass);
                }
                result = ss.SynchronizeRaceClassesDataWithClient(lastSync, raceclassPushList.ToArray());
            }
            catch (Exception ex)
            {
                StatusText = "Syncrhonization of RaceClasses failed with error: " + ex.Message;
                System.Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        public bool Synchronize_EventClasses(hardcardserver.SyncService ss)
        {
            bool result = true;

            List<hardcardserver.EventClass> eventclassPushList = new List<hardcardserver.EventClass>();
            DateTime lastSync;
            try
            {
                /*Get the last synchronized timestamp*/
                lastSync = GetLastSynchronized();

                /*Get customer records to be syncrhonized to the server*/
                IEnumerable<EventClass> rcfound =
                    from rc in _hardcardContext.EventClasses
                    select rc;

                foreach (var rclocal in rcfound)
                {
                    hardcardserver.EventClass newEventClass = new hardcardserver.EventClass();
                    /*TODO: is there a better way to manage this assigment, maybe an 
                     overloaded operator*/

                    newEventClass.EventClassID = (int)rclocal.Id;
                    newEventClass.EventID = (int)rclocal.EventId;
                    newEventClass.ClassID = (int)rclocal.ClassId;
                    eventclassPushList.Add(newEventClass);
                }
                result = ss.SynchronizeEventClassesDataWithClient(lastSync, eventclassPushList.ToArray());
            }
            catch (Exception ex)
            {
                StatusText = "Syncrhonization of EventClasses failed with error: " + ex.Message;
                System.Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        public bool Synchronize_Sessions(hardcardserver.SyncService ss)
        {
            bool result = true;

            List<hardcardserver.Session> sessionsPushList = new List<hardcardserver.Session>();
            DateTime lastSync;
            try
            {
                /*Get the last synchronized timestamp*/
                lastSync = GetLastSynchronized();

                /*Get customer records to be syncrhonized to the server*/
                IEnumerable<Session> sfound =
                    from s in _hardcardContext.Sessions
                    select s;

                foreach (var slocal in sfound)
                {
                    hardcardserver.Session newSession = new hardcardserver.Session();
                    /*TODO: is there a better way to manage this assigment, maybe an 
                     overloaded operator*/

                    newSession.SessionID = (int)slocal.Id;
                    newSession.EventClassID = (int)slocal.EventClassId;
                    newSession.StartTime = slocal.StartTime;
                    sessionsPushList.Add(newSession);
                }
                result = ss.SynchronizeSessionsDataWithClient(lastSync, sessionsPushList.ToArray());
            }
            catch (Exception ex)
            {
                StatusText = "Syncrhonization of Sessions failed with error: " + ex.Message;
                System.Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        public bool Synchronize_Passings(hardcardserver.SyncService ss)
        {
            bool result = true;

            List<hardcardserver.Passing> passingsPushList = new List<hardcardserver.Passing>();
            DateTime lastSync;
            try
            {
                /*Get the last synchronized timestamp*/
                lastSync = GetLastSynchronized();

                /*Get customer records to be syncrhonized to the server*/
                IEnumerable<Passing> pfound =
                    from p in _hardcardContext.Passings 
                    where (DateTime.Compare(p.LastUpdated.Value, lastSync) > 0) 
                    select p;

                foreach (var plocal in pfound)
                {
                    hardcardserver.Passing newPassing = new hardcardserver.Passing();
                    /*TODO: is there a better way to manage this assigment, maybe an 
                     overloaded operator*/

                    newPassing.PassingID = plocal.Id;
                    newPassing.RFID = plocal.RFID;
                    newPassing.PassngTime = plocal.PassingTime;
                    newPassing.RaceTime = plocal.RaceTime;
                    newPassing.SessionID = plocal.SessionId;
                    passingsPushList.Add(newPassing);
                }
                result = ss.SynchronizePassingsDataWithClient(lastSync, passingsPushList.ToArray());
                LogSynchronization();
            }
            catch (Exception ex)
            {
                StatusText = "Syncrhonization of Competitors failed with error: " + ex.Message;
                System.Console.WriteLine(ex.Message);
                return false;
            }

            return true;
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

        public DateTime GetLastSynchronized()
        {
            return (from s in _hardcardContext.Synchronizations
                    select s.SyncDate).Max();
        }

        public void LogSynchronization()
        {
            Synchronization sync = new Synchronization();
            try
            {
                sync.SyncDate = DateTime.UtcNow;
                if ((from s in _hardcardContext.Synchronizations
                     select s.Id).Count() > 0)
                {
                    sync.Id = (from s in _hardcardContext.Synchronizations
                               select s.Id).Max() + 1;
                }
                else
                {
                    sync.Id = 1000;
                }
                _hardcardContext.AddToSynchronizations(sync);
                _hardcardContext.SaveChanges();
            }
            catch (Exception ex)
            {
                /*Need better error logging or display*/
                System.Console.WriteLine(ex.Message);
            }
        }

        public void Toggle_Synchronization_Service()
        {
            if (threadEnabled == false)
            {
                _synchronizeView.SyncThreadButton.Content = "Turn Off Synchronization Service";
                threadEnabled = true;
                synchronizeThread = new Thread(SynchronizeThread);
                synchronizeThread.Start();
            }
            else
            {
                _synchronizeView.SyncThreadButton.Content = "Turn On Synchronization Service";
                StatusText = "Synchronization service stopped.";
                threadEnabled = false;
            }
        }

        public void SynchronizeThread()
        {
            while (threadEnabled)
            {
                Synchronize();
                Thread.Sleep(3000);
            }
        }
    }
}
