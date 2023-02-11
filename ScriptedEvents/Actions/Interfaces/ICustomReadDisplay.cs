namespace ScriptedEvents.Actions.Interfaces
{
    /// <summary>
    /// Indicates that the action has a custom display in the "script read" command.
    /// </summary>
    public interface ICustomReadDisplay
    {
        /// <summary>
        /// Controls how to display the action in the read command.
        /// </summary>
        /// <param name="message">The text to show, instead of the standard action text.</param>
        /// <returns>Whether or not to show the action in the command.</returns>
        public bool Read(out string message);
    }
}
