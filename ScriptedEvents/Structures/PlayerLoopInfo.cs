namespace ScriptedEvents.Structures
{
    using System.Collections.Generic;

    using Exiled.API.Features;

    /// <summary>
    /// Contains information about the current loop comperhension.
    /// </summary>
    public class PlayerLoopInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerLoopInfo"/> class.
        /// </summary>
        /// <param name="line">The line at which the loop is occuring.</param>
        /// <param name="playersToLoopThrough">The players that are yet to be looped over.</param>
        public PlayerLoopInfo(int line, List<Player> playersToLoopThrough)
        {
            Line = line;
            PlayersToLoopThrough = playersToLoopThrough;
        }

        /// <summary>
        /// Gets or sets the name of the action.
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// Gets or sets the aliases of the action.
        /// </summary>
        public List<Player> PlayersToLoopThrough { get; set; }
    }
}
