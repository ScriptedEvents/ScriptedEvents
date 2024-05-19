namespace ScriptedEvents.Variables
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using ScriptedEvents.Variables.Interfaces;

    public class CustomPlayerVariable : IFloatVariable, IPlayerVariable
    {
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
        public CustomPlayerVariable(string name, string description, List<Player> value)
        {
            Name = name;
            Description = description;
            PlayerList = value;
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string Description { get; }

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => PlayerList;

        private List<Player> PlayerList { get; }

        /// <summary>
        /// Adds a range of players to the variable. Will ignore duplicates.
        /// </summary>
        /// <param name="player">The players to add.</param>
        public void Add(params Player[] player)
        {
            foreach (Player plr in player)
            {
                if (!PlayerList.Contains(plr))
                    PlayerList.Add(plr);
            }
        }

        /// <summary>
        /// Removes a range of players from the variable. Will ignore duplicates.
        /// </summary>
        /// <param name="player">The players to remove.</param>
        public void Remove(params Player[] player)
        {
            foreach (Player plr in player)
            {
                if (PlayerList.Contains(plr))
                    PlayerList.Remove(plr);
            }
        }
    }
}
