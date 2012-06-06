
namespace Hardcard.Scoring
{
    public interface ITagEventPublisher
    {
        /// <summary>
        /// Subscribe / unsubscribe to be informed every time a Tag event occurs.
        /// </summary>
        event TagEventHandler TagDetected;
        /// <summary>
        /// Get the Name of this publisher as a string.
        /// </summary>
        string Name { get; }
    }
}
