namespace ScriptedEvents.API.Features.Exceptions
{
    using System;

    /// <summary>
    /// Exception thrown by Scripted Events.
    /// </summary>
    public class ScriptedEventsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptedEventsException"/> class.
        /// </summary>
        /// <param name="message">The message in the exception.</param>
        public ScriptedEventsException(string message)
            : base(message)
        {
        }
    }
}
