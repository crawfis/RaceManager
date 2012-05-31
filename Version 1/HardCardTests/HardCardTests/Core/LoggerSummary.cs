using System.Collections.Generic;

namespace HardCard.Scoring
{
    public class LoggerSummary : TagSubscriberBase
    {
        internal override void LogTag(object sender, TagReadEventArgs e)
        {
            string tagName = e.TagInfo.ID.Value;
            if (tagCounts.ContainsKey(tagName))
            {
                tagCounts[tagName] = tagCounts[tagName] + 1;
            }
            else
            {
                tagCounts.Add(tagName, 1);
            }
        }

        public void PrintStatsToConsole()
        {
            foreach (var tag in tagCounts)
            {
                System.Console.WriteLine(" Tag: {0}, Count: {1}", tag.Key, tag.Value);
            }
        }

        private Dictionary<string, int> tagCounts = new Dictionary<string, int>(16);
    }
}
