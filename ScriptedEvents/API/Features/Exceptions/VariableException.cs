namespace ScriptedEvents.API.Features.Exceptions
{
    using System;

    /// <summary>
    /// Exception thrown when a disabled script is executed.
    /// </summary>
    public class VariableException : Exception
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
