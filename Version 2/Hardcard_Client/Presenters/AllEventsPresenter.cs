using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using RacingEventsTrackSystem.Model;
using RacingEventsTrackSystem.Views;
using RacingEventsTrackSystem.DataAccess;
using RacingEventsTrackSystem.Presenters;
using System.Windows;
using System.Data.Objects;

using System.Data;
using System.Data.SqlClient;
using System.Data.Sql;
using Microsoft.Win32;
using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using Microsoft.SqlServer.Management.Common;
using System.Windows.Shapes;


namespace RacingEventsTrackSystem.Presenters
{
    public partial class AllEventsPresenter : PresenterBase<Shell>
    {
        private readonly ApplicationPresenter _applicationPresenter;
        // Keeps Events existing in DataBase
        private ObservableCollection<Event> _allEvents;
        private Event _currentEvent;

        // Keeps EventClasses for the CurrentEvent, any control changing CurrentEvent, has to reset that collection.
        private ObservableCollection<EventClass> _allEventClasses;
        // EventClass selected in _allEventClasses list.
        private EventClass _currentEventClass;                    
        private string _statusText;                              


        public AllEventsPresenter(ApplicationPresenter applicationPresenter, 
                                  Shell view
                                  ) : base(view)
        {
            try 
            {
                var hc = applicationPresenter.HardcardContext;
                _applicationPresenter = applicationPresenter;
                _allEventClasses = new ObservableCollection<EventClass>();
                _allEvents = new ObservableCollection<Event>(hc.Events);
                if (_allEvents.Count() > 0 ) 
                { 
                    _currentEvent =_allEvents.First();
                    _allEventClasses = new ObservableCollection<EventClass>(_currentEvent.EventClasses);
                    if (_allEventClasses.Count() > 0)
                    {
                        _currentEventClass = _allEventClasses.First();
                    }
                 }
                SetEventDependents(_currentEvent);
                StatusText = ("AllEventsPresenter constructor no error");
            } 
            catch (Exception ex)
            {
                StatusText = "AllEventsPresenter constructor failed with error: " + ex.Message;
                MessageBox.Show(StatusText); // stop executable
            }
        }

        public ApplicationPresenter ApplicationPresenter
        {
            get { return _applicationPresenter; }
            set {}
        }

        public ObservableCollection<Event> AllEvents
        {
            get { return _allEvents; }
            set
            {
                _allEvents = value;
                OnPropertyChanged("AllEvents");
            }
        }

        public Event CurrentEvent
        {
            get { return _currentEvent; }
            set
            {
                _currentEvent = value;
                SetEventDependents(_currentEvent);
                OnPropertyChanged("CurrentEvent");
            }
        }
        public ObservableCollection<EventClass> AllEventClasses
        {
            get { return _allEventClasses; }
            set
            {
                _allEventClasses = value;
                OnPropertyChanged("AllEventClasses");
            }
        }

        public EventClass CurrentEventClass
        {
            get { return _currentEventClass; }
            set
            {
                _currentEventClass = value;
                OnPropertyChanged("CurrentEventClass");
            }
        }

        public string StatusText
        {
            get { return _statusText; }
            set
            {
                _statusText = value;
                OnPropertyChanged("StatusText");
            }
        }

        public void Search(string criteria)
        {
            if (!string.IsNullOrEmpty(criteria) && criteria.Length > 0)
            {
                AllEvents = new ObservableCollection<Event>(FindByLookup(criteria));
                StatusText = string.Format("{0} events found.", AllEvents.Count);
            }
            else
            {
                AllEvents = new ObservableCollection<Event>(_applicationPresenter.HardcardContext.Events);
                StatusText = "Displaying all events.";
            }
        }

        //
        // Search for substring in EventName or EventLocation
        //
        public List<Event> FindByLookup(string name)
        {
            IEnumerable<Event> found =
                from c in _applicationPresenter.HardcardContext.Events
                where (c.EventName.ToLower().Contains(name.ToLower())
                    || c.EventLocation.ToLower().Contains(name.ToLower()))
                select c;
            return found.ToList();
        }

