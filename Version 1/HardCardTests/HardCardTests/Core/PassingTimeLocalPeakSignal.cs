using System;
using System.Collections.Generic;
using System.Text;

namespace HardCard.Scoring
{
    public class PassingTimeLocalPeakSignal : IPassingStrategy
    {
        public PassingTimeLocalPeakSignal()
        {
        }

        #region IPassingStrategy Members
        /// <summary>
        /// Takes a list of TagInfo records and computes a passing
        /// time.
        /// </summary>
        /// <param name="tagList">The latest reading of the tag</param>
        public TagInfo HandlePassing(IList<TagInfo> tagList)
        {
            if (tagList.Count <= 0)
            {
                throw new InvalidOperationException("The list of passings sent to PassingTimePeakSignal.HandlePassing was empty");
            }
            int midpoint = tagList.Count / 2;
            float strength = tagList[midpoint].SignalStrenth;
            //
            // Search backwards for a local maximum signal strength.
            //
            int index = midpoint;
            int leftPeak = midpoint - 1;
            while ((leftPeak >= 0) && (tagList[leftPeak].SignalStrenth > strength))
            {
                strength = tagList[leftPeak].SignalStrenth;
                index = leftPeak;
                leftPeak--;
            }
            //
            // Search Forward for a local maximum signal strength (greater
            //   than the local peak to the left.
            //
            int rightPeak = midpoint + 1;
            while ((rightPeak < tagList.Count) && (tagList[rightPeak].SignalStrenth > strength))
            {
                strength = tagList[rightPeak].SignalStrenth;
                index = rightPeak;
                rightPeak++;
            }
            TagInfo tag = tagList[index];
            tag.Hits = tagList.Count;
            return tag;//tagList[index];
        }
        #endregion
    }
}
