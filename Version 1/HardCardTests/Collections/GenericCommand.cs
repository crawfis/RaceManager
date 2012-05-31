using System;

namespace OhioState.Collections
{
    /// <summary>
    /// A Command Design pattern using Generics and delegates (or lambda expression).
    /// </summary>
    /// <typeparam name="T">The class or interface that will receive the command.</typeparam>
    /// <remarks>See Jason Olson's June 14, 2008 Blog: 
    /// http://www.managed-world.com/2008/06/15/AvoidingInheritanceDependenciesUsingGenericsAndLambdas.aspx
    /// on this.</remarks>
    /// <remarks>Note that this is strongly typed. As such, you should use this only with types supporting a
    /// common (and higher-level) interface, otherwise use the standard Command Design Pattern.</remarks>
    public class GenericCommand<T>
    {
        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="receiver">The object that will be used in the command.</param>
        /// <param name="commandToExecute">An <typeparamref name="Action{T}"/> or delegate
        /// instance that takes as input one parameter, the <paramref name="receiver"/>.</param>
        public GenericCommand(T receiver, Action<T> commandToExecute)
        {
            this.receiver = receiver;
            this.commandToExecute = commandToExecute;
        }
        #endregion

        #region Public interface
        /// <summary>
        /// Perform the command, passing in the reciever.
        /// </summary>
        public void Execute()
        {
            commandToExecute(receiver);
        }
        #endregion

        #region Member variables
        private T receiver;
        private Action<T>  commandToExecute;
        #endregion
    }
}
