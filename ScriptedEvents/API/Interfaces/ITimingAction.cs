namespace ScriptedEvents.API.Interfaces
{
    using ScriptedEvents.Structures;

    /// <summary>
    /// Represents an action that returns an amount of time to delay the remainder of the action for.
    /// </summary>
    public interface ITimingAction : IAction
    {
        /// <summary>
        /// The code to run when the action is reached.
        /// </summary>
        /// <param name="script">The Script that is currently running.</param>
        /// <param name="message">Whether or not the execution was successful, and any additional errors.</param>
        /// <returns>The amount of time to yield the script execution for.</returns>
        public float? Execute(Script script, out ActionResponse message);
    }
}
