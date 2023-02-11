namespace ScriptedEvents.Conditions.Floats
{
    using ScriptedEvents.Conditions.Interfaces;

    public class LessThanOrEqualTo : IFloatCondition
    {
        public string Symbol => "<=";

        public bool Execute(float left, float right) => left <= right;
    }
}
