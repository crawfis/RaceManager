using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Hardcard.Scoring
{
    /// <summary>
    /// NetworkListener converts the incoming network packets from
    /// the RFID readers to events that other listeners can subscribe to.
    /// </summary>
    public class NetworkListener : ITagEventPublisher
    {
        #region Public Interface
        /// <summary>
        /// Constructor. Require a nice name to refer to this NetworkListener by.
        /// </summary>
        /// <param name="name">A name to be used to set the Name property to.</param>
        public NetworkListener(string name)
        {
            portNumber = Config.Instance.PortNumber;
            Name = name;
            TagDetected += NullListener.Instance.IgnoreEvent;
            TagAdded += NullListener.Instance.IgnoreAddEvent;
        }

        /// <summary>
        /// Get a string representing the user-defined name for this instance.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Subscribe / unsubscribe to be informed every time a new tag is added to the system.
        /// </summary>
        public event TagAddedEventHandler TagAdded;

        /// <summary>
        /// Subscribe / unsubscribe to be informed every time a Tag is read.
        /// </summary>
        public event TagEventHandler TagDetected;

        /// <summary>
        /// Configure the TcpListener and start a new thread to handle client
        /// comminications.
        /// </summary>
        public void Start()
        {
            tcpListener = new TcpListener(System.Net.IPAddress.Any, portNumber);
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            listenThread.Name = this.Name;
            this.listenThread.Start();
        }

        /// <summary>
        /// Signal to the running thread that it should end.
        /// </summary>
        public void End()
        {
            quitRequested = true;
            System.Threading.Thread.Sleep(1);
            tcpListener.Server.Close();
            try
            {
                tcpListener.Stop();
            }
            catch (SocketException e)
            {
            }
        }
        #endregion

        #region Implementation Details
        /// <summary>
        /// A dummy test stub that when passed a <typeparamref name="TagReadEventArgs"/> fires
        /// the TagDetected event. It also checks for any new tags and fires the TagAdded event
        /// if one is found.
        /// </summary>
        /// <param name="tagInfo">The <typeparamref name="TagReadEventArgs"/> with the reading information
        /// to propogate.</param>
        internal void TestReceivePacket(TagReadEventArgs e)
        {
            TagInfo tagInfo = e.TagInfo;
            if (!registeredTags.Contains(tagInfo.ID))
            {
                registeredTags.Add(tagInfo.ID);
                TagAdded(this, tagInfo.ID);
            }
            TagDetected(this, e);
        }

        private void ListenForClients()
        {
            try
            {
                this.tcpListener.Start();
            }
            catch (SocketException e)
            {
                // Not sure how to handle this here. THe calling method should handle this, but it was called from a different thread.
                // No good access to the logger. That needs fixed. Do not want to load Windows.Forms, but seems the best bet.
                System.Windows.Forms.MessageBox.Show("Could not open the network port. Check that another program is not using current port");
                //throw e;
            }
 
            while (!quitRequested)
            {
                //blocks until a client has connected to the server
                // TODO: This will hang at closing until a tag is read.
                try
                {
                    TcpClient client = this.tcpListener.AcceptTcpClient();

                    //create a thread to handle communication
                    //with connected client
                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                    clientNumber++;
                    clientThread.Name = "TcpClient" + clientNumber.ToString();
                    clientThread.Start(client);
                }
                catch (SocketException e)
                {
                }
            }
        }

        // This method is started on a new thread each time a new client is connected 
        //    (or reconnected after a time-out). See ListenForClients() above.
        // It reads the packets, splits them into tag readings and calls the parser 
        //    on each reading.
        private void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            try
            {
                NetworkStream clientStream = tcpClient.GetStream();

                int bytesRead;

                while (!quitRequested)
                {
                    bytesRead = 0;

                    try
                    {
                        bytesRead = ReadData(clientStream);
                    }
                    catch
                    {
                        //a socket error has occured
                        break;
                    }

                    if (bytesRead == 0)
                    {
                        //the client has disconnected from the server
                        break;
                    }
                }
            }
            catch { }
            finally
            {
                tcpClient.Close();
            }
        }

        // Reads the data from the stream, splits into tag readings and 
        // calls a parser to get the tag info out.
        // Calls SendTag for each tag read.
        // TODO: If there is a tag that stays in the antenna (or reader is 
        //     constantly sending data will this flounder or just never send
        //     tags until there is no activity?
        private int ReadData(NetworkStream stream)
        {
            byte[] readBuffer = new byte[1024];
            StringBuilder tagReadings = new StringBuilder();
            int numberOfBytesRead = 0;
            int totalBytesRead = 0;

            // Incoming message may be larger than the buffer size.
            do
            {
                // This blocks until data arrives.
                numberOfBytesRead = stream.Read(readBuffer, 0, readBuffer.Length);
                totalBytesRead += numberOfBytesRead;

                tagReadings.AppendFormat("{0}", Encoding.ASCII.GetString(readBuffer, 0, numberOfBytesRead));

            }
            while (stream.DataAvailable);

            string[] lines = tagReadings.ToString().ToLower().Split('\0', '\r', '\n');
            foreach (string line in lines)
            {
                if (line != String.Empty)
                {
                    //Console.WriteLine("Received: {0}", line);
                    try
                    {
                        TagInfo tag = lineProcessor.Process(line);
                        SendTag(tag);
                    }
                    catch (ApplicationException e)
                    {
                        Console.WriteLine("Invalid tagReadings packet!");
                        Console.WriteLine("Tag ignored with the following line:");
                        Console.Write("   ");
                        Console.WriteLine(e.Message);
                    }
                }
            }
            return totalBytesRead;
        }

        // Wrap the tag into a TagReadEventArgs and fire it.
        private void SendTag(TagInfo tagInfo)
        {
            TagReadEventArgs e = new TagReadEventArgs(TagEventType.Read, tagInfo);
            TagDetected(this, e);
        }
        #endregion

        #region Instance variables (data).
        private List<TagId> registeredTags = new List<TagId>();
        private /*readonly*/ int portNumber = 3900;
        private Hardcard.Scoring.Simulator.ILineProcessStrategy lineProcessor = new Hardcard.Scoring.Simulator.TestTagProcessor();
        private bool quitRequested = false;
        private Thread listenThread;
        TcpListener tcpListener = null;
        private int clientNumber = 0;
        #endregion
    }
}
