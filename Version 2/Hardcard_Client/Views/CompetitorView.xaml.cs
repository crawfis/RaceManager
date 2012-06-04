using System.Windows;
using System.Windows.Controls;
using RacingEventsTrackSystem;
using RacingEventsTrackSystem.Presenters;
using Microsoft.Win32;
using RacingEventsTrackSystem.DataAccess;
using System;

namespace RacingEventsTrackSystem.Views
{
    public partial class CompetitorView : UserControl
    {
        public CompetitorView()
        {
            InitializeComponent();
        }

        public AllCompetitorsPresenter Presenter
        {
            get { return DataContext as AllCompetitorsPresenter; }
        }

        private void SaveCompetitor_Click(object sender, RoutedEventArgs e)
        {
            EventClass eventClass = new EventClass();
            eventClass = (cmbxEventClass.SelectedItem as EventClass);

            string status = (cmbxCompetitorStatus.SelectedItem != null) ? cmbxCompetitorStatus.SelectedItem.ToString() : "";
            string vehicleType = string.Copy(txbxVehicleType.Text);
            string vehicleModel = string.Copy(txbxVehicleModel.Text);
            int vehicleCC = 0;
            int.TryParse(txbxVehicleCC.Text, out vehicleCC);

            Presenter.AddAtleteToCompetitorsList(Presenter.ApplicationPresenter.AllAthletesPresenter.CurrentAthlete,
                eventClass,
                status,
                vehicleType,
                vehicleModel,
                vehicleCC);  
        }
    }
}
