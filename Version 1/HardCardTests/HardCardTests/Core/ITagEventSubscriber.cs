
namespace Hardcard.Scoring
{
    public interface ITagEventSubscriber
    {
        /// <summary>
        /// Adds a new <typeparamref name="ITagEventPublisher"/> to subscribe to and
        /// listen for TagEvent events. This is how the class knows about Tags 
        /// and when they are activated by the antenna.
        /// </summary>
        /// <param name="tagPublisher">The <typeparamref name="ITagEventPublisher"/> to listen to.</param>
        void AddPublisher(ITagEventPublisher tagPublisher);
    }
}
