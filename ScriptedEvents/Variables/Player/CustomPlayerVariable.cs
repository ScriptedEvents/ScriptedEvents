namespace ScriptedEvents.Variables.Condition
{
#pragma warning disable SA1402 // File may only contain a single type
    using System.Collections.Generic;

    using Exiled.API.Features;

    using ScriptedEvents.Variables.Interfaces;

    public class CustomPlayerVariable : IPlayerVariable
    {
        public CustomPlayerVariable()
        {
        }

        public CustomPlayerVariable(string name, string description, IEnumerable<Player> value)
        {
            Name = name;
            Description = description;
            Players = value;
        }

        public string Name { get; }

        public string Description { get; }

        public IEnumerable<Player> Players { get; }
    }
}
