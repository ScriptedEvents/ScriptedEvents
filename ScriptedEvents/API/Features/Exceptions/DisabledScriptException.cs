namespace ScriptedEvents.API.Features.Exceptions
{
    /// <summary>
    /// Exception thrown when a disabled script is executed.
    /// </summary>
    public class DisabledScriptException : ScriptedEventsException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisabledScriptException"/> class.
        /// </summary>
        /// <param name="scriptName">The name of the script that is attempting to execute.</param>
        public DisabledScriptException(string scriptName)
            : base($"The given script '{scriptName}' is disabled.")
        {
            ScriptName = scriptName;
        }

        /// <summary>
        /// Gets the name of the script that threw the exception.
        /// </summary>
        public string ScriptName { get; }
    }
}
