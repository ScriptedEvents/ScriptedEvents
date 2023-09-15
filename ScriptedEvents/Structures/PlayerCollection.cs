namespace ScriptedEvents.Structures
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;

    public class PlayerCollection : IEnumerable
    {
        private IEnumerable<Player> players;

        public PlayerCollection(IEnumerable<Player> newPlayers, bool success = true, string message = "")
        {
            players = newPlayers ?? Array.Empty<Player>();

            Success = success;
            Message = message;
        }

        public int Length => players.Count();

        public bool Success { get; }

        public string Message { get; }

        public IEnumerator<Player> GetEnumerator()
        {
            return players.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return players.GetEnumerator();
        }

        public Player[] GetInnerArray()
        {
            return players.ToArray();
        }
    }
}
