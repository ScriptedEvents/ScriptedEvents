namespace ScriptedEvents.Actions.Interfaces
{
    /// <summary>
    /// Represents any action.
    /// </summary>
    public interface IAction
    {
        string Name { get; }
        string[] Aliases { get; }
        string[] Arguments { get; set; }
    }
}
