using ScriptedEvents.Enums;

namespace ScriptedEvents.Structures
{
    using System;

    /// <summary>
    /// Represents a response to an action execution.
    /// </summary>
    public class ActionResponse
    {
        public ActionResponse(bool success, ActionReturnValues? values = null, ErrorTrace? errorTrace = null, ActionFlags flags = ActionFlags.None)
        {
            Success = success;
            ErrorTrace = errorTrace;
            ResponseFlags = flags;
            ResponseVariables = values?.Values ?? Array.Empty<object>();
        }

        /// <summary>
        /// Gets a value indicating whether or not the execution of the action was successful.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Gets the error trace if the action failed.
        /// </summary>
        public ErrorTrace? ErrorTrace { get; }

        /// <summary>
        /// Gets flags that control what happens after the execution is complete.
        /// </summary>
        public ActionFlags ResponseFlags { get; }

        /// <summary>
        /// Gets variables that were returned as result of the action.
        /// </summary>
        public object[] ResponseVariables { get; }
    }
}
