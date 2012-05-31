using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using HardCard.Scoring;

namespace SampleGUI
{
    // TODO; This is just a quick place holder. The newer technology and LINQ may make this much easier and
    // more flexible.
    public class LoggerSql : ITagEventSubscriber
    {
        public delegate void LogDelegate(TagReadEventArgs e);

        public LoggerSql(SqlConnection connection, LogDelegate logCommand)
        {
            if (connection == null)
                throw new ArgumentNullException("The SQL connection was null when passed into LoggerSql");
            if (logCommand == null)
                throw new ArgumentNullException("No LogDelegate was specified on the constructor to LoggerSql");

            this.connection = connection;
            this.logCommand = logCommand;
            this.Count = 0;
        }

        public int Count { get; private set; }

        public void AddPublisher(ITagEventPublisher rfidReader)
        {
            rfidReaders.Add(rfidReader);
            rfidReader.TagDetected += new TagEventHandler(LogTag);
        }

        internal void LogTag(object sender, TagReadEventArgs tagInfo)
        {
            NetworkListener network = sender as NetworkListener;
            if (network != null)
                logCommand(tagInfo);
        }

        //private void Log(TagReadEventArgs e)
        //{
        //    TagInfo tagInfo = e.TagInfo;
        //    // Increment Count here, that way we can see failures on adding to the database.
        //    Count++;
        //    //connection.Open();
        //    try
        //    {
        //        dataTable.AddTagReadingsRow(tagInfo.ID.Value, tagInfo.Time, tagInfo.SignalStrenth, (short)tagInfo.Antenna, tagInfo.Frequency);
        //        using (SqlCommand command = new SqlCommand("INSERT INTO TagReadings VALUES(@Tag, @Time, @SignalStrength, @Antenna, @Frequency)", connection))
        //        {
        //            //command.Parameters.Add(new SqlParameter("ReadingNumber", Count));
        //            command.Parameters.Add(new SqlParameter("Tag", tagInfo.ID.Value));
        //            command.Parameters.Add(new SqlParameter("Time", tagInfo.Time));
        //            command.Parameters.Add(new SqlParameter("SignalStrength", tagInfo.SignalStrenth));
        //            command.Parameters.Add(new SqlParameter("Antenna", tagInfo.Antenna));
        //            command.Parameters.Add(new SqlParameter("Frequency", tagInfo.Frequency));
        //            command.ExecuteNonQuery();
        //        }
        //    }
        //    catch
        //    {
        //        Console.WriteLine("Count not insert.");
        //    }
        //}

        private SqlConnection connection;
        private static int maxReadersDefault = 4;
        private List<ITagEventPublisher> rfidReaders = new List<ITagEventPublisher>(maxReadersDefault);
        private LogDelegate logCommand;
    }
}
