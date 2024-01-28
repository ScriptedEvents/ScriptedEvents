namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class CountdownAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "COUNTDOWN";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Broadcast;

        /// <inheritdoc/>
        public string Description => "Displays a countdown on the player(s) screens (using broadcasts).";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to show the countdown to.", true),
            new Argument("duration", typeof(int), "The duration of the countdown. Variables are supported.", true),
            new Argument("text", typeof(string), "The text to show on the broadcast. Variables are supported.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection players = (PlayerCollection)Arguments[0];

            long duration = (long)Arguments[1];

            string text = null;

            if (Arguments.Length > 2)
                text = VariableSystem.ReplaceVariables(Arguments.JoinMessage(2), script);

            foreach (Player ply in players)
                CountdownHelper.AddCountdown(ply, text, TimeSpan.FromSeconds(duration), script);

            return new(true);
        }
    }
}
