using ScriptedEvents.Conditions.Interfaces;

namespace ScriptedEvents.Conditions.Strings
{
    public class StringNotEqual : IStringCondition
    {
        public string Symbol => "!=";

        public bool Execute(string left, string right)
        {
            return !left.Equals(right);
        }
    }
}
