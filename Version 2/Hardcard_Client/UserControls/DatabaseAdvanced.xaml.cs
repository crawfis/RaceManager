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

namespace RacingEventsTrackSystem
{
    /// <summary>
    /// Interaction logic for DatabaseAdvanced.xaml
    /// </summary>
    public partial class DatabaseAdvanced : Window
    {
        private DataBaseEvent _databaseEvent;
        private AllDatabasesManager _adbm = null;

        public DatabaseAdvanced(DataBaseEvent databaseEvent, AllDatabasesManager adbm)
        {
            InitializeComponent();
            _databaseEvent = databaseEvent;
            _adbm = adbm;
        }

        public DataBaseEvent databaseEvent
        {
            get { return _databaseEvent; }
            set
            {
                _databaseEvent = value;
            }
        }
        //
        // Backup database for current event into *.bak file 
        //
        private void btnBackup_Click(object sender, RoutedEventArgs e)
        {
            _adbm.BackupDatabase(_databaseEvent.RaceEvent.DatabaseName);
        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            // _adbm.RestoreDatabase(_databaseEvent.RaceEvent.DatabaseName);
            //_adbm.RestoreDBwithNewFiles(_databaseEvent.RaceEvent.DatabaseName, newDatabaseName);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            //_adbm.DeleteDatabase("'Hardcard'");
            _adbm.DeleteDatabase(_databaseEvent.RaceEvent.DatabaseName);
        }
    }
}
