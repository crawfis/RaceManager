using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace HardCard.Scoring
{
    // TODO; This is just a quick place holder. The newer technology and LINQ may make this much easier and
    // more flexible.
    public class LoggerSql : ITagEventSubscriber
    {
        public LoggerSql(SqlConnection connection)
        {
            this.connection = connection;
            connection.Open();
            try
            {
                // TODO: Check whether we are creating a table or if the table will be pre-built.
                // TODO: Make this more parameterable / configurable. Remove hard-coding.
                using (SqlCommand command = new SqlCommand("CREATE TABLE RaceTest (TagID TEXT, Time INT, Strength FLOAT)", connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch
            {
                Console.WriteLine("Table couldn't be created.");
            }
        }

        public void AddPublisher(ITagEventPublisher rfidReader)
        {
            rfidReaders.Add(rfidReader);
            rfidReader.TagDetected += new TagEventHandler(LogTag);
        }

        internal void LogTag(object sender, TagReadEventArgs tagInfo)
        {
            NetworkListener network = sender as NetworkListener;
            if (network != null)
                Log(tagInfo);
        }

        private void Log(TagReadEventArgs e)
        {
            TagInfo tagInfo = e.TagInfo;
            connection.Open();
            try
            {
                using (SqlCommand command = new SqlCommand("INSERT INTO Dogs1 VALUES(@TagID, @Time, @Strength)", connection))
                {
                    command.Parameters.Add(new SqlParameter("TagID", tagInfo.ID.Value));
                    command.Parameters.Add(new SqlParameter("Time", tagInfo.Time));
                    command.Parameters.Add(new SqlParameter("Strength", tagInfo.SignalStrenth));
                    command.ExecuteNonQuery();
                }
            }
            catch
            {
                Console.WriteLine("Count not insert.");
            }
        }

        private SqlConnection connection;
        private static int maxReadersDefault = 4;
        private List<ITagEventPublisher> rfidReaders = new List<ITagEventPublisher>(maxReadersDefault);
    }
}
