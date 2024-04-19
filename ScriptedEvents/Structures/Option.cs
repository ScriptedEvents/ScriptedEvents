namespace ScriptedEvents.Structures
{
    public readonly struct Option
    {
        public Option(string name, string description = null)
        {
            Name = name;
            Description = description ?? string.Empty;
        }

        public string Name { get; }

        public string Description { get; }
    }
}
