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
    /// Interaction logic for RaceClassSideBar.xaml
    /// </summary>
    public partial class AllRaceClassesControl : UserControl
    {
        public AllRaceClassesControl()
        {
            InitializeComponent();
        }

       // public EventPresenter Event
        public AllRaceClassesPresenter Presenter
        {
            get { return (DataContext as AllRaceClassesPresenter); }
        }

        private void AllRaceClassesList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Presenter.DisplayCurrentRaceClass();
        }

        private void NewRaceClass(object sender, RoutedEventArgs e)
        {
            Presenter.CreateNewRaceClass();
        }

        private void DeleteRaceClass(object sender, RoutedEventArgs e)
        {
            Presenter.DeleteRaceClass(Presenter.CurrentRaceClass);
        }
        


    }
}
