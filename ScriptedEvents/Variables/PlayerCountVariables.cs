namespace ScriptedEvents.Variables.PlayerCount
{
    using System.Collections.Generic;
#pragma warning disable SA1402 // File may only contain a single type.
    using System.Linq;

    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Variables.Interfaces;

    public class PlayerCountVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Players";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new AllPlayers(),
            new PlayersAlive(),
            new PlayersDead(),
            new Staff(),
        };
    }

    public class AllPlayers : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{PLAYERS}";

        /// <inheritdoc/>
        public string Description => "The total amount of players in the server.";

        /// <inheritdoc/>
        public float Value => Player.List.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.List;
    }

    public class PlayersAlive : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{PLAYERSALIVE}";

        /// <inheritdoc/>
        public string Description => "The total amount of alive players in the server.";

        /// <inheritdoc/>
        public float Value => Player.List.Count(p => p.IsAlive);

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.List.Where(p => p.IsAlive);
    }

    public class PlayersDead : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{PLAYERSDEAD}";

        /// <inheritdoc/>
        public string Description => "The total amount of dead players in the server.";

        /// <inheritdoc/>
        public float Value => Player.List.Count(p => p.IsDead);

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.List.Where(p => p.IsDead);
    }

    public class Staff : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{SERVERSTAFF}";

        /// <inheritdoc/>
        public string Description => "Gets all of the staff on the server (RA access)";

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(player => player.RemoteAdminAccess);
    }
}
