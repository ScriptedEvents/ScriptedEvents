namespace ScriptedEvents.Actions.Interfaces
{
    public interface IAction
    {
        string Name { get; }
        string[] Aliases { get; }
        string[] Arguments { get; set; }
    }
}
