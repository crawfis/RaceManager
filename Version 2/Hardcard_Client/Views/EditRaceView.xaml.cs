using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using RacingEventsTrackSystem.Presenters;

namespace RacingEventsTrackSystem.Views
{
    /// <summary>
    /// Interaction logic for EditRace.xaml
    /// </summary>
    public partial class RaceView : UserControl
    {
        public RaceView()
        {
            InitializeComponent();
        }

        public EditRacePresenter Presenter
        {
            get { return DataContext as EditRacePresenter; }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //Presenter.Save();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            //Presenter.Delete();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            //Presenter.Close();
        }
    }
}
