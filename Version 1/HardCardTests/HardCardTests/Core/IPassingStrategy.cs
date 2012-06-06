using System.Collections.Generic;

namespace Hardcard.Scoring
{
    public interface IPassingStrategy
    {
        /// <summary>
        /// Send the latest reading. If the strategy has enough
        /// information to determine a passing time, HandlePassing
        /// will return true, otherwise it will return false. If true,
        /// the tag info with the passing time can be retrieved from the
        /// Passing property.
        /// </summary>
        /// <param name="tagInfo">The latest reading of the tag</param>
        /// <returns>True if a passing time was calculated. False otherwise.</returns>
        TagInfo HandlePassing(IList<TagInfo> tagInfo);
    }
}
