namespace ScriptedEvents.Actions.Broadcast
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Enums;
    using ScriptedEvents.Interfaces;
    using ScriptedEvents.Structures;

    public class HintAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "Hint";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Broadcast;

        /// <inheritdoc/>
        public string Description => "Broadcasts a hint to specified players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to show the hint for.", true),
            new Argument("duration", typeof(TimeSpan), "The duration of the hint.", true),
            new Argument("message", typeof(string), "The hint content.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            var players = (Player[])Arguments[0]!;
            var duration = (TimeSpan)Arguments[1]!;
            var message = (string)Arguments[2]!;

            foreach (Player plr in players)
            {
                plr.ShowHint(message, duration.Seconds);
            }

            return new(true);
        }
    }
}