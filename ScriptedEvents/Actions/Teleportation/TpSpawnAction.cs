using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Teleportation
{
    public class TpSpawnAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "TPSpawn";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Teleportation;

        /// <inheritdoc/>
        public string Description => $"Teleports players to the specified {nameof(SpawnLocationType)}.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(PlayerCollection), "The players to teleport", true),
            new Argument("spawn", typeof(SpawnLocationType), "The spawn to teleport to.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection players = (PlayerCollection)Arguments[0]!;
            SpawnLocationType rt = (SpawnLocationType)Arguments[1]!;

            foreach (Player ply in players)
            {
                if (ply.Role is not FpcRole || !ply.IsConnected) continue;
                ply.Teleport(rt, UnityEngine.Vector3.up);
            }

            return new(true);
        }
    }
}
