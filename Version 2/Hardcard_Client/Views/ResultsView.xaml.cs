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
    /// Interaction logic for ResultsView.xaml
    /// </summary>
    public partial class ResultsView : UserControl
    {
        public ResultsView()
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
    }
}
