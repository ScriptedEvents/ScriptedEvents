namespace ScriptedEvents.API.Enums
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
        None = 0,

        /// <summary>
        /// Fatal error. Stops execution of the script.
        /// </summary>
        FatalError = 1,

        /// <summary>
        /// No error. Stops execution of the script.
        /// </summary>
        StopEventExecution = 2,
    }
}