        //
        // Create new event. It doesn't add the event to collection or entity. 
        // 
        public void CreateNewEvent()
        {
            DatabaseParam dbParam = new DatabaseParam();
            //CreateDatabase(dbParam);
            RestoreEventDataBase();

            _applicationPresenter.HardcardContext.SaveChanges();
            Event newEvent = new Event();
            newEvent.EventName = "Unknown";
            newEvent.EventLocation = "USA";
            SaveEvent(newEvent);
            CurrentEvent = newEvent;
            OpenEvent(newEvent);
        }
        
        //
        // Update existing Event or add new entry if Event is not in DataContext.Events
        //
        // If this Event is in Event table then it will be updated in DataContext, otherwise it will be added to 
        // the DataContext.
        // Same for Events Collection.
        // The method submits DataContext in the DataBase.
        //

        public void SaveEvent(Event myEvent)
        {
            var hc = _applicationPresenter.HardcardContext;
            if (myEvent == null) return;
            //hc.SaveChanges();
            Event dbEvent = null;
            // If myEvent is in DataContext.Events then just update it
            if (IsInEvent(myEvent))
            {
                //update DataContext.RaceClasses
                dbEvent = hc.Events.Single(e => e.Id == myEvent.Id);
                hc.ApplyCurrentValues(dbEvent.EntityKey.EntitySetName, myEvent);
                StatusText = string.Format("Event '{0}' was updated.", myEvent.ToString());
            }
            else
            {
                long max_id = 0;
                if ((from c in hc.Events select c).Count() > 0) 
                    max_id = (from e in hc.Events select e.Id).Max();
                myEvent.Id = ++max_id;
                hc.Events.AddObject(myEvent);
            }
            
            int i = AllEvents.IndexOf(myEvent);
            if (i >= 0)
            {
                //Update in Collection
                AllEvents.RemoveAt(i);
                //dbEvent = hc.Events.Single(e => e.Id == myEvent.Id);//
                //AllEvents.Insert(i, dbEvent);
                AllEvents.Insert(i, myEvent);
                CurrentEvent = myEvent;
            }
            else
            {
                AllEvents.Add(myEvent);
            }

            hc.SaveChanges();
            OpenEvent(myEvent);
            StatusText = string.Format( "Event '{0}' was saved.", myEvent.EventName);
        }


        // 
        // Delete myEvent DataContext.Events and AllEvents Collection. Don't reset Current Event.
        //
        public void DeleteEvent(Event myEvent)
        {
            if (myEvent == null) return;
            var hc = _applicationPresenter.HardcardContext;
            hc.SaveChanges();
            // Delete from DataContext
            if (IsInEvent(myEvent))
            {
                // check if there is reference to this Athlete in Competitor table
                if (IsEventInEventClass(myEvent))
                {
                    string str = string.Format("Remove record from EventClass first for Event.Id = {0}",
                           myEvent.Id);
                    MessageBox.Show(str);
                    return;
                }
                else
                {
                    //delete from DataContext
                    hc.Events.DeleteObject(hc.Events.Single(e => e.Id == myEvent.Id));
                    StatusText = string.Format("Athlete '{0}' was deleted.", myEvent.ToString());
                }

            }

            // Delete from Collection            
            if (AllEvents.Contains(myEvent))
            {
                AllEvents.Remove(myEvent);
                OpenEvent(new Event());
            }
            hc.SaveChanges();
            StatusText = string.Format("Event '{0}' was deleted.", myEvent.EventName);
        }

        //
        // Returns true if myEvent.Id exists in the Event table
        //
        private bool IsInEvent(Event myEvent)
        {
            var hc = _applicationPresenter.HardcardContext;
            return (hc.Events.Count(s => s.Id == myEvent.Id) == 0) ? false : true;
        }

        //
        // Returns true if EventClass table has FK for input myEvent
        //
        private bool IsEventInEventClass(Event myEvent)
        {
            var hc = _applicationPresenter.HardcardContext;
            return (hc.EventClasses.Count(ec => ec.EventId == myEvent.Id) == 0) ? false : true;
        }

