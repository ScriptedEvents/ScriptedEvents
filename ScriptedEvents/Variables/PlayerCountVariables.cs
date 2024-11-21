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
        public string Name => "Players";

        /// <inheritdoc/>
        public string Description => "Returns all players on the server.";

        /// <inheritdoc/>
        IEnumerable<Player> IPlayerVariable.Players => Player.List;

        public IEnumerable<Player> GetPlayers()
        {
            return ((IPlayerVariable)this).Players.Where(plr => plr is not null);
        }
    }

    public class AllNpcs : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "NPCs";

        /// <inheritdoc/>
        public string Description => "Returns all NPCs on the server.";

        /// <inheritdoc/>
        IEnumerable<Player> IPlayerVariable.Players => Npc.List;

        public IEnumerable<Player> GetPlayers()
        {
            return ((IPlayerVariable)this).Players.Where(plr => plr is not null);
        }
    }

    public class PlayersAlive : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "PlayersAlive";

        /// <inheritdoc/>
        public string Description => "Returns all alive players on the server.";

        /// <inheritdoc/>
        IEnumerable<Player> IPlayerVariable.Players => Player.List.Where(p => p.IsAlive);

        public IEnumerable<Player> GetPlayers()
        {
            return ((IPlayerVariable)this).Players.Where(plr => plr is not null);
        }
    }

    public class Humans : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "Humans";

        /// <inheritdoc/>
        public string Description => "Returns all humans that are currently alive (humans as in a human role, not not NPCs).";

        /// <inheritdoc/>
        IEnumerable<Player> IPlayerVariable.Players => Player.List.Where(p => p.IsHuman);

        public IEnumerable<Player> GetPlayers()
        {
            return ((IPlayerVariable)this).Players.Where(plr => plr is not null);
        }
    }

    public class Staff : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "ServerStaff";

        /// <inheritdoc/>
        public string Description => "Returns all staff on the server (RA access)";

        /// <inheritdoc/>
        IEnumerable<Player> IPlayerVariable.Players => Player.Get(player => player.RemoteAdminAccess);

        public IEnumerable<Player> GetPlayers()
        {
            return ((IPlayerVariable)this).Players.Where(plr => plr is not null);
        }
    }

    public class NonePlayer : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "NonePlr";

        /// <inheritdoc/>
        public string Description => "Returns an empty player variable.";

        /// <inheritdoc/>
        IEnumerable<Player> IPlayerVariable.Players => Enumerable.Empty<Player>();

        public IEnumerable<Player> GetPlayers()
        {
            return ((IPlayerVariable)this).Players.Where(plr => plr is not null);
        }
    }
}
