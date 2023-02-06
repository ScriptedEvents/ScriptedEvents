namespace ScriptedEvents.API.Features.Actions
{
    public interface ITimingAction : IAction
    {
        public float? GetDelay(out ActionResponse message);
    }
}
