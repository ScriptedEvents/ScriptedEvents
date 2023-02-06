namespace ScriptedEvents.Conditions.Interfaces
{
    public interface IFloatCondition : ICondition
    {
        public bool Execute(float left, float right);
    }
}