        public void OpenEvent(Event myEvent)
        {
            if (myEvent == null) return;
            View.ShowEvent( new EventPresenter( this, new EventView(), myEvent),
                            View.eventView);
        }

        public void DisplayCurrentEvent()
        {
            if(_currentEvent != null)
                OpenEvent(_currentEvent);
        }

        
        public void  SetEventDependents(Event myEvent)
        {
            // Set Competitors, EventClasses, and Sessions for CurrentEvent
            // Actually it should reset Sessions only when Session view is called??!!!!!!!!!!!!!
            if (myEvent == null) return;

            var allCompetitorsPresenter = _applicationPresenter.AllCompetitorsPresenter;
            if (allCompetitorsPresenter == null) return;
            allCompetitorsPresenter.AllCompetitors = allCompetitorsPresenter.InitCompetitorsForEvent(myEvent);

            AllEventClasses = InitAllEventClasses(myEvent);

            // Set Sessions for myEvent
            var sessionsPresenter = _applicationPresenter.AllSessionsPresenter;
            if (sessionsPresenter == null) return;
            sessionsPresenter.SessionsForEvent = sessionsPresenter.InitSessionsForEvent(myEvent);
        }

        // 
        // Create EventClasses collection for Current Event
        //
        public ObservableCollection<EventClass> InitAllEventClasses(Event myEvent)
        {
            if (myEvent == null)
            {
                CurrentEventClass = null;
                return new ObservableCollection<EventClass>(); 
            }
            else if (myEvent.EventClasses.Count() == 0)
            {
                CurrentEventClass = null;
                return new ObservableCollection<EventClass>(); 
            }
            CurrentEventClass = myEvent.EventClasses.First();
            return new ObservableCollection<EventClass>(myEvent.EventClasses.ToList());
        }

        public class DatabaseParam
        {
            //public string ServerName = "servername";
            //public string ServerName = "(local)";
            public string ServerName = string.Format(@"SATA-COMP\sqlexpress");
            public string DatabaseName = "Hardcard10";
            public string DataFileName = "DataFileName";
            public string DataPathName = "DataPathName";
            public string DataFileGrowth = "DataFileGrowth";
            public string LogFileName = "LogFileName";
            public string LogPathName = "LogPathName";
            public string LogFileGrowth = "LogFileGrowth";
        }

