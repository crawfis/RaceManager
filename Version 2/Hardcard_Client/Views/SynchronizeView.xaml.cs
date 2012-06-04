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

namespace RacingEventsTrackSystem.Views
{
    /// <summary>
    /// Interaction logic for SynchronizeView.xaml
    /// </summary>
    public partial class SynchronizeView : UserControl
    {
        private bool threadEnabled = false;

        public SynchronizeView()
        {
            InitializeComponent();
            DataContext = new SynchronizePresenter(this, new SynchronizeModel());
        }

        public SynchronizePresenter Presenter
        {
            get { return DataContext as SynchronizePresenter; }
        }

        private void Synchronize_Click(object sender, RoutedEventArgs e)
        {
            Presenter.Synchronize();
        }

        private void Synchronization_Service_Click(object sender, RoutedEventArgs e)
        {
            if (threadEnabled == false)
            {
                threadEnabled = true;
                CompetitorsCheckBox.IsEnabled = false;
                EventsCheckBox.IsEnabled = false;
                EventsComboBox.IsEnabled = false;
            }
            else
            {
                threadEnabled = false;
                CompetitorsCheckBox.IsEnabled = true;
                EventsCheckBox.IsEnabled = true;
                EventsComboBox.IsEnabled = true;
            }
            Presenter.Toggle_Synchronization_Service();
        }
    }
}
