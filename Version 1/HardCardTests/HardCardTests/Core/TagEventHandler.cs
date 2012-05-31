
namespace HardCard.Scoring
{
    /// <summary>
    /// Delegate defining the protocol for handling a tag detection
    /// message, or any message that passing a <typeparamref name="TagReadEventsArg"/>.
    /// </summary>
    /// <param name="sender">The object instance that is calling the method, firing
    /// the event, etc.</param>
    /// <param name="tagInfo">The information about the tag, including its ID contained
    /// in a <typeparamref name=""/>TagReadEventArgs</param> data structure.
    public delegate void TagEventHandler(object sender, TagReadEventArgs tagInfo);
}
