
using System.Collections.Generic;
namespace Hardcard.Scoring
{
    /// <summary>
    /// This implements the Null Design Pattern and is used to set an initial
    /// <typeparamref name="IPassingStrategy"/>. This implementation never detects
    /// a passing.
    /// It also uses the Singleton Design Pattern, making it fairly efficient.
    /// </summary>
    class NullPassingStrategy : IPassingStrategy
    {
        /// <summary>
        /// Static constructor called only by the system (CLR). It will create
        /// the single instance. The default constructor is private so no one else
        /// can create this.
        /// </summary>
        static NullPassingStrategy()
        {
            Instance = new NullPassingStrategy();
        }

        /// <summary>
        /// Get the single instance of the NullListener.
        /// </summary>
        public static NullPassingStrategy Instance { get; private set; }

        /// <summary>
        /// Takes the latest reading as does nothing.
        /// </summary>
        /// <param name="tagInfo">The latest reading of the tag</param>
        /// <returns>Always returns false.</returns>
        public TagInfo HandlePassing(IList<TagInfo> tagInfo)
        {
            return new TagInfo();
        }
    }
}
