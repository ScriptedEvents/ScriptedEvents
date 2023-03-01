namespace ScriptedEvents.Variables.Condition
{
    using ScriptedEvents.Variables.Interfaces;

    public class CustomVariable : IObjectVariable
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
