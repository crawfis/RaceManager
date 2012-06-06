using System.Collections.Generic;
using System.Threading;
using OhioState.Collections;

namespace Hardcard.Scoring
{
    public class ProcessBufferedReadings : ITagEventPublisher
    {
        public ProcessBufferedReadings(string name, IPriorityCollection<TagInfo> readingsQueue)
        {
            sleepTime = Config.Instance.SleepTime;
            System.Console.WriteLine("Sleep time is: " + sleepTime);
            this.Name = name;
            this.readingsQueue = readingsQueue;
            leftCloud = new Dictionary<TagId, bool>(MaximumNumberOfTags);
        }

        #region ITagEventPublisher Members
        public event TagEventHandler TagDetected;

        public string Name
        {
            get;
            set;
        }
        #endregion

        public void Start()
        {
            // TODO: Spawn thread and timer to signal the thread should work. Could just use a blocking queue, but
            //   we want some delay between processing the requests.
            passingThread = new Thread(PassingThreadStart);
            passingThread.Start();
        }

        public void Exit()
        {
            exitRequested = true;
            if (!passingThread.Join(sleepTime))
                passingThread.Abort();
        }

        internal void PassingThreadStart()
        {
            while (!exitRequested)
            {
                Thread.Sleep(sleepTime);
                CheckForPassings();
            }
        }


        internal void CheckForPassings()
        {
            // Mark all active tags initially as leaving the cloud and needing a pass time determined.
            foreach (TagId tag in activeTags)
            {
                leftCloud[tag] = true;
            }
            // Process the queue. For each reading mark the tag as not leaving the cloud and add the
            // reading to the tag's list.
            while( readingsQueue.Count > 0)
            {
                TagInfo reading = readingsQueue.GetNext();
                if( lastTagReading.ContainsKey(reading.ID))
                    if ((reading.Time - lastTagReading[reading.ID]) < sleepTime)
                        continue;
                // If this is the first time we have seen the tag since its last passing, then
                // add it to the list of actively tracked tags.
                if( !activeTags.Contains(reading.ID))
                {
                    activeTags.Add(reading.ID);
                }
                leftCloud[reading.ID] = false; // Adds it if not found.
                // Allocate a List<T> for the tag if not done already.
                if (!tagReadings.ContainsKey(reading.ID))
                {
                    tagReadings[reading.ID] = new List<TagInfo>();
                }
                // Add the Reading to the list of readings associated with that tag.
                if (tagReadings[reading.ID].Count < maxReadingsPerPassing)
                    tagReadings[reading.ID].Add(reading);
            }
            // Determine those tags that left the cloud and process them.
            List<TagId> removeList = new List<TagId>();
            foreach (TagId tag in activeTags)
            {
                if (leftCloud[tag])
                    removeList.Add(tag);
            }
            // Process the tags that left the cloud, calculating the passing time and reseting the data structures.
            foreach (TagId tag in removeList)
            {
                activeTags.Remove(tag);
                List<TagInfo> passingTimes = tagReadings[tag];
                // TODO: pass the list of readings over to Passing Strategy to determine a passing time.
                ProcessPassingTimes(passingTimes);
                lastTagReading[tag] = passingTimes[passingTimes.Count - 1].Time;
                tagReadings[tag] = new List<TagInfo>();
            }
        }

        private void ProcessPassingTimes(List<TagInfo> passingTimes)
        {
            // Create TagInfo with the new calculated passing time.
            TagInfo tagInfo = passingStrategy.HandlePassing(passingTimes);
            TagReadEventArgs tagEvent = new TagReadEventArgs(TagEventType.PassDetermined, tagInfo);
            TagDetected(this, tagEvent);
        }

        private const int maxReadingsPerPassing = 2000;
        private static int MaximumNumberOfTags = 1024; // TODO; Move this to a config file or separate file.
        private IPriorityCollection<TagInfo> readingsQueue;
        private List<TagId> activeTags = new List<TagId>(64); // Tags currently in the cloud.
        private Dictionary<TagId, bool> leftCloud;
        private Dictionary<TagId, List<TagInfo>> tagReadings = new Dictionary<TagId, List<TagInfo>>();
        private Dictionary<TagId, long> lastTagReading = new Dictionary<TagId, long>();
        private int sleepTime = 1000; // 1 second.
        private Thread passingThread;
        private bool exitRequested = false;
        private IPassingStrategy passingStrategy = new PassingTimeLocalPeakSignal();
    }
}
