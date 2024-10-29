using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions.Map
{
    using System;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.API.Features.Pickups;

    using PlayerRoles;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;

    public class CleanupRagdollsAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "CLEANUPRAGDOLLS";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => "Cleans up ragdolls of specified players from the map.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(PlayerCollection), "The players which ragdolls are to be removed. Dont provide this argument if you want to remove every ragdoll from the map.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length == 0)
            {
                foreach (var ragdoll in Ragdoll.List)
                {
                    ragdoll.Destroy();
                }

                return new(true);
            }

            foreach (var player in ((PlayerCollection)Arguments[0]).GetArray())
            {
                foreach (var ragdoll in Ragdoll.Get(player))
                {
                    ragdoll.Destroy();
                }
            }

            return new(true);
        }
    }
}