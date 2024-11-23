using System;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;
using UnityEngine;

namespace ScriptedEvents.Actions.Teleportation
{
    public class TpxAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "TPPos";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Teleportation;

        /// <inheritdoc/>
        public string Description => "Teleports players to the specified X, Y, Z coordinates.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to teleport", true),
            new Argument("X", typeof(float), "The X-coordinate to teleport to.", true),
            new Argument("Y", typeof(float), "The Y-coordinate to teleport to.", true),
            new Argument("Z", typeof(float), "The Z-coordinate to teleport to.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Player[] players = (Player[])Arguments[0]!;
            float x = (float)Arguments[1]!;
            float y = (float)Arguments[2]!;
            float z = (float)Arguments[3]!;

            Vector3 vz = new(x, y, z);

            foreach (Player ply in players)
            {
                if (ply.Role is not FpcRole || !ply.IsConnected) continue;
                ply.Teleport(vz);
            }

            return new(true);
        }
    }
}
