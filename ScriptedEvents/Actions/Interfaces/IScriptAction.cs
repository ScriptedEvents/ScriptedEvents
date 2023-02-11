using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Interfaces
{
    /// <summary>
    /// Represents an action that can be executed.
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
