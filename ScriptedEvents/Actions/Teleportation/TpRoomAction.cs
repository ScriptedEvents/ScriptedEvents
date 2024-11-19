using System;
using System.Linq;
using System.Security.Policy;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using ScriptedEvents.API.Features.Exceptions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Teleportation
{
    public class TpRoomAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "TPRoom";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Teleportation;

        /// <inheritdoc/>
        public string Description => "Teleports players to the specified room center.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(PlayerCollection), "The players to teleport", true),
            new MultiTypeArgument(
                "room", 
                new[]
                {
                    typeof(RoomType),
                    typeof(ZoneType)
                }, 
                "The room to teleport to. Alternatively, a zone can be provided to teleport players to a random room in the zone (random for each player). Do NOT use Scp173 room!!!",
                true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection players = (PlayerCollection)Arguments[0]!;
            Action<Player> act = Arguments[1]! switch
            {
                RoomType roomType => p => p.Teleport(Room.List.Where(r => r.Type == roomType).GetRandomValue()),
                ZoneType zoneType => p => p.Teleport(Room.List.Where(r => r.Zone == zoneType).GetRandomValue()),
                _ => throw new ImpossibleException()
            };
            
            foreach (Player ply in players)
            {
                if (ply.Role is not FpcRole || !ply.IsConnected) continue;
                act(ply);
            }

            return new(true);
        }
    }
}
