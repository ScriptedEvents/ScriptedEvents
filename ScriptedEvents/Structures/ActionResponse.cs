namespace ScriptedEvents.Structures
{
    using System;

    using ScriptedEvents.API.Enums;

    /// <summary>
    /// Represents a response to an action execution.
    /// </summary>
    public class ActionResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionResponse"/> class.
        /// </summary>
        /// <param name="success">Whether or not the execution of the action was successful.</param>
        /// <param name="errorTrace">The error trace if the action has failed.</param>
        /// <param name="flags">Flags that control what happens after the execution is complete.</param>
        /// <param name="variablesToRet">Variables to assign after successful action usage.</param>
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
