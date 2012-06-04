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
    /// Interaction logic for AllSessions.xaml
    /// </summary>
    public partial class AllSessions : UserControl
    {
        public AllSessions()
        {
            InitializeComponent();
        }

        public AllSessionsPresenter Presenter
        {
            get { return DataContext as AllSessionsPresenter; }
        }

        private void ExcludeSession(object sender, RoutedEventArgs e)
        {
            Presenter.ExcludeSessionFromEvent(Presenter.CurrentSessionForEvent);
        }
    }
}
