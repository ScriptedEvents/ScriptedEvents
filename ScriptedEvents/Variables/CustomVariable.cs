namespace ScriptedEvents.Variables
{
    using ScriptedEvents.Variables.Interfaces;

    public class CustomVariable : IStringVariable
    {
        public CustomVariable()
        {
        }

        public CustomVariable(string name, string description, string value)
        {
            Name = name;
            Description = description;
            Value = value;
        }

        public string Name { get; }

        public string Description { get; }

        public string Value { get; }
    }
}
