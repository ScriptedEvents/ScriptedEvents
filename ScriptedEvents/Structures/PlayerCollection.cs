namespace ScriptedEvents.Structures
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;

    /// <summary>
    /// Indicates a read-only collection of players.
    /// </summary>
    public class PlayerCollection : IEnumerable<Player>
    {
        private readonly List<Player> players;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerCollection"/> class.
        /// </summary>
        /// <param name="newPlayers">The list of players.</param>
        /// <param name="success">Whether or not the player retrieval was successful.</param>
        /// <param name="message">The error message, if <paramref name="success"/> is <see langword="false"/>.</param>
        public PlayerCollection(List<Player> newPlayers, bool success = true, string message = "")
        {
            players = newPlayers ?? new();

            Success = success;
            Message = message;
        }

        /// <summary>
        /// Gets the length of the collection.
        /// </summary>
        public int Length => players.Count();

        /// <summary>
        /// Gets a value indicating whether or not the player retrieval was successful.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Gets the error message, if <see cref="Success"/> is <see langword="false"/>.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets a player at a specific index in the list.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The player.</returns>
        public Player this[int index]
            => players[index];

        /// <inheritdoc/>
        public IEnumerator<Player> GetEnumerator()
        {
            return players.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return players.GetEnumerator();
        }

        /// <summary>
        /// Returns the internal list of players.
        /// </summary>
        /// <returns>A list of players.</returns>
        public List<Player> GetInnerList()
        {
            return players;
        }

        /// <summary>
        /// Returns the internal array of players.
        /// </summary>
        /// <returns>An array of players.</returns>
        public Player[] GetArray()
        {
            return GetInnerList().ToArray();
        }
    }
}
