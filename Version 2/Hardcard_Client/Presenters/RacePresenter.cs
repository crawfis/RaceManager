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
    public class RacePresenter : PresenterBase<Shell>
    {
        private HardcardEntities hardcardContext;
        private readonly ApplicationPresenter _applicationPresenter;
        private Shell _view;

        private ObservableCollection<String> _eventList;
        private bool _manualPassing = false;
        private String _statusText;

        private HardcardServer server;
        private TagSubscriber passingsLogger;

        private Object lockObject;

        // TODO put this in a config
        private int networkPort = 3900;

        public ApplicationPresenter ApplicationPresenter
        {
            get { return _applicationPresenter; }
            set { }
        }

        public RacePresenter(ApplicationPresenter applicationPresenter, 
                                   Shell view
                                   ) : base(view)
        {
            try 
            { 
                _applicationPresenter = applicationPresenter;

                _view = view;
                hardcardContext = new HardcardEntities();
                IQueryable<String> events =
                    from p in hardcardContext.Events
                    orderby p.EventName
                    select p.EventName;
                _eventList = new ObservableCollection<string>(events.ToList());
                lockObject = new Object();
                StatusText = ("RacePresenter constructor no error");
            } 
            catch (Exception ex)
            {
                StatusText = "RacePresenter constructor failed with error: " + ex.Message;
                MessageBox.Show(StatusText); // stop executable
            }
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

        public void StartRace()
        {

            HardcardServer.NetworkPort = networkPort;
            server = new HardcardServer("Hardcard Race System");
            passingsLogger = new Hardcard.Scoring.TagSubscriber(UpdatePassingsList);
            passingsLogger.AddPublisher(server.PassingsPublisher);

            server.Start();

            StatusText = "Hardcard Server Started";
        }
        public void StartTestRace()
        {
            ApplicationPresenter.AllSessionsPresenter.WriteTestDataToPassing();
        }

        //
        // Add one row to PassingsForSession list.
        //
        public void UpdatePassingsList(TagReadEventArgs e)
        {
            lock (lockObject)
            {
                Passing newPassing = new Passing();
                // TODO: Change the database to string or only allow Hardcard tags to be longs.
                long rfid = 1000;
                Int64.TryParse(e.TagInfo.ID.Value, out rfid);
                newPassing.RFID = rfid;
                newPassing.SessionId = null;
                newPassing.LapNo = null;
                newPassing.LapTime = null;
                newPassing.RaceTime = e.TagInfo.Time;
                //newPassing.PassingTime = AllSessionsPresenter.ConvertFromUnixTime(newPassing.RaceTime);
                newPassing.LastUpdated = DateTime.UtcNow;
                hardcardContext.AddToPassings(newPassing);
                hardcardContext.SaveChanges();

                List<Passing> temp_passings_list;
                temp_passings_list =  (ApplicationPresenter.AllSessionsPresenter.PassingsForSession).ToList();
                temp_passings_list.Add(newPassing);
                ApplicationPresenter.AllSessionsPresenter.PassingsForSession = new ObservableCollection<Passing>(temp_passings_list);
            }
        }

        public void StopRace()
        {
            server.End();
            StatusText = "Hardcard Server Stopped";
        }
    }
}
