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
using RacingEventsTrackSystem.DataAccess;


namespace RacingEventsTrackSystem.Views
{
    /// <summary>
    /// Interaction logic for StandingView.xaml
    /// </summary>
    public partial class StandingView : UserControl
    {
        public StandingView()
        {
            InitializeComponent();
        }
        public AllSessionsPresenter Presenter
        {
            get { return DataContext as AllSessionsPresenter; }
        }

        private void UpdateStanding_Click(object sender, RoutedEventArgs e)
        {
            Presenter.UpdateStandingTable(Presenter.CurrentSessionForEvent);
        }

        private void WriteStandingReport_Click(object sender, RoutedEventArgs e)
        {
            Presenter.WriteStandingReport(Presenter.CurrentSessionForEvent); 
        }

        private void CheckAll_Click(object sender, RoutedEventArgs e)
        {
            Presenter.AllCompetitorsInTiming(Presenter.CurrentSessionForEvent); 
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            Presenter.NoCompetitorsInTiming(Presenter.CurrentSessionForEvent); 
        }
        private void DisplayCompetitorInTiming_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chbx = e.OriginalSource as CheckBox;
            if (chbx != null)
            {
                 Presenter.FilterCompetitorsInTiming(chbx.DataContext as Standing, (bool)chbx.IsChecked);
            }
            
        }
    }
}
