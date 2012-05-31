using System;
using System.Collections.Generic;
using OhioState.Collections;

namespace HardCard.Scoring.Simulator
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //NetworkSimulator network;
            NetworkListener listener = new NetworkListener("Race Host");
            //TagPassing passDetector = new TagPassing();
            //passDetector.AddReader(listener);
            LoggerConsole log = new LoggerConsole();
            LoggerSummary countLogger = new LoggerSummary();
            LoggerBinary binaryLog = new LoggerBinary("tagReadings.bin");
            //LoggerText textLog = new LoggerText("tagReadings.txt");
            //LoggerXML log = new LoggerXML("test.xml");
            //log.AddReader(listener);
            int initialiCapacity = 10000;
            PriorityCollectionBlocking<TagInfo> queue = new PriorityCollectionBlocking<TagInfo>("Queue", initialiCapacity);
            BufferReadings buffer = new BufferReadings(queue);
            ProcessBufferedReadings passDetector = new ProcessBufferedReadings("Pass Detector", queue);
            binaryLog.AddPublisher(listener);
            buffer.AddPublisher(listener);
            countLogger.AddPublisher(listener);
            log.AddPublisher(passDetector);
            passDetector.Start();
            listener.Start();

            //System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
            //if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    string filename = fileDialog.FileName;
            //    if (System.IO.File.Exists(filename))
            //    {
            //        network = new NetworkSimulator(filename, new TestTagProcessor());
            //        network.AddListener(listener);
            //        network.Start(true);
            //        //log.Serialize();
            //    }
            //    else
            //    {
            //        Console.WriteLine("Error! The specified file does not exist. Rerun the applications.");
            //    }
            //}

            Console.WriteLine("Press any key stop the reading ...");
            Console.Read();
            listener.End();
            passDetector.Exit();
            countLogger.PrintStatsToConsole();
            binaryLog.Dispose();
            Console.WriteLine("Press any key to exit the demo ...");
            Console.Read();
            Environment.Exit(0);
        }
    }
}
