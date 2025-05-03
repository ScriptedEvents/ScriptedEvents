using System;
using ScriptedEvents.API.Enums;
using ScriptedEvents.API.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Broadcast
{
    public class ClearBroadcastsAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "CLEARBROADCASTS";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Broadcast;

        /// <inheritdoc/>
        public string Description => "Clears all broadcasts for specified player(s).";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(PlayerCollection), "The players to affect.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            foreach (Exiled.API.Features.Player player in (PlayerCollection)Arguments[0])
                player.ClearBroadcasts();

            return new(true);
        }
    }
}