        private string getServerName()
        {
            string sqlSErverInstance;
            List<string> lstLocalInstances = new List<string>();
            RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server");
            String[] instances = (String[])rk.GetValue("InstalledInstances");
            if (instances.Length > 0)
            {
                foreach (String element in instances)
                {
                    if (element == "SQLEXPRESS")
                        lstLocalInstances.Add(System.Environment.MachineName);
                    else
                        lstLocalInstances.Add(System.Environment.MachineName + @"\" + element);
                }
            }
            return sqlSErverInstance = lstLocalInstances[0].ToString();
        }
        
        private void CreateDatabase(DatabaseParam DBParam)
        {

            string currentCommectionString = RacingEventsTrackSystem.Properties.Settings.Default.HardcardConnectionString;
            SqlConnection currConn = new SqlConnection(currentCommectionString);
            if (currConn.State == ConnectionState.Open)
                currConn.Close();
            
            //System.Data.SqlClient.SqlConnection tmpConn;
            string sqlCreateDBQuery;
            SqlConnection tmpConn;

            /*
             tmpConn = new SqlConnection("Server=localhost;Integrated security=SSPI;database=master");

             sqlCreateDBQuery = "CREATE DATABASE Hardcard11 ON PRIMARY ";
            
            +
                 "(NAME = MyDatabase_Data, " +
                 "FILENAME = 'C:\\MyDatabaseData.mdf', " +
                 "SIZE = 2MB, MAXSIZE = 10MB, FILEGROWTH = 10%) " +
                 "LOG ON (NAME = MyDatabase_Log, " +
                 "FILENAME = 'C:\\MyDatabaseLog.ldf', " +
                 "SIZE = 1MB, " +
                 "MAXSIZE = 5MB, " +
                 "FILEGROWTH = 10%)";
             */
              

            
            
 
            tmpConn = new SqlConnection();
            string serverName = getServerName();
            tmpConn.ConnectionString = string.Format(@"SERVER = SATA-COMP\sqlexpress; DATABASE = master;Integrated security=SSPI");
            //            tmpConn.ConnectionString = "SERVER = " + DBParam.ServerName +
            //"; DATABASE = master; User ID =; Pwd =;";
  //          "; DATABASE = master; User ID = sa; Pwd = sa";
            
            sqlCreateDBQuery = "CREATE DATABASE Hardcard12";
          
            /*
            sqlCreateDBQuery = " CREATE DATABASE "
                               + DBParam.DatabaseName
                               + " ON PRIMARY ";
            
                               + " (NAME = " + DBParam.DataFileName + ", "
                               + " FILENAME = '" + DBParam.DataPathName + "', "
                               + " SIZE = 2MB,"
                               + " FILEGROWTH =" + DBParam.DataFileGrowth + ") "
                               + " LOG ON (NAME =" + DBParam.LogFileName + ", "
                               + " FILENAME = '" + DBParam.LogPathName + "', "
                               + " SIZE = 1MB, "
                               + " FILEGROWTH =" + DBParam.LogFileGrowth + ") ";
            */
            SqlCommand myCommand = new SqlCommand(sqlCreateDBQuery, tmpConn);
            try
            {
                tmpConn.Open();
                MessageBox.Show(sqlCreateDBQuery);
                myCommand.ExecuteNonQuery();
                System.Windows.Forms.MessageBox.Show("Database has been created successfully!",
                                  "Create Database", System.Windows.Forms.MessageBoxButtons.OK,
                                              System.Windows.Forms.MessageBoxIcon.Information);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "Create Database",
                                            System.Windows.Forms.MessageBoxButtons.OK,
                                     System.Windows.Forms.MessageBoxIcon.Information);
            }
            finally
            {
                tmpConn.Close();
            }
            return;
        }

        public void BackupCurrentEvent()
        {

            System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox.Show(
                           "Want to Backup all data for current event?",
                           "Exit",
                           System.Windows.Forms.MessageBoxButtons.YesNo);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                System.Windows.Forms.MessageBox.Show("Copying DataBase to file");
                BackupEventDataBase();
                //DisplayAllDBConnections(); //just for this function test 
            }
            else
            {
                MessageBox.Show("Contine program");
            }
        }

