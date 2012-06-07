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

namespace RacingEventsTrackSystem.Views
{
    /// <summary>
    /// Interaction logic for RaceView.xaml
    /// </summary>
    public partial class RaceView : UserControl
    {
        public RaceView()
        {
            try
            {
                InitializeComponent();
                //DataContext = new RacePresenter(this);
                //DataContext = new RacePresenter(this);
            }
            catch (Exception ex) 
            {
                System.Console.WriteLine("RaceView constructor failed with error: " + ex.Message);
            };
        }

        public RacePresenter Presenter
        {
            get { return DataContext as RacePresenter; }
        }
/*
        private void Event_Selected(object sender, RoutedEventArgs e)
        {
            Presenter.EventSelected();
        }
*/
        private void Session_Selected(object sender, RoutedEventArgs e)
        {
            Presenter.SessionSelected();
        }

        private void Startrace_Click(object sender, RoutedEventArgs e)
        {
            // TODO disable the Start Button
            // TODO: Enable the Stop Buttom
            Presenter.StartRace();
        }

        private void Stoprace_Click(object sender, RoutedEventArgs e)
        {
            Presenter.StopRace();
            // TODO ensable the Start Button
            // TODO: disable the Stop Buttom
            this.eventComboBox.IsEnabled = true;
            this.sessionComboBox.IsEnabled = true;
        }

        private void AddPassing_Click(object sender, RoutedEventArgs e)
        {
            Presenter.AddNewPassing();
        }
    }
}
