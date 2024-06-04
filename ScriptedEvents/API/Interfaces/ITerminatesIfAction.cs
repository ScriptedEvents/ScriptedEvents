namespace ScriptedEvents.API.Interfaces
{
    /// <summary>
    /// Indicates an action that ignores the <see cref="Script.IfActionBlocksExecution"/> property, allowing the action to run even if the value is true.
    /// </summary>
    internal interface ITerminatesIfAction
    {
    }
}
