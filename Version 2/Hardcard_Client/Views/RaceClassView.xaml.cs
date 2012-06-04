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

using System.Collections.ObjectModel;
using RacingEventsTrackSystem.Model;
using RacingEventsTrackSystem.Views;
using RacingEventsTrackSystem.DataAccess;


namespace RacingEventsTrackSystem.Views
{
    /// <summary>
    /// Interaction logic for ClassView.xaml
    /// </summary>
    public partial class RaceClassView : UserControl
    {
        public RaceClassView()
        {
            InitializeComponent();
        }

        public AllRaceClassesPresenter Presenter
        {
            get { return (DataContext as AllRaceClassesPresenter); }
        }

        private void ExcludeClassFromEvent(object sender, RoutedEventArgs e)
        {
            Presenter.ExcludeClassFromEvent();
        }

        private void AddClassToEvent(object sender, RoutedEventArgs e)
        {
            Presenter.AddRaceClassToEvent(Presenter.CurrentRaceClass);
        }

        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            Presenter.SaveRaceClasses();
        }
    }
}
