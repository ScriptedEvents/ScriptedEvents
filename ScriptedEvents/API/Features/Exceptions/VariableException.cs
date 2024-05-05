namespace ScriptedEvents.API.Features.Exceptions
{
    /// <summary>
    /// Exception thrown when a variable errors.
    /// </summary>
    public class VariableException : ScriptedEventsException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VariableException"/> class.
        /// </summary>
        /// <param name="message">The message in the exception.</param>
        public VariableException(string message)
            : base(message)
        {
        }
    }
}
