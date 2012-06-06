using System;
using System.Collections.Generic;
using System.IO;
using Hardcard.Scoring;

namespace Hardcard.Scoring.Simulator
{
    /// <summary>
    /// Read in a log and produce network packets to simulate
    /// the RFID readers.
    /// </summary>
    internal class NetworkSimulator
    {
        public NetworkSimulator(string fileName, ILineProcessStrategy lineProcessor)
        {
            this.fileName = fileName;
            this.lineProcessor = lineProcessor;
        }

        public void AddListener(NetworkListener listener)
        {
            listeners.Add(listener);
        }

        public void Start(bool useTiming)
        {
             try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(fileName))
                {
                    String line;
                    // Read the first line and determine the start time.
                    if (useTiming && (line = sr.ReadLine()) != null)
                    {
                    }
                    // Read lines from the file until the end of
                    // the file is reached.
                    bool firstTag = true;
                    long startTime = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        //Console.WriteLine(line);
                        TagInfo tagInfo = lineProcessor.Process(line);
                        TagReadEventArgs e = new TagReadEventArgs(TagEventType.Read, tagInfo);
                        // Read the first line and determine the start time.
                        if (useTiming)
                        {
                            if (firstTag)
                            {
                                firstTag = false;
                                startTime = e.TagInfo.Time;
                            }
                            else
                            {
                                int sleepTime = (int)(e.TagInfo.Time - startTime);
                                System.Threading.Thread.Sleep(sleepTime);
                            }
                        }
                        foreach (NetworkListener listener in listeners)
                        {
                            listener.TestReceivePacket(e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        private string fileName;
        private ILineProcessStrategy lineProcessor;
        private List<NetworkListener> listeners = new List<NetworkListener>();
    }
}
