
namespace Hardcard.Scoring
{
    /// <summary>
    /// A simple class to send all log messages to the Console.
    /// </summary>
    internal class LoggerConsole : TagSubscriberBase
    {
        /// <summary>
        /// A <typeparamref name="TagReadEventArgs.TagMessageDelegate"/> method that is used to 
        /// handle the events and write to the Console.
        /// </summary>
        /// <param name="sender">The object that sent the event (also the object that was
        /// subscribed to).</param>
        /// <param name="tagInfo">The <typeparamref name="TagReadEventArgs"/> for the current reading.</param>
        internal override void LogTag(object sender, TagReadEventArgs e)
        {
            TagInfo tagInfo = e.TagInfo;
            ITagEventPublisher network = sender as ITagEventPublisher;
            if (network != null)
            {
                System.Console.Write(network.Name);
                System.Console.WriteLine("  Tag ID: {0}  Antenna: {1}  Signal Strength: {2} Frequency: {3}  Time: {4}",
                    tagInfo.ID.Value, tagInfo.Antenna, tagInfo.SignalStrenth, tagInfo.Frequency, tagInfo.Time);
            }
        }

        internal void LogPassing(TagInfo tagInfo)
        {
                System.Console.WriteLine("Passing Time Determined:");
                System.Console.WriteLine("  Tag: {0}   Passing Time: {1}", tagInfo.ID.Value, tagInfo.Time);
        }
    }
}
