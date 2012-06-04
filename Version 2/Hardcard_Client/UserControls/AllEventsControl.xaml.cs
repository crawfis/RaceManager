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
    /// Interaction logic for EventSideBar.xaml
    /// </summary>
    public partial class AllEventsControl : UserControl
    {
        public AllEventsControl()
        {
            InitializeComponent();
        }

       // public EventPresenter Event
        private AllEventsPresenter Presenter
        {
            get { return DataContext as AllEventsPresenter; }
        }

        private void EventsList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Presenter.DisplayCurrentEvent();
        }

        private void DeleteEvent(object sender, RoutedEventArgs e)
        {
            Presenter.DeleteEvent(Presenter.CurrentEvent);
        }

    }
}