        public void BackupEventDataBase()
        {
            //string currentCommectionString = RacingEventsTrackSystem.Properties.Settings.Default.HardcardConnectionString;
            //SqlConnection currConn = new SqlConnection(currentCommectionString);

            //SqlConnection currConn = new SqlConnection();
            //if (currConn.State == ConnectionState.Open)
            //    currConn.Close();

            //System.Data.SqlClient.SqlConnection tmpConn;
            
            //
            // Uses sqlCreateDBQuery
            // it works for Memory stick 
            //
            /*
            string sqlCreateDBQuery;
            SqlConnection tmpConn;

            tmpConn = new SqlConnection();
            string serverName = getServerName();
            tmpConn.ConnectionString = string.Format(@"SERVER = SATA-COMP\sqlexpress; DATABASE = Hardcard;Integrated security=SSPI");

            sqlCreateDBQuery = "BACKUP DATABASE Hardcard TO DISK = \'H:\\Hardcard.bak\' WITH FORMAT";

            SqlCommand myCommand = new SqlCommand(sqlCreateDBQuery, tmpConn);
            try
            {
                if (tmpConn.State != ConnectionState.Open)
                    tmpConn.Open();
                MessageBox.Show(sqlCreateDBQuery);
                myCommand.ExecuteNonQuery();
                System.Windows.Forms.MessageBox.Show("Database has been written successfully!",
                                  "Backup Database", System.Windows.Forms.MessageBoxButtons.OK,
                                              System.Windows.Forms.MessageBoxIcon.Information);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "Backup Database",
                                            System.Windows.Forms.MessageBoxButtons.OK,
                                     System.Windows.Forms.MessageBoxIcon.Information);
            }
            finally
            {
                tmpConn.Close();
            }
            return;
            */


            //
            // Uses SMO object 
            // it works for Memory stick 
            //
            //Connect to the local, default instance of SQL Server. 

            //Microsoft.SqlServer.Management.Smo.Server smoServer = new Server(new ServerConnection(server)); 
            string srvName = "SATA-COMP\\sqlexpress";
            Server smoServer = new Server(new ServerConnection(srvName));  
            //Server smoServer = new Server();  
            string DbFileName = CurrentEvent.EventName;

              Backup bkp = new Backup();

                   //this.Cursor = Cursors.WaitCursor;
                   //this.dataGridView1.DataSource = string.Empty;
                   try
                   {
                       string fileName = "H:\\Hardcard_tmp.bak";
                       string databaseName = "Hardcard";

                       bkp.Action = BackupActionType.Database;
                       bkp.Database = databaseName;
                       bkp.Devices.AddDevice(fileName, DeviceType.File);
                       //bkp.Incremental = chkIncremental.Checked; ???
                       //this.progressBar1.Value = 0;
                       //this.progressBar1.Maximum = 100;
                       //this.progressBar1.Value = 10;

                       bkp.PercentCompleteNotification = 10;
                       //bkp.PercentComplete += new PercentCompleteEventHandler(ProgressEventHandler);

                       bkp.SqlBackup(smoServer);
                       MessageBox.Show("Database Backed Up To: " + fileName, "SMO Demos");
                   }
            
                   catch (Exception ex)
                   {
                       MessageBox.Show(ex.ToString());
                   }
                   finally
                   {
                       //this.Cursor = Cursors.Default;
                       //this.progressBar1.Value = 0;
                   }
        }
         /*
        public void ProgressEventHandler(object sender, PercentCompleteEventArgs e)
        {
            this.progressBar1.Value = e.Percent;
        }
         */

        //
        // displays all database connections to the instance of SQL Server.
        //
        public void DisplayAllDBConnections()
        {   // doesn't work
            Server srv = default(Server);
            string srvName = "SATA-COMP\\sqlexpress";
            srv = new Server("srvName");
            int count = 0; 
            int total = 0; 
            string str = "";
            //Iterate through the databases and call the GetActiveDBConnectionCount method. 
            Database db = new Database();
            //db = default(Database);
            foreach (Database db1 in srv.Databases) // Exception:can not connect to server
            { 
              count = srv.GetActiveDBConnectionCount(db1.Name); 
              total = total + count; 
              //Display the number of connections for each database. 
              str += string.Format("\n{0} connections on {1} ",count, db1.Name); 
            } 
            //Display the total number of connections on the instance of SQL Server. 
               str += string.Format("\nTotal connections = {0} ",total); 
             System.Windows.Forms.MessageBox.Show(str);
        } 

