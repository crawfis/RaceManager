using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using RacingEventsTrackSystem.Presenters;

using System.Collections.ObjectModel;
using RacingEventsTrackSystem.Model;
using RacingEventsTrackSystem.Views;
using RacingEventsTrackSystem.DataAccess;
using Microsoft.Win32;
//using System.Windows.Forms;
//using System.Windows.MessageBox;


namespace RacingEventsTrackSystem.Views
{
    /// <summary>
    /// Interaction logic for EventView.xaml
    /// </summary>
    public partial class EventView : UserControl
    {
        public EventView()
        {
            InitializeComponent();
        }

        public AllEventsPresenter Presenter
        {
            get { return DataContext as AllEventsPresenter; }
        }

        /*
        // add race -- button deleted
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not implemented"); 
            
            ApplicationPresenter ap = Shell.theShell.DataContext as ApplicationPresenter;
            RaceManagement raceWin = new RaceManagement(ap.CurrentEvent);
            raceWin.Show();
            
        }
       */
        /*
        private void NewEvent_Click(object sender, RoutedEventArgs e)
        {
            Presenter.CreateNewEvent();
        }
        */
        private void SaveEvent_Click(object sender, RoutedEventArgs e)
        {
            Presenter.SaveEvent(Presenter.CurrentEvent);
        }
    }
}
