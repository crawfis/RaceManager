using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RacingEventsTrackSystem.Model;
using RacingEventsTrackSystem.Views;
using RacingEventsTrackSystem.DataAccess;
using System.Collections.ObjectModel;
using System.Threading;
using Hardcard.Scoring;
using OhioState.Collections;
using System.Windows;

namespace RacingEventsTrackSystem.Presenters
{
    public class RacePresenter : Notifier
    {
        private HardcardEntities hardcardContext;

        private RaceManagerView _view;
        private ObservableCollection<Passing> _passingsList;
        private ObservableCollection<String> _eventList;
        private ObservableCollection<long> _sessionList;
        private bool _manualPassing = false;
        private String _statusText;

        private long currentSession;
        private String currentEvent;

        private HardcardServer server;
        private TagSubscriber passingsLogger;

        private Object lockObject;

        private long lastPassing;
        private Thread passingsThread;
        private bool stopThread = false;

        // TODO put this in a config
        private int networkPort = 3900;

        public RacePresenter(RaceManagerView view)
        {
            _view = view;
            hardcardContext = new HardcardEntities();
            IQueryable<String> events = 
                from p in hardcardContext.Events 
                orderby p.EventName
                select p.EventName;
            _eventList = new ObservableCollection<string>(events.ToList());
            lockObject = new Object();
        }

        public ObservableCollection<String> EventList
        {
            get { return _eventList; }
            set
            {
                _eventList = value;
                OnPropertyChanged("EventList");
            }
        }

        public ObservableCollection<long> SessionList
        {
            get { return _sessionList; }
            set
            {
                _sessionList = value;
                OnPropertyChanged("SessionList");
            }
        }

        public ObservableCollection<Passing> PassingsList
        {
            get { return _passingsList; }
            set
            {
                _passingsList = value;
                OnPropertyChanged("PassingsList");
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

        public bool ManualPassing
        {
            get { return _manualPassing; }
            set
            {
                _manualPassing = value;
                OnPropertyChanged("PopupPassing");
            }
        }

        public void EventSelected()
        {
            currentEvent = _view.eventComboBox.SelectedValue.ToString();
            IQueryable<long> sessions =
                from p in hardcardContext.Sessions
                select p.Id;
            SessionList = new ObservableCollection<long>(sessions.ToList());
        }

        public void SessionSelected()
        {
            currentSession = (long) long.Parse(_view.sessionComboBox.SelectedValue.ToString());
        }

        public void StartRace()
        {

            HardcardServer.NetworkPort = networkPort;
            server = new HardcardServer("Hardcard Race System");
            passingsLogger = new Hardcard.Scoring.TagSubscriber(UpdatePassingsList);
            passingsLogger.AddPublisher(server.PassingsPublisher);

            server.Start();

            StatusText = "Hardcard Server Started";
        }

        public void UpdatePassingsList(TagReadEventArgs e)
        {
            lock (lockObject)
            {
                Passing newPassing = new Passing();
                long rfid = 1000;
                Int64.TryParse(e.TagInfo.ID.Value, out rfid);
                newPassing.RFID = rfid;
                newPassing.SessionId = null;
                newPassing.RaceTime = e.TagInfo.Time;
                newPassing.LastUpdated = DateTime.UtcNow;
                hardcardContext.AddToPassings(newPassing);
                hardcardContext.SaveChanges();

                List<Passing> temp_passings_list;
                temp_passings_list = PassingsList.ToList();
                temp_passings_list.Add(newPassing);
                PassingsList = new ObservableCollection<Passing>(temp_passings_list.ToList());
            }
        }

        public void StopRace()
        {
            server.End();
            StatusText = "Hardcard Server Stopped";
        }

        public void FakeStartRace()
        {  
            int numpassings = 0;
            stopThread = false;
            numpassings = (from p in hardcardContext.Passings
                           select p.Id).Count();
            if (numpassings > 0)
            {
                lastPassing =
                    (from p in hardcardContext.Passings
                     select p.Id).Max();
            }
            else
            {
                lastPassing = 0;
            }
            IQueryable<Passing> passings =
                from p in hardcardContext.Passings
                where p.Id > lastPassing
                select p;
            PassingsList = new ObservableCollection<Passing>(passings.ToList());
            StatusText = "Race Started";

            passingsThread = new Thread(FakePassings);
            passingsThread.Start();
        }

        public void FakeStopRace()
        {
            stopThread = true;
            passingsThread.Join();
            StatusText = "Race Stopped";
        }

        public void FakePassings()
        {
            int racetime = 0;
            short lap = 0;
            short laptime = 4;
            int id;
            while (!stopThread)
            {
                Thread.Sleep(1000);
                for (id = 10000; id < 10002; id++)
                {
                    Passing newPassing = new Passing();
                    newPassing.RFID = id;
                    newPassing.SessionId = 1;
                    racetime = racetime + id - 10000 + (laptime%3);
                    newPassing.RaceTime = racetime;
                    hardcardContext.AddToPassings(newPassing);
                    hardcardContext.SaveChanges();
                }
                IQueryable<Passing> passings = from p in hardcardContext.Passings
                                               where p.Id > lastPassing
                                               orderby p.Id descending
                                               select p;
                PassingsList = new ObservableCollection<Passing>(passings.ToList());
                lap++;
            }
        }

        public void AddNewPassing()
        {
            ManualPassing = true;
        }
        public void SaveRace<T>(Session session, PresenterBase<T> presenter)
        {
            MessageBox.Show("N/E");
            StatusText = string.Format("Event '{0}' was deleted.", session.EventClassId);
        }

        public void DeleteRace(Session session)
        {
            MessageBox.Show("N/E");
            StatusText = string.Format("Event '{0}' was deleted.", session.StartTime);
        }


    }
}