        public void RestoreEventDataBase()
        {
            //string currentCommectionString = RacingEventsTrackSystem.Properties.Settings.Default.HardcardConnectionString;
            //SqlConnection currConn = new SqlConnection(currentCommectionString);

            //SqlConnection currConn = new SqlConnection();
            //if (currConn.State == ConnectionState.Open)
            //    currConn.Close();

            //System.Data.SqlClient.SqlConnection tmpConn;

            //string serverName = getServerName();
            string serverName = "SATA-COMP\\sqlexpress";

            //
            // Uses sqlCreateDBQuery
            // it doesn't work
            //
            
            string currentCommectionString = RacingEventsTrackSystem.Properties.Settings.Default.HardcardConnectionString;
            SqlConnection currConn = new SqlConnection(currentCommectionString);
            if (currConn.State == ConnectionState.Open)
                currConn.Close();
            
                      string sqlCreateDBQuery;
                      SqlConnection tmpConn;

                      tmpConn = new SqlConnection();
            
                      tmpConn.ConnectionString = string.Format(@"SERVER = SATA-COMP\sqlexpress; DATABASE = master;Integrated security=SSPI");

                      sqlCreateDBQuery = "USE master RESTORE DATABASE Hardcard FROM DISK = 'E:\\Hardcard.bak'"
                                         + "WITH FILE = 1, RECOVERY,  NOUNLOAD,  REPLACE, STATS = 10,"
                                         + "MOVE 'C:\\Program Files\\Microsoft SQL Server\\MSSQL10.SQLEXPRESS\\MSSQL\\data\\Hardcard.mdf' to 'E:\\Hardcard_bk.mdf',"
                                         + "MOVE 'C:\\Program Files\\Microsoft SQL Server\\MSSQL10.SQLEXPRESS\\MSSQL\\data\\Hardcard.ldf' to 'E:\\Hardcard_bk.ldf'";
                      // RESTORE FILELISTONLY give you what logical files exist for a particular backup

            //  RESTORE DATABASE [NewCopy_YourDatabase] 
            // FROM  DISK ='\\Backup\YourDatabase\YourDatabase_FULL_20120318.bak' WITH  FILE = 1,  
            // RECOVERY,  NOUNLOAD,  REPLACE, STATS = 10 , 
            // move 'YourDatabase_Data' to 'D:\Data\NewCopy_YourDatabase_Data.MDF' , 
            // move 'YourDatabase_Log' to 'F:\Logs\NewCopy_YourDatabase_Log.LDF'
             
                      SqlCommand myCommand = new SqlCommand(sqlCreateDBQuery, tmpConn);
                      try
                      {
                          if (tmpConn.State != ConnectionState.Open)
                              tmpConn.Open();
                          MessageBox.Show(sqlCreateDBQuery);
                          myCommand.ExecuteNonQuery();
                          System.Windows.Forms.MessageBox.Show("Database has been restored successfully!",
                                            "Restore Database", System.Windows.Forms.MessageBoxButtons.OK,
                                                        System.Windows.Forms.MessageBoxIcon.Information);
                      }
                      catch (System.Exception ex)
                      {
                          System.Windows.Forms.MessageBox.Show(ex.ToString(), "Restore Database",
                                                      System.Windows.Forms.MessageBoxButtons.OK,
                                               System.Windows.Forms.MessageBoxIcon.Information);
                      }
                      finally
                      {
                          tmpConn.Close();
                      }
                      return;
              

            //
            // Uses SMO Object
            // it doesn't work ??
            //
            
            
            // tmpConn = new SqlConnection("Server=localhost;Integrated security=SSPI;database=master");

            // sqlCreateDBQuery = "CREATE DATABASE Hardcard11 ON PRIMARY ";
            /*
            Server smoServer = new Server(new ServerConnection(serverName));
            //Server smoServer = new Server();  
            //string DbFileName = CurrentEvent.EventName;
            string databaseName = "'Hardcard12'";
            Database db = smoServer.Databases[databaseName]; 
            //string dbPath = Path.Combine(db.PrimaryFilePath, 'MyDataBase.mdf'); string logPath = Path.Combine(db.PrimaryFilePath, 'MyDataBase_Log.ldf'); 
            Restore restore = new Restore();
            try
            {
                string fileName = "E:\\Hardcard_tmp.bak";

                restore.NoRecovery = true;
                restore.Database = databaseName;
                restore.Action = RestoreActionType.Database;
                restore.ReplaceDatabase = true;




                //Add the device that contains the full database backup to the Restore object. 
                BackupDeviceItem bdi = default(BackupDeviceItem);
                bdi = new BackupDeviceItem(fileName, DeviceType.File);
                restore.Devices.Add(bdi);
                //restore.Devices.AddDevice(fileName, DeviceType.File);

                restore.PercentCompleteNotification = 10;
                //bkp.PercentComplete += new PercentCompleteEventHandler(ProgressEventHandler);

                restore.SqlRestore(smoServer);
                // Db or file should be offline and then turn to online
                //db = smoServer.Databases['MyDataBase']; 
                //db.SetOnline(); 
                //smoServer.Refresh(); 
                //db.Refresh(); 
                MessageBox.Show("Database Restored From: " + fileName, "SMO Demos");
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                //this.Cursor = Cursors.Default;
                //this.progressBar1.Value = 0;
            }
            */

        }


    }
}
