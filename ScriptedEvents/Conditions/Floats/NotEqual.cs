using ScriptedEvents.Conditions.Interfaces;

namespace ScriptedEvents.Conditions.Floats
{
    public class NotEqual : IFloatCondition
    {
        public string Symbol => "!=";

        public bool Execute(float left, float right) => left != right;
    }
}
