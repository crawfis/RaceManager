using System;

namespace HardCard.Scoring
{
    /// <summary>
    /// This class is fairly dumb. It requires the user to send in a delegate 
    /// which is called to handle the actual tag message. This is useful for 
    /// keeping the data bindings or Sql connections separate from the tag logging.
    /// </summary>
    public class TagSubscriber : TagSubscriberBase
    {
        /// <summary>
        /// A delegate type definition that returns void and takes
        /// a single parameter.
        /// </summary>
        /// <param name="e">An instance of <typeparamref name="TagReadEventArgs"/></param>
        public delegate void TagReadDelegate(TagReadEventArgs e);

        public TagSubscriber(TagReadDelegate logCommand)
        {
            if (logCommand == null)
                throw new ArgumentNullException("No LogDelegate was specified on the constructor to LoggerSql");

            this.logCommand = logCommand;
        }

        internal override void LogTag(object sender, TagReadEventArgs tagInfo)
        {
            logCommand(tagInfo);
        }

        private TagReadDelegate logCommand;
    }
}
