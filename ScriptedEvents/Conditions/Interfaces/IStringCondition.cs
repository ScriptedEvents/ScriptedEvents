namespace ScriptedEvents.Conditions.Interfaces
{
    public interface IStringCondition : ICondition
    {
        public bool Execute(string left, string right);
    }
}
