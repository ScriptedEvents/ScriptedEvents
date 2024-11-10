namespace ScriptedEvents.Enums
{
    using System;

    /// <summary>
    /// Flags to indicate certain actions to take when an action finishes.
    /// </summary>
    [Flags]
    public enum ActionFlags
    {
        /// <summary>
        /// No actions to take place.
        /// </summary>
        None,

        /// <summary>
        /// Fatal error. Stops execution of the script.
        /// </summary>
        FatalError,

        /// <summary>
        /// No error. Stops execution of the script.
        /// </summary>
        StopEventExecution,
        
        /// <summary>
        /// No error. Stops execution of the script and gets the values returned by the action to add to the script that first executed this script.
        /// </summary>
        StopScriptAndSendReturnedValues,
    }
}
