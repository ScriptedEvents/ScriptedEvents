namespace ScriptedEvents.Variables.PlayerCount
{
#pragma warning disable SA1402 // File may only contain a single type.
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;
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
            new PlayersDead(),
            new Humans(),
            new Staff(),
            new NonePlayer(),
            new Scp096Targets(),
            new Scp173Observers(),
            new StoredPlayers(),
        };
    }

    public class AllPlayers : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{PLAYERS}";

        /// <inheritdoc/>
        public string Description => "The amount of players in the server.";

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.List;
    }

    public class AllNpcs : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{NPCS}";

        /// <inheritdoc/>
        public string Description => "The amount of NPCs in the server.";

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Npc.List;
    }

    public class PlayersAlive : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{PLAYERSALIVE}";

        /// <inheritdoc/>
        public string Description => "The amount of alive players in the server.";

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.List.Where(p => p.IsAlive);
    }

    public class PlayersDead : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{PLAYERSDEAD}";

        /// <inheritdoc/>
        public string Description => "The amount of dead players in the server.";

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.List.Where(p => p.IsDead);
    }

    public class Humans : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{HUMANS}";

        /// <inheritdoc/>
        public string Description => "The amount of humans that are currently alive.";

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.List.Where(p => p.IsHuman);
    }

    public class Staff : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{SERVERSTAFF}";

        /// <inheritdoc/>
        public string Description => "The amount of staff on the server (RA access)";

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(player => player.RemoteAdminAccess);
    }

    public class NonePlayer : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{NONE}";

        /// <inheritdoc/>
        public string Description => "Will always be an empty variable with no players.";

        /// <inheritdoc/>
        public float Value => 0;

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Enumerable.Empty<Player>();
    }

    public class Scp096Targets : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{SCP096TARGETS}";

        /// <inheritdoc/>
        public string Description => "The amount of players that are being targeted by an SCP-096.";

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players
        {
             get
             {
                List<Player> list = new();
                foreach (Player ply in Player.Get(PlayerRoles.RoleTypeId.Scp096))
                {
                    list.AddRange((ply.Role as Scp096Role).Targets);
                }

                return list;
             }
        }
    }

    public class Scp173Observers : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{SCP173OBSERVERS}";

        /// <inheritdoc/>
        public string Description => "The amount of players that are looking at SCP-173.";

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players
        {
            get
            {
                List<Player> list = new();
                foreach (Player ply in Player.Get(PlayerRoles.RoleTypeId.Scp173))
                {
                    list.AddRange((ply.Role as Scp173Role).ObservingPlayers);
                }

                return list;
            }
        }
    }

    public class StoredPlayers : IPlayerVariable, IArgumentVariable, INeedSourceVariable
    {
        /// <inheritdoc/>
        public string Name => "{STOREDPLAYERS}";

        /// <inheritdoc/>
        public string Description => "Retrieves the player variable from the variable storage.";

        /// <inheritdoc/>
        public IEnumerable<Player> Players
        {
            get
            {
                string playersAsString = VariableStorage.Read(RawArguments[0]);
                List<Player> list = new();

                if (ScriptModule.TryGetPlayers(playersAsString, null, out PlayerCollection collection, Source))
                {
                    return list;
                }

                return collection.ToList();
            }
        }

        public string[] RawArguments { get; set; }

        public object[] Arguments { get; set; }

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("variable", typeof(string), "The variable name to retrieve players from.", true),
        };

        public Script Source { get; set; }
    }
}
