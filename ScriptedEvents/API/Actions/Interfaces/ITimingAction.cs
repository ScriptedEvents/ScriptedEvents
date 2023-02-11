namespace ScriptedEvents.API.Features.Actions
{
    public interface ITimingAction : IAction
    {
        public float? Execute(Script script, out ActionResponse message);
    }
}
