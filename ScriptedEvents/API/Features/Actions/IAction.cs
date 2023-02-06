namespace ScriptedEvents.API.Features.Actions
{
    public interface IAction
    {
        string Name { get; }
        string[] Aliases { get; }
        string[] Arguments { get; set; }
        ActionResponse Execute();
    }
}
