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
            playerList = value;
        }

        private List<Player> playerList { get; }

        public string Name { get; }

        public string Description { get; }

        public float Value => Players.Count();

        public IEnumerable<Player> Players => playerList;

        public void Add(params Player[] player)
        {
            foreach (Player plr in player)
            {
                if (!playerList.Contains(plr))
                    playerList.Add(plr);
            }
        }

        public void Remove(params Player[] player)
        {
            foreach (Player plr in player)
            {
                if (playerList.Contains(plr))
                    playerList.Remove(plr);
            }
        }
    }
}
