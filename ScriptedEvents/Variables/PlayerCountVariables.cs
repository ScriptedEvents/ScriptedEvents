﻿namespace ScriptedEvents.Variables.PlayerCount
{
#pragma warning disable SA1402 // File may only contain a single type.
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
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
            new PlayersAlive(),
            new PlayersDead(),
            new Staff(),
            new InRoom(),
            new NonePlayer(),
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
                    throw new ArgumentException(MsgGen.VariableArgCount(Name, new[] { "roomType" }));
                }

                if (Enum.TryParse<RoomType>(Arguments[0], out RoomType rt))
                {
                    return Player.Get(plr => plr.CurrentRoom.Type == rt);
                }
                else if (VariableSystem.TryGetVariable(Arguments[0], out IConditionVariable variable, out _, Source, false))
                {
                    if (variable is IStringVariable variableString && Enum.TryParse(variableString.Value, out RoomType rt2))
                    {
                        return Player.Get(plr => plr.CurrentRoom.Type == rt2);
                    }
                }

                throw new ArgumentException($"Provided value '{Arguments[0]}' is not a valid RoomType or the variable does not provide a RoomType.");
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
        public float Value => Player.List.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Enumerable.Empty<Player>();
    }
}
