namespace ScriptedEvents.Variables.Condition
{
#pragma warning disable SA1402 // File may only contain a single type
    using ScriptedEvents.Variables.Interfaces;

    public class CustomVariable : IConditionVariable
    {
        public CustomVariable()
        {
        }

        public CustomVariable(string name, string description, object value)
        {
            Name = name;
            Description = description;
            Value = value;
        }

        public string Name { get; }

        public string Description { get; }

        public object Value { get; }
    }
}
