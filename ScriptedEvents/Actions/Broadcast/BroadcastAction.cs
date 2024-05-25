namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;

    public class BroadcastAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "BROADCAST";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Broadcast;

        /// <inheritdoc/>
        public string Description => "Broadcasts a message to specific player(s).";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(PlayerCollection), "The players to show.", true),
            new Argument("durationSeconds", typeof(float), "The duration of the message.", true),
            new Argument("message", typeof(string), "The message.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection players = (PlayerCollection)Arguments[0];
            float duration = (float)Arguments[1];

            string message = VariableSystemV2.ReplaceVariables(Arguments.JoinMessage(2), script);
            foreach (Player player in players)
            {
                player.Broadcast((ushort)duration, message);
            }

            return new(true);
        }
    }
}