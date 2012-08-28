using System;
using System.Collections.ObjectModel;
using RacingEventsTrackSystem.Model;
using RacingEventsTrackSystem.Views;
using RacingEventsTrackSystem.DataAccess;
using RacingEventsTrackSystem.Presenters;
using System.Windows;

namespace RacingEventsTrackSystem.Presenters
{
    public partial class ApplicationPresenter : PresenterBase<Shell>
    {

        private string _statusText;
        public string StatusText
        {
            get { return _statusText; }
            set
            {
                _statusText = value;
                OnPropertyChanged("StatusText");
            }
        }

        private HardcardEntities _hardcardContext;
        public HardcardEntities HardcardContext
        {
            get { return _hardcardContext; }
            set { _hardcardContext = value; }
        }

        private AllEventsPresenter _allEventsPresenter;
        public AllEventsPresenter AllEventsPresenter
        {
            get { return _allEventsPresenter; }
            set { _allEventsPresenter = value; }
        }

        private AllRaceClassesPresenter _allRaceClassesPresenter;
        public AllRaceClassesPresenter AllRaceClassesPresenter
        {
            get { return _allRaceClassesPresenter; }
            set { _allRaceClassesPresenter = value; }
        }

        private AllCompetitorsPresenter _allCompetitorsPresenter;
        public AllCompetitorsPresenter AllCompetitorsPresenter
        {
            get { return _allCompetitorsPresenter; }
            set { _allCompetitorsPresenter = value; }
        }

        private AllAthletesPresenter _allAthletesPresenter;
        public AllAthletesPresenter AllAthletesPresenter
        {
            get { return _allAthletesPresenter; }
            set { _allAthletesPresenter = value; }
        }

        private RacePresenter _racePresenter;
        public RacePresenter RacePresenter
        {
            get { return _racePresenter; }
            set { _racePresenter = value; }
        }
    
        private AllSessionsPresenter _allSessionsPresenter;
        public AllSessionsPresenter AllSessionsPresenter
        {
            get { return _allSessionsPresenter; }
            set { _allSessionsPresenter = value; }
        }


        public ApplicationPresenter(Shell view,
                                    HardcardEntities hardcardContext) : base(view)
        {
            try
            {
                HardcardContext = hardcardContext;
                StatusText = ("ApplicationPresenter constructor no error");

            }
            catch (Exception ex)
            {
                StatusText = "ApplicationPresenter constructor failed with error: " + ex.Message;
            }
        }
        
        public void InitApplicationPresenter()
        {
            try
            {
                _allEventsPresenter = new AllEventsPresenter(this, View);
                _allAthletesPresenter = new AllAthletesPresenter(this, View);
                _allRaceClassesPresenter = new AllRaceClassesPresenter(this, View);
                _allSessionsPresenter = new AllSessionsPresenter(this, View);
                _allCompetitorsPresenter = new AllCompetitorsPresenter(this, View);
                _racePresenter = new RacePresenter(this, View);
            }
            catch (Exception ex)
            {
                StatusText = "ApplicationPresenter constructor failed with error: " + ex.Message;
            }
        }

        public void CloseView<T>(PresenterBase<T> presenter)
        {
            View.CloseView(presenter);
        }
    }
}
