using System.Windows;
using System.Windows.Controls;
using RacingEventsTrackSystem;
using RacingEventsTrackSystem.Presenters;
using RacingEventsTrackSystem.DataAccess;
using Microsoft.Win32;

namespace RacingEventsTrackSystem.Views
{
    public partial class SessionView : UserControl
    {
        public SessionView()
        {
            InitializeComponent();
        }

        public AllSessionsPresenter Presenter
        {
            get { return DataContext as AllSessionsPresenter; }
        }

        private void SaveSession_Click(object sender, RoutedEventArgs e)
        {
            Presenter.SaveSession(Presenter.CurrentSessionForEvent);
        }

        private void NewSession_Click(object sender, RoutedEventArgs e)
        {
            Presenter.CreateNewSession();
        }

        private void ExcludeCompetitorFromSession(object sender, RoutedEventArgs e)
        {
            Presenter.ExcludeCurrentCompetitorFromSession(liviEntriesList.SelectedItem as Entry);
        }

        private void AddCompetitorToSession(object sender, RoutedEventArgs e)
        {
            Presenter.AddCompetitorToSession(Presenter.CurrentCompetitorForEventClass, Presenter.TmpEntry);
        }

        private void SaveChanges(object sender, RoutedEventArgs e)
        {

        }
    }
}
