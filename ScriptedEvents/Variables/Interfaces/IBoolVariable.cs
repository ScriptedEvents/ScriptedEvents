namespace ScriptedEvents.Variables.Interfaces
{
    public interface IBoolVariable : IConditionVariable
    {
        public string ReversedName { get; }
        public bool Value { get; }
    }
}
