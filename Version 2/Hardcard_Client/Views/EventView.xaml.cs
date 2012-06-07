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

        // add race -- button deleted
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not emplemented"); 
            /*
            ApplicationPresenter ap = Shell.theShell.DataContext as ApplicationPresenter;
            RaceManagement raceWin = new RaceManagement(ap.CurrentEvent);
            raceWin.Show();
            */
        }

        private void NewEvent_Click(object sender, RoutedEventArgs e)
        {
            Presenter.CreateNewEvent();
        }

        private void SaveEvent_Click(object sender, RoutedEventArgs e)
        {
            Presenter.SaveEvent(Presenter.CurrentEvent);
        }
        
        private void DeleteEventDataBase()
        {
            Process proc = null;
            try
            {
                string BatFileDir = string.Format(@"E:\user_June\OSU_RogerS\source\Database_Scripts\EventDataBaseScripts");//this is where mybatch.bat lies                 
                string BatFileName = "DeleteDBForEvent.bat";
                string strBatFile = BatFileDir + "\\" + BatFileName;
                if (!System.IO.File.Exists(strBatFile))
                {
                    MessageBox.Show("File " + strBatFile + " not found.");
                    return;
                }
                proc = new Process();
                proc.StartInfo.WorkingDirectory = BatFileDir;
                proc.StartInfo.FileName = BatFileName;
                string args = "sata-comp Hardcard E:\\user_June\\OSU_RogerS\\source\\Database_Scripts\\EventDataBaseScripts\\delete_event_records_for_SQL_EX.sql";
                proc.StartInfo.Arguments = string.Format(args);//this is argument                 
                proc.StartInfo.CreateNoWindow = false;
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
            }
        }

        private void LoadExistingEvent_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                string filePath = ofd.FileName;
                LoadEventDataBaseFromSql(filePath);
            }
        }

        private void BackupCurrentEvent_Click(object sender, RoutedEventArgs e)
        {
            Presenter.BackupCurrentEvent(); 
        }

        private void LoadEventDataBaseFromSql(string sqlFilePath)
        {
            Process proc = null;
            try
            {
                string BatFileDir = string.Format(@"E:\user_June\OSU_RogerS\source\Database_Scripts\EventDataBaseScripts");//this is where mybatch.bat lies                 
                string BatFileName = "LoadExistingDBForEvent.bat";
                string strBatFile = BatFileDir + "\\" + BatFileName;
                if (!System.IO.File.Exists(strBatFile))
                {
                    MessageBox.Show("File " + strBatFile + " not found.");
                    return;
                }
                proc = new Process();
                proc.StartInfo.WorkingDirectory = BatFileDir;
                proc.StartInfo.FileName = BatFileName;
                string args = "sata-comp Hardcard E:\\user_June\\OSU_RogerS\\source\\Database_Scripts\\EventDataBaseScripts\\create_event_records_for_SQL_EX.sql";
                proc.StartInfo.Arguments = string.Format(args);//this is argument                 
                proc.StartInfo.CreateNoWindow = false;
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
            }
        }


    }
}
