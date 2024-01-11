namespace ScriptedEvents.Structures
{
    public struct Contributor
    {
        public string Name;
        public string Role;

        public Contributor(string name, string role)
        {
            Name = name;
            Role = role;
        }

        public override string ToString()
        {
            return $"{Name} - {Role}";
        }
    }
}
