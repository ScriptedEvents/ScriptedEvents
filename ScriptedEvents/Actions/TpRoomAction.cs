namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;

    public class TpRoomAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "TPROOM";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => "Teleports players to the specified room center.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to teleport", true),
            new Argument("room", typeof(RoomType), "The room to teleport to. Alternatively, a zone can be provided to teleport players to a random room in the zone (random for each player). Do NOT use Scp173 room!!!", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!ScriptHelper.TryGetPlayers(Arguments[0], null, out Player[] players, script))
                return new(MessageType.NoPlayersFound, this, "players");

            if (Enum.TryParse(Arguments[1], true, out ZoneType zt))
            {
                List<Room> validRooms = Room.List.Where(r => r.Zone.HasFlag(zt) && r.Type is not RoomType.Lcz173 && r.Type is not RoomType.Hcz079).ToList();
                foreach (Player ply in players)
                {
                    validRooms.ShuffleList();
                    if (ply.IsDead || !ply.IsConnected) continue;
                    ply.Teleport(validRooms[UnityEngine.Random.Range(0, validRooms.Count)]);
                }

                return new(true);
            }

            if (!Enum.TryParse(Arguments[1], true, out RoomType rt))
                return new(false, $"Invalid room: {Arguments[1]}");

            foreach (Player ply in players)
            {
                if (ply.Role is not FpcRole || !ply.IsConnected) continue;
                ply.Teleport(rt);
            }

            return new(true);
        }
    }
}
