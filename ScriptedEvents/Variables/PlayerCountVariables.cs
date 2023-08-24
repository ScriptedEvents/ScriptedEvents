namespace ScriptedEvents.Variables.PlayerCount
{
    using System;
    using System.Collections.Generic;
#pragma warning disable SA1402 // File may only contain a single type.
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using PlayerRoles;
    using ScriptedEvents.API.Enums;
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
            new PlayersAlive(),
            new PlayersDead(),
            new Staff(),
            new InRoom(),
        };
    }

    public class AllPlayers : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{PLAYERS}";

        /// <inheritdoc/>
        public string Description => "The amount of players in the server.";

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
        public string Description => "The amount of alive players in the server.";

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
        public string Description => "The amount of dead players in the server.";

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
        public string[] Arguments { get; set; }

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
                if (Arguments.Length < 1)
                {
                    return Array.Empty<Player>();
                }

                if (Enum.TryParse<RoomType>(Arguments[0], out RoomType rt))
                {
                    return Player.Get(plr => plr.CurrentRoom.Type == rt);
                }
                else if (VariableSystem.TryGetVariable(Arguments[0], out IConditionVariable variable, out _, Source))
                {
                    if (variable is IStringVariable variableString && Enum.TryParse(variableString.Value, out RoomType rt2))
                    {
                        return Player.Get(plr => plr.CurrentRoom.Type == rt2);
                    }
                }

                return Array.Empty<Player>();
            }
        }
    }
}
