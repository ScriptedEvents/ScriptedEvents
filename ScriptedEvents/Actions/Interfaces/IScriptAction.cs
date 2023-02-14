namespace ScriptedEvents.Actions.Interfaces
{
    using ScriptedEvents.Structures;

    /// <summary>
    /// Represents an action that can be executed and returns a response.
    /// </summary>
    public interface IScriptAction : IAction
    {
        /// <summary>
        /// The code to run when the action is reached.
        /// </summary>
        /// <param name="script">The Script that is currently running.</param>
        /// <returns>Whether or not the execution was successful, and any additional errors.</returns>
        ActionResponse Execute(Script script);
    }
}
