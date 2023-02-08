namespace ScriptedEvents.API.Features.Actions
{
    public interface IScriptAction : IAction
    {
        ActionResponse Execute(Script script);
    }
}
