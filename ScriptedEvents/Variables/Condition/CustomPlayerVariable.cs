namespace ScriptedEvents.Variables.Condition
{
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;

    using ScriptedEvents.Variables.Interfaces;

    public class CustomPlayerVariable : IFloatVariable, IPlayerVariable
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

        public float Value => Players.Count();

        public IEnumerable<Player> Players { get; }
    }
}
