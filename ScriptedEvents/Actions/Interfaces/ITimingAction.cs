namespace ScriptedEvents.Actions.Interfaces
{
    public interface ITimingAction : IAction
    {
        public float? Execute(Script script, out ActionResponse message);
    }
}
