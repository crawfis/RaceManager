
namespace HardCard.Scoring
{
    /// <summary>
    /// Delegate defining the protocol for registering or adding a tag.
    /// </summary>
    /// <param name="sender">The object instance that is calling the method, firing
    /// the event, etc.</param>
    /// <param name="tagInfo">The tag identificatio using the <typeparamref name="TagId"/>
    /// data structure.
    public delegate void TagAddedEventHandler(object sender, TagId tagId);
}
