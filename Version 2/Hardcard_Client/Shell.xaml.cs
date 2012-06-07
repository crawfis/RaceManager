using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


using RacingEventsTrackSystem.Presenters;
using RacingEventsTrackSystem.Model;
using RacingEventsTrackSystem.DataAccess;
using RacingEventsTrackSystem.UserControls;
using RacingEventsTrackSystem.Views;

using System.Data;
using System.Data.SqlClient;
using System.Data.Sql;


namespace RacingEventsTrackSystem
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : Window
    {
        public Shell()
        {
            InitializeComponent();
            //string myDatabase = RacingEventsTrackSystem.Properties.Settings.Default.HardcardConnectionString;
            //HardcardEntities hardcardContext = new HardcardEntities(myDatabase);
            HardcardEntities hardcardContext = new HardcardEntities();
            ApplicationPresenter ap = new ApplicationPresenter(this, hardcardContext);
            ap.InitApplicationPresenter();
            DataContext = ap;

            //DataContext for Controls
            ctlAllRaceClasses.DataContext = ap.AllRaceClassesPresenter;
            //ctlAllRaceClasses.DataContext = ap.allEventsPresenter;
            //lsbxEventClasses.DataContext = ap.allEventsPresenter;


            //DataContext for Views
            athleteView.DataContext = ap.AllAthletesPresenter;
            raceClassView.DataContext = ap.AllRaceClassesPresenter;

            //DataContext for SearchBar
            ctlEventSearchBar.DataContext = ap.AllEventsPresenter;
            ctlRaceClassSearchBar.DataContext = ap.AllEventsPresenter;

            // Desable buttons 'New' and "Delete' Athletes for GetCompetitors

            // Tab "Events"    
            //"Events List"/ctlAllEvents/AllEventsControl.xaml
            //"Edit Event"/eventView/EventView.xaml
            ctlAllEvents.DataContext = ap.AllEventsPresenter;
            eventView.DataContext = ap.AllEventsPresenter;

            // Tab "Athletes"    
            //"Athletes List"/ctlAllAthletes/AllAthletesControl.xaml
            //"Edit Athlete" /athleteView/AthleteView.xaml
            ctlAthleteSearchBar.DataContext = ap.AllEventsPresenter;
            ctlAllAthletes.DataContext = ap.AllAthletesPresenter;
            ctlAthleteSearchBar.txblEventName.Visibility = System.Windows.Visibility.Hidden;

            // Tab "Competitor"    
            //"Athletes List"/ctlAthletes/AllAthletesControl.xaml
            //"Competitors for Event" control: AllCompetitorsControl.xaml
            ctlAthletes.DataContext = ap.AllAthletesPresenter;
            //competitorView.DataContext = ap.AllCompetitorsPresenter;
            competitorView.DataContext = ap.AllCompetitorsPresenter;
            ctlCompetitorsSearchBar.DataContext = ap.AllEventsPresenter;
            ctlCompetitorsForEvent.DataContext = ap.AllCompetitorsPresenter;
            //ctlAllAthletes.btnAddAthleteToCompetitors.Visibility = System.Windows.Visibility.Hidden;
            ctlAthletes.btnNewAthlete.Visibility = System.Windows.Visibility.Hidden;
            ctlAthletes.btnDeleteAthlete.Visibility = System.Windows.Visibility.Hidden;

            competitorView.cmbxCompetitorStatus.SelectedIndex = 0;


            // Tab "Session"
            ctlSessionSearchBar.DataContext = ap.AllEventsPresenter;
            sessionView.DataContext = ap.AllSessionsPresenter;
            ctlAllSessions.DataContext = ap.AllSessionsPresenter;
            ctlCompetitorsForEventClass.DataContext = ap.AllSessionsPresenter;

            // Tab "Results"
            resultsView.DataContext = ap.AllSessionsPresenter;
            ctlAllResultsSessions.DataContext = ap.AllSessionsPresenter;
            ctlAllResultsSessions.btnExcludeSession.Visibility = System.Windows.Visibility.Hidden;
        }


        public ApplicationPresenter Presenter
        {
            get { return DataContext as ApplicationPresenter; }
        }

        public void ShowEvent( EventPresenter presenter, EventView eventView)
        {
            if (eventView.DataContext.Equals(presenter)) return;// EU error. AllEventsPresenter == EventPresenter never
            eventView.DataContext = presenter.AllEventsPresenter;
            eventView.Focus();
        }

        public void ShowAthlete(AthletePresenter presenter, AthleteView athleteView)
        {
            if (athleteView.DataContext.Equals(presenter)) return;
            athleteView.DataContext = presenter.AllAthletesPresenter;
            athleteView.Focus();
        }

        public void ShowCompetitors<T>(PresenterBase<T> presenter)
        {
            AthleteTab.DataContext = presenter;;
            //.tabAthleteView.   lsbxEventCompetitors.DataContext = presenter;
            //AthleteTab.Focus();
        }

        public void ShowCompetitor(CompetitorPresenter presenter, CompetitorView competitorView)
        {
            if (competitorView.DataContext.Equals(presenter)) return;
            competitorView.DataContext = presenter.AllCompetitorsPresenter;
            competitorView.Focus();
        }

        public void ShowRaceClass(RaceClassPresenter presenter, RaceClassView raceClassView)
        {
            if (raceClassView.DataContext.Equals(presenter)) return;// EU error. AllEventsPresenter == EventPresenter never
            raceClassView.DataContext = presenter.AllRaceClassesPresenter;
            raceClassView.Focus();
        }

        public void ShowSession(SessionPresenter presenter, SessionView sessionView)
        {
            if (sessionView.DataContext.Equals(presenter)) return;
            sessionView.DataContext = presenter.AllSessionsPresenter;
            sessionView.Focus();
        }

        public void RemoveTab<T>(PresenterBase<T> presenter, TabControl tabs)
        {
            for (int i = 0; i < tabs.Items.Count; i++)
            {
                TabItem item = (TabItem)tabs.Items[i];
                if (item.DataContext.Equals(presenter))
                {
                    tabs.Items.Remove(item);
                    break;
                }
            }
        }
        
        public void CloseView<T>(PresenterBase<T> presenter)
        {

        }
        
       /*
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Button button = e.OriginalSource as Button;
           // if (button != null)
           //     DisplayAllAthletes(new AllAthletesPresenter(this, button.  .DataContext.  .DataContext._hardcardContext), new AllAthletesView());
        }
        */
        private void ctlAllEvents_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void tabAthleteView_Loaded(object sender, RoutedEventArgs e)
        {

        }

        
       
    }
    

    

}
