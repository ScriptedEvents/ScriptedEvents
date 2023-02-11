namespace ScriptedEvents.Conditions.Floats
{
    using ScriptedEvents.Conditions.Interfaces;

    public class Equal : IFloatCondition
    {
        public string Symbol => "=";

        public bool Execute(float left, float right) => left == right;
    }
}
