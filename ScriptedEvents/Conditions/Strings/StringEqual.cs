namespace ScriptedEvents.Conditions.Strings
{
    using ScriptedEvents.Conditions.Interfaces;

    public class StringEqual : IStringCondition
    {
        public string Symbol => "=";

        public bool Execute(string left, string right)
        {
            return left.Equals(right);
        }
    }
}
