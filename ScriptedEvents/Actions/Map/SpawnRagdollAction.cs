using System;
using Exiled.API.Features;
using PlayerRoles;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;
using UnityEngine;

namespace ScriptedEvents.Actions.Map
{
    public class SpawnRagdollAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "SpawnRagdoll";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => "Spawns a ragdoll at the provided XYZ coordinates.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("x", typeof(float), "The X coordinate.", true),
            new Argument("y", typeof(float), "The Y coordinate.", true),
            new Argument("z", typeof(float), "The Z coordinate.", true),
            new Argument("role", typeof(RoleTypeId), "Character of the ragdoll.", true),
            new Argument("name", typeof(string), "Name of the ragdoll.", true),
            new Argument("deathReason", typeof(string), "The death reason.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Vector3 pos = new(
                (float)Arguments[0]!,
                (float)Arguments[1]!,
                (float)Arguments[2]!
            );
            
            Ragdoll.CreateAndSpawn(
                (RoleTypeId)Arguments[3]!, 
                (string)Arguments[4]!, 
                (string)Arguments[5]!, 
                pos, default
            );

            return new(true);
        }
    }
}
