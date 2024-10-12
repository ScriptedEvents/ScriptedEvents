namespace ScriptedEvents.Variables
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using ScriptedEvents.Variables.Interfaces;

    public class CustomPlayerVariable : IPlayerVariable
    {
        private readonly IEnumerable<Player> _players;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPlayerVariable"/> class.
        /// </summary>
        public CustomPlayerVariable()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPlayerVariable"/> class.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="description">The description of the variable.</param>
        /// <param name="value">The players to add to the variable.</param>
        public CustomPlayerVariable(string name, string description, IEnumerable<Player> value)
        {
            Name = name;
            Description = description;
            _players = value;
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string Description { get; }

        IEnumerable<Player> IPlayerVariable.Players => _players;

        public IEnumerable<Player> GetPlayers() => _players.Where(plr => plr is not null);
    }
}
