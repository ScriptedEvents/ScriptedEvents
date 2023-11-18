namespace ScriptedEvents.Structures
{
    using System.Collections.Generic;

    using Exiled.API.Features;

    public struct PlayerDisable
    {
        public PlayerDisable(string key, List<Player> players)
        {
            Key = key;
            Players = players;
        }

        public string Key { get; }

        public List<Player> Players { get; }
    }
}
