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

namespace RacingEventsTrackSystem.UserControls
{
    /// <summary>
    /// Interaction logic for SideBar.xaml
    /// </summary>
    public partial class AllCompetitorsControl : UserControl
    {
        public AllCompetitorsControl()
        {
            InitializeComponent();
        }

        public AllCompetitorsPresenter Presenter
        {
            get { return DataContext as AllCompetitorsPresenter; }
        }

        private void CompetitorsList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //Presenter.DisplayCurrentCompetitor();
        }

        private void ExcludeCompetitor(object sender, RoutedEventArgs e)
        {
            Presenter.DeleteCompetitor(Presenter.CurrentCompetitorForEvent);
        }
    }
}
