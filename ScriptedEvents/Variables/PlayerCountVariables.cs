namespace ScriptedEvents.Variables.PlayerCount
{
#pragma warning disable SA1402 // File may only contain a single type.
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using ScriptedEvents.Variables.Interfaces;

    public class PlayerCountVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Players";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new AllPlayers(),
            new AllNpcs(),
            new PlayersAlive(),
            new Humans(),
            new Staff(),
            new NonePlayer(),
        };
    }

    public class AllPlayers : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "@PLAYERS";

        /// <inheritdoc/>
        public string Description => "Returns all players on the server.";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.List;
    }

    public class AllNpcs : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "@NPCS";

        /// <inheritdoc/>
        public string Description => "Returns all NPCs on the server.";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Npc.List;
    }

    public class PlayersAlive : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "@PLAYERSALIVE";

        /// <inheritdoc/>
        public string Description => "Returns all alive players on the server.";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.List.Where(p => p.IsAlive);
    }

    public class Humans : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "@HUMANS";

        /// <inheritdoc/>
        public string Description => "Returns all humans that are currently alive (humans as in a human role, not not NPCs).";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.List.Where(p => p.IsHuman);
    }

    public class Staff : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "@STAFF";

        /// <inheritdoc/>
        public string Description => "Returns all staff on the server (RA access)";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(player => player.RemoteAdminAccess);
    }

    public class NonePlayer : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "@NONE";

        /// <inheritdoc/>
        public string Description => "Returns an empty player variable.";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Enumerable.Empty<Player>();
    }
}
