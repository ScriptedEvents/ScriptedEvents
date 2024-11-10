namespace ScriptedEvents.API.Features.Exceptions
{
    using System;

    /// <summary>
    /// An expection that should be impossible to trigger.
    /// </summary>
    public class ImpossibleException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImpossibleException"/> class.
        /// </summary>
        public ImpossibleException()
        {
        }
    }
}
