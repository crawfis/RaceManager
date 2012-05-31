using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Hardcard.Scoring;
using System.Net.Sockets;
using System.Threading;
using OhioState.Collections;

namespace Hardcard.Simulator
{
    public partial class Form1 : Form
    {
        private IList<TagInfo> readings;
        private long currentTime = -500;
        private long startTime;
        private int lastTime = 0;
        private int currentReadingIndex = 0;
        private int speed = 1;
        private string server = "localhost";
        private int port = 3900;
        private bool quitting = false;
        private Thread connectionThread;
        private IPriorityCollection<TagInfo> readingsQueue;
        public Form1()
        {
            InitializeComponent();
            readingsQueue = new PriorityCollectionBlocking<TagInfo>("Queue", 50);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if( this.openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                using (BinaryReader reader = new BinaryReader(File.Open(openFileDialog1.FileName, FileMode.Open)))
                {
                    int numberOfReadings = reader.ReadInt32();
                    readings = new List<TagInfo>(numberOfReadings);
                    for (int i = 0; i < numberOfReadings; i++)
                    {
                        TagInfo tag = new TagInfo();

                        TagId id;
                        id.Value = reader.ReadString();
                        tag.ID = id;
                        tag.Time = reader.ReadInt64();
                        tag.SignalStrenth = reader.ReadSingle();
                        tag.Antenna = reader.ReadInt32();
                        tag.Frequency = reader.ReadSingle();
                        readings.Add(tag);
                    }
                    
                }
                this.dataGridView1.DataSource = readings;
                EnablePlayBack();
            }
        }

        private void EnablePlayBack()
        {
            playButton.Enabled = true;
            startTime = readings[0].Time;
            toolStripProgressBar1.Maximum = mapTimeToInt(readings[readings.Count - 1].Time);
            toolStripNumReadingsLabel.Text = readings.Count.ToString() + " Readings";

            connectionThread = new Thread(StartConnection);
            connectionThread.Start();
        }

        internal void StartConnection()
        {
            client = new TcpClient();
            TryToConnect();
            while (!quitting)
            {
                TagInfo tag = readingsQueue.GetNext();
                NetworkStream stream = client.GetStream();
                StringBuilder tagLineBuilder = new StringBuilder(128);

                tagLineBuilder.Append("tag: ");
                tagLineBuilder.Append(tag.ID.Value);
                tagLineBuilder.Append(", freq: ");
                tagLineBuilder.Append(tag.Frequency);
                tagLineBuilder.Append(", sig: ");
                tagLineBuilder.Append(tag.SignalStrenth);
                tagLineBuilder.Append(", ant: ");
                tagLineBuilder.Append(tag.Antenna);
                tagLineBuilder.Append(", time: ");
                tagLineBuilder.Append(tag.Time);
                tagLineBuilder.AppendLine();
                // Translate the passed message into ASCII and store it as a Byte array.
                string tagLine = tagLineBuilder.ToString();
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(tagLine);
                stream.Write(data, 0, data.Length);

                // Translate the passed message into ASCII and store it as a Byte array.
                //Byte[] data = System.Text.Encoding.ASCII.GetBytes("tag, ");
                //stream.Write(data, 0, data.Length);
                //data = System.Text.Encoding.ASCII.GetBytes(tagInfo.ID.Value);
                //stream.Write(data, 0, data.Length);
                //data = System.Text.Encoding.ASCII.GetBytes(", freq, ");
                //stream.Write(data, 0, data.Length);
                //data = System.Text.Encoding.ASCII.GetBytes(tagInfo.Frequency.ToString());
                //stream.Write(data, 0, data.Length);
                //data = System.Text.Encoding.ASCII.GetBytes(", sig, ");
                //stream.Write(data, 0, data.Length);
                //data = System.Text.Encoding.ASCII.GetBytes(tagInfo.SignalStrenth.ToString());
                //stream.Write(data, 0, data.Length);
                //data = System.Text.Encoding.ASCII.GetBytes(", ant, ");
                //stream.Write(data, 0, data.Length);
                //data = System.Text.Encoding.ASCII.GetBytes(tagInfo.Antenna.ToString());
                //stream.Write(data, 0, data.Length);
                //data = System.Text.Encoding.ASCII.GetBytes(", time, ");
                //stream.Write(data, 0, data.Length);
                //data = System.Text.Encoding.ASCII.GetBytes(tagInfo.Time.ToString());
                //stream.Write(data, 0, data.Length);
                //data = System.Text.Encoding.ASCII.GetBytes("\n");
                //stream.Write(data, 0, data.Length);

            }
        }

        private void TryToConnect()
        {
            while (!quitting)
            {
                try
                {
                    client.Connect(server, port);
                    break;
                }
                catch
                {
                }
            }
        }

        private int mapTimeToInt(long time)
        {
            return (int) ((time-startTime) / 10000);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            stopButton.Enabled = true;
            playButton.Enabled = false;
            timer1.Start();
            currentTime += readings[0].Time;
            lastTime = System.Environment.TickCount;
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            stopButton.Enabled = false;
            playButton.Enabled = true;
            timer1.Stop();
            toolStripProgressBar1.Value = 0;
            currentTime = -500;
            currentReadingIndex = 0;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int ticks = System.Environment.TickCount;
            int delta = speed * (ticks - lastTime);
            lastTime = ticks;
            currentTime += delta;
            int index = currentReadingIndex;
            while ((currentReadingIndex < readings.Count) && (readings[currentReadingIndex].Time < currentTime))
            {
                index = currentReadingIndex;
                this.toolStripProgressBar1.Value = mapTimeToInt(readings[index].Time);
                dataGridView1.Rows[index].Selected = true;
                dataGridView1.CurrentCell = dataGridView1.Rows[index].Cells[0];
                if (index > 10)
                    this.dataGridView1.FirstDisplayedScrollingRowIndex = index - 10;
                timeLabel.Text = readings[index].DateTime.ToLongTimeString();

                FireReadingEvent(readings[currentReadingIndex]);
                currentReadingIndex++;
            }
            if (currentReadingIndex >= readings.Count)
            {
                stopButton.Enabled = false;
                playButton.Enabled = true;
                timer1.Stop();
                toolStripProgressBar1.Value = 0;
                currentTime = -500;
                currentReadingIndex = 0;
                startTime = readings[0].Time;
            }
        }

        private TcpClient client;
        private void FireReadingEvent(TagInfo tagInfo)
        {
            //Marshal this over to the connectionThread.
            readingsQueue.Put(tagInfo);
        }

        private void fullspeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            speed = 1;
        }

        private void speed2xMenuItem_Click(object sender, EventArgs e)
        {
            speed = 2;
        }

        private void speed4xMenuItem_Click(object sender, EventArgs e)
        {
            speed = 4;
        }

        private void speed8xMenuItem_Click(object sender, EventArgs e)
        {
            speed = 8;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Quit();
        }

        private void Quit()
        {
            quitting = true;
            readingsQueue.Put(new TagInfo());
            Thread.Sleep(4);
        }
    }
}
