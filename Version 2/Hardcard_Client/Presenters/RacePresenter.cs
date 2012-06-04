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

        private NetworkListener networkListener;
        private TagSubscriber passingsLogger;
        private ProcessBufferedReadings passingDetector;
        private PriorityCollectionBlocking<TagInfo> passingsQueue;
        private BufferReadings passingsBuffer;

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
            int queueCapacity = 10240;

            _passingsList = new ObservableCollection<Passing>();

            /*Start nework listener and detect passings*/
            passingsLogger = new TagSubscriber(UpdatePassingsList);

            passingsQueue = new PriorityCollectionBlocking<TagInfo>("Queue", queueCapacity);

            passingsBuffer = new BufferReadings(passingsQueue);
            passingDetector = new ProcessBufferedReadings("Pass Detector", passingsQueue);

            networkListener = new NetworkListener("Race Host", networkPort);

            passingsBuffer.AddPublisher(networkListener);
            passingsLogger.AddPublisher(passingDetector);

            passingDetector.Start();
            networkListener.Start();

            StatusText = "Race Started";
        }

        public void UpdatePassingsList(TagReadEventArgs e)
        {
            lock (lockObject)
            {
                Passing newPassing = new Passing();
                //The tags receved from the simulator will not be valid 
                //in the current database, seeting it to default - 1000.
                /*
                long id;
                if (long.TryParse(e.TagInfo.ID.ToString(), out id))
                {
                    newPassing.RFID = id;
                }
                else
                {
                    newPassing.RFID = 0;
                };*/
                newPassing.RFID = 1000;
                newPassing.SessionId = currentSession;
                newPassing.RaceTime = e.TagInfo.Time;
                newPassing.LastUpdated = DateTime.UtcNow;
                hardcardContext.AddToPassings(newPassing);
                hardcardContext.SaveChanges();

                List<Passing> temp_passings_list = new List<Passing>();
                temp_passings_list = PassingsList.ToList();
                temp_passings_list.Add(newPassing);
                PassingsList = new ObservableCollection<Passing>(temp_passings_list.ToList());
            }
        }

        public void StopRace()
        {
            passingDetector.Exit();
            networkListener.End();
            StatusText = "Race Stopped";
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
                    //newPassing.LapTime = id - 10000 + (laptime%3);
                    //newPassing.Lap = lap;
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
            /*    
            if (CurrentEcent.Contains(competitor))
                AllCompetitors.Remove(competitor);
            _competitorRepository.Delete(competitor);
            */
            MessageBox.Show("N/E");
            StatusText = string.Format("Event '{0}' was deleted.", session.EventClassId);
        }

        public void DeleteRace(Session session)
        {
            /*    
            if (CurrentEcent.Contains(competitor))
                AllCompetitors.Remove(competitor);
            _competitorRepository.Delete(competitor);
            */
            MessageBox.Show("N/E");
            StatusText = string.Format("Event '{0}' was deleted.", session.StartTime);
        }


    }
}
