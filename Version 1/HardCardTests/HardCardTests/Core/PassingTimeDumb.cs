using System;
using System.Collections.Generic;

namespace Hardcard.Scoring
{
    /// <summary>
    /// A very simple passing detection strategy. Just report the
    /// tagInfo data from the middle of the tag records.
    /// </summary>
    public class PassingTimeDumb : IPassingStrategy
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public PassingTimeDumb()
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
            if (tagList.Count > 0)
            {
                int midpoint = tagList.Count / 2;
                return tagList[midpoint];
            }
            throw new InvalidOperationException("THe list of passings sent to PassingTimeDumb.HandlePassing was empty");
        }
        #endregion
    }
}
