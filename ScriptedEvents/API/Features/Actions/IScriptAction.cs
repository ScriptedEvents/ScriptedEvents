namespace ScriptedEvents.API.Features.Actions
{
    public interface IScriptAction
    {
        ActionResponse Execute(Script script);
    }
}
