namespace ScriptedEvents.Actions.Broadcast
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Interfaces;
    using ScriptedEvents.Structures;

    public class ClearBroadcastsAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "ClearBroadcasts";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Broadcast;

        /// <inheritdoc/>
        public string Description => "Clears broadcasts for specified players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(PlayerCollection), "The players to affect.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            foreach (Player player in (PlayerCollection)Arguments[0])
                player.ClearBroadcasts();

            return new(true);
        }
    }
}