namespace ScriptedEvents.Actions.Interfaces
{
    public interface IScriptAction : IAction
    {
        ActionResponse Execute(Script script);
    }
}
