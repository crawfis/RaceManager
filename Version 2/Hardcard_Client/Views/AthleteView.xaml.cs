using System.Windows;
using System.Windows.Controls;
using RacingEventsTrackSystem;
using RacingEventsTrackSystem.Presenters;
using Microsoft.Win32;

namespace RacingEventsTrackSystem.Views
{
    public partial class AthleteView : UserControl
    {
        public AthleteView()
        {
            InitializeComponent();
        }

        public AllAthletesPresenter Presenter
        {
            get { return DataContext as AllAthletesPresenter; }
        }

        private void SaveAthlete_Click(object sender, RoutedEventArgs e)
        {
            Presenter.SaveAthlete(Presenter.CurrentAthlete);
        }
    }
}
