namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;

    public class CountdownAction : IScriptAction, IHelpInfo, ILongDescription
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
            new Argument("players", typeof(PlayerCollection), "The players to show the countdown to.", true),
            new Argument("durationSeconds", typeof(long), "The duration of the countdown.", true),
            new Argument("text", typeof(string), "The text to show on the broadcast.", true),
        };

        public string LongDescription => $@"Countdowns use the broadcast system. As such, players who are given a countdown cannot see any other broadcasts until the countdown concludes or is terminated.

The text of the broadcast will be formatted using the countdown_string Exiled config. If the text parameter is not provided, the text displayed will simply be 'Countdown'.";

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection players = (PlayerCollection)Arguments[0];

            long duration = (long)Arguments[1];

            string text = null;

            if (Arguments.Length > 2)
                text = VariableSystemV2.ReplaceVariables(Arguments.JoinMessage(2), script);

            foreach (Player ply in players)
                MainPlugin.GetModule<CountdownModule>().AddCountdown(ply, text, TimeSpan.FromSeconds(duration), script);

            return new(true);
        }
    }
}
