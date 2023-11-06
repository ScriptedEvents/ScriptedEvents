﻿namespace ScriptedEvents.Structures
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;

    public class PlayerCollection : IEnumerable
    {
        private List<Player> players;

        public PlayerCollection(List<Player> newPlayers, bool success = true, string message = "")
        {
            players = newPlayers ?? new();

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

        public List<Player> GetInnerList()
        {
            return players.ToList();
        }

        public Player[] GetArray()
        {
            return GetInnerList().ToArray();
        }
    }
}
