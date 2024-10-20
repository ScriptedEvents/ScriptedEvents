namespace ScriptedEvents.ErrorSystem
{
    using System.Collections.Generic;

    using ScriptedEvents.Structures;

    public class ErrorTrace
    {
        public ErrorTrace(List<ErrorInfo> errors, Script script)
        {
            Errors = errors;
        }

        /// <summary>
        /// Gets or sets the stack of errors.
        /// </summary>
        List<ErrorInfo> Errors { get; set; }
    }
}