using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Roles;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Teleportation
{
    public class TpDoorAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "TPDoor";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Teleportation;

        /// <inheritdoc/>
        public string Description => "Teleports players to the specified door.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to teleport", true),
            new Argument("door", typeof(DoorType), "The door type to teleport to. If there are multiple doors with the same doortype, a random door will be chosen for each player.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Player[] players = (Player[])Arguments[0]!;
            DoorType doorType = (DoorType)Arguments[1]!;

            foreach (Player ply in players)
            {
                ply.Teleport(Door.List.Where(d => d.Type == doorType).GetRandomValue());
            }

            return new(true);
        }
    }
}
