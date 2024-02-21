namespace ScriptedEvents.Structures
{
    using System.Collections.Generic;

    using Exiled.API.Features;

    /// <summary>
    /// Represents a disable rule that only affects certain players.
    /// </summary>
    public readonly struct PlayerDisable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerDisable"/> struct.
        /// </summary>
        /// <param name="key">The key of the disable rule.</param>
        /// <param name="players">The affected players.</param>
        public PlayerDisable(string key, List<Player> players)
        {
            Key = key;
            Players = players;
        }

        /// <summary>
        /// Gets the key of the rule.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets a list of players affected by this disable rule.
        /// </summary>
        public List<Player> Players { get; }
    }
}
