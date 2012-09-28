using System.Windows.Controls;
using RacingEventsTrackSystem.Presenters;
using System.Collections.Generic;
using System;
using System.Windows.Media;
namespace RacingEventsTrackSystem.UserControls
{
    public partial class EventAndSessionSearchBar : UserControl
    {
        static public bool RaceStarted = false;
        public EventAndSessionSearchBar()
        {
            InitializeComponent();
            btnStartStopRace.Foreground = Brushes.Black;
            btnStartStopRace.Background = Brushes.Azure;
            btnStartStopRace.Content = "Start Tag Readings";
        }
        public ApplicationPresenter Presenter
        {
            get { return DataContext as ApplicationPresenter; }
        }

        private void StartTagReadings_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (RaceStarted == false)
            {
                RaceStarted = true;
                btnStartStopRace.Foreground = Brushes.Black;
                btnStartStopRace.Background = Brushes.Salmon;
                btnStartStopRace.Content = "Stop Tag Readings";
                //Presenter.RacePresenter.StartRace();
                Presenter.RacePresenter.StartTestRace();

            }
            else
            {
                RaceStarted = false;
                btnStartStopRace.Foreground = Brushes.Black;
                btnStartStopRace.Background = Brushes.Azure;
                btnStartStopRace.Content = "Start Tag Readings";
            }
        }
    }
}