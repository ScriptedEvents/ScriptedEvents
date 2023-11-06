namespace ScriptedEvents.Variables
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

        public CustomPlayerVariable(string name, string description, List<Player> value)
        {
            Name = name;
            Description = description;
            PlayerList = value;
        }

        public string Name { get; }

        public string Description { get; }

        public float Value => Players.Count();

        public List<Player> PlayerList { get; set; }

        public IEnumerable<Player> Players => PlayerList;
    }
}
