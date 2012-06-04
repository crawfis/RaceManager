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

namespace RacingEventsTrackSystem.UserControls
{
    /// <summary>
    /// Interaction logic for AllCompetitorsForEventClass.xaml
    /// </summary>
    public partial class AllCompetitorsForEventClass : UserControl
    {
        public AllCompetitorsForEventClass()
        {
            InitializeComponent();
        }
        public AllSessionsPresenter Presenter
        {
            get { return DataContext as AllSessionsPresenter; }
        }
        private void AddCompetitorToSession(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
