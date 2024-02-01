namespace ScriptedEvents.Variables.PlayerCount
{
#pragma warning disable SA1402 // File may only contain a single type.
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using ScriptedEvents.API.Features;
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
            new InRoom(),
            new NonePlayer(),
            new Scp096Targets(),
            new Scp173Observers(),
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

    public class InRoom : IFloatVariable, IArgumentVariable, IPlayerVariable, INeedSourceVariable
    {
        /// <inheritdoc/>
        public string Name => "{INROOM}";

        /// <inheritdoc/>
        public string Description => "The amount of players in the specified room.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("roomType", typeof(RoomType), "The room to filter by.", false),
        };

        /// <inheritdoc/>
        public Script Source { get; set; }

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players
        {
            get
            {
                RoomType rt = (RoomType)Arguments[0];
                return Player.Get(plr => plr.CurrentRoom.Type == rt);
            }
        }
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
}
