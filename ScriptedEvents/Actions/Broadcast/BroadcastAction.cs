namespace ScriptedEvents.Actions.Broadcast
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.Enums;
    using ScriptedEvents.Interfaces;
    using ScriptedEvents.Structures;

    public class BroadcastAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "Broadcast";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Broadcast;

        /// <inheritdoc/>
        public string Description => "Broadcasts a message to specified players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(PlayerCollection), "The players to show.", true),
            new Argument("duration", typeof(TimeSpan), "The duration of the message.", true),
            new Argument("message", typeof(string), "The message.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            var players = (PlayerCollection)Arguments[0];
            var duration = (TimeSpan)Arguments[1];
            var message = (string)Arguments[2];

            foreach (Player player in players)
            {
                player.Broadcast((ushort)duration.Seconds, message);
            }

            return new(true);
        }
    }
}