namespace ScriptedEvents.Variables.Interfaces
{
    using System.Collections.Generic;

    using Exiled.API.Features;

    /// <summary>
    /// Represents a variable that can be used to get a list of players.
    /// </summary>
    public interface IPlayerVariable : IVariable
    {
        /// <summary>
        /// Gets the list of players representing this variable.
        /// </summary>
        public IEnumerable<Player> Players { get; }
    }
}
