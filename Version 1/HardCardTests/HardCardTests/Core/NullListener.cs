using System;

namespace Hardcard.Scoring
{
    /// <summary>
    /// This implements the Null Design Pattern and is used so that we do not need
    /// to check whether anyone subscribed to the events. We will subscribe this to the 
    /// events in <typeparamref name="NetworkListener"/> to avoid this check. It also uses
    /// the Singleton Design Pattern, making it fairly efficient.
    /// </summary>
    public class NullListener
    {
        /// <summary>
        /// Static constructor called only by the system (CLR). It will create
        /// the single instance. The default constructor is private so no one else
        /// can create this.
        /// </summary>
        static NullListener()
        {
            Instance = new NullListener();
        }

        /// <summary>
        /// Get the single instance of the NullListener.
        /// </summary>
        public static NullListener Instance { get; private set; }

        /// <summary>
        /// A no-op implementation of <typeparamref name="TagReadEventArgs.TagMessageDelegate"/> delegate.
        /// </summary>
        /// <param name="sender">Who cares, it is ignored.</param>
        /// <param name="args">Ignored.</param>
        public void IgnoreEvent(object sender, EventArgs args) { }

        /// <summary>
        /// A no-op implementation of <typeparamref name="TagReadEventArgs.TagMessageDelegate"/> delegate.
        /// </summary>
        /// <param name="sender">Who cares, it is ignored.</param>
        /// <param name="args">Ignored.</param>
        public void IgnoreAddEvent(object sender, TagId tagId) { }

        // Note that this is the only constructor and it is hidden.
        private NullListener()
        {
        }
    }
}
