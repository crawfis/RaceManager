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
using RacingEventsTrackSystem.DataAccess;
using RacingEventsTrackSystem.UserControls;
using RacingEventsTrackSystem.Views;


namespace RacingEventsTrackSystem.Views
{
    /// <summary>
    /// Interaction logic for TimingView.xaml
    /// </summary>
    public partial class TimingView : UserControl
    {
        public TimingView()
        {
            InitializeComponent();
        }
        public AllSessionsPresenter Presenter
        {
            get { return DataContext as AllSessionsPresenter; }
        }

        private void AddPassing_Click(object sender, RoutedEventArgs e)
        {
            Presenter.UpdateStandingTable(Presenter.CurrentSessionForEvent);
        }

        private void RemovePassing_Click(object sender, RoutedEventArgs e)
        {
            Presenter.RemovePassingRow(Presenter.CurrentSessionForEvent);
        }

        private void EditPassing_Click(object sender, RoutedEventArgs e)
        {
            //Presenter.EditCurrentPassing(Presenter.CurrentSessionForEvent);
            //PassingEditControl passingEditControl = new PassingEditControl(Presenter.CurrentPassingForEvent);
            //PassingEditControl passingEditControl = new PassingEditControl();
            //viewPassingEdit.DataSource = Presenter.CurrentPassingForSession;
            //PassingEditForm passingEditForm = new PassingEditForm();
            //passingEditForm.ShowDialog();
            PassingEditForm passingEditForm = new PassingEditForm();
            passingEditForm.viewPassingEdit.DataContext = Presenter.CurrentPassingForSession;
            //passingEditForm.viewPassingEdit.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            //passingEditForm.viewPassingEdit.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            passingEditForm.ShowDialog();
            Presenter.ReCalculatePassingsAndStandings(Presenter.CurrentSessionForEvent);
            //viewPassingEdit.Focus();

        }
        private void RefreshPassingAndStanding(object sender, RoutedEventArgs e)
        {
            Presenter.CurrentSessionForEvent.MinLapTime = Int32.Parse(txbxMinLapTime.Text);
            Presenter.ReCalculatePassingsAndStandings(Presenter.CurrentSessionForEvent);
        }

        private void InsertPassing_Click(object sender, RoutedEventArgs e)
        {
            Presenter.InsertPassingRow(Presenter.CurrentSessionForEvent);
        }
    }
}
