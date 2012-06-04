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
    /// Interaction logic for AllAthletesControl.xaml
    /// </summary>
    /// 
    public partial class AllAthletesControl : UserControl
    {
        public AllAthletesControl()
        {
            InitializeComponent();
        }
        public AllAthletesPresenter Presenter
        {
            get { return DataContext as AllAthletesPresenter; }
        }

        private void AthleteList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Presenter.DisplayCurrentAthlete();
        }

        private void NewAthlete(object sender, RoutedEventArgs e)
        {
            Presenter.CreateNewAthlete();
        }

        private void DeleteAthlete(object sender, RoutedEventArgs e)
        {
            Presenter.DeleteAthlete(Presenter.CurrentAthlete);
        }
     }
}
