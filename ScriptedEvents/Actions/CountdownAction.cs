namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;

    using Exiled.API.Features;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class CountdownAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "COUNTDOWN";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

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
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!ScriptHelper.TryGetPlayers(Arguments[0], null, out Player[] players, script))
                return new(MessageType.NoPlayersFound, this, "players");

            if (!VariableSystem.TryParse(Arguments[1], out int duration, script))
                return new(MessageType.NotANumber, this, "duration", Arguments[1]);

            string text = null;

            if (Arguments.Length > 2)
                text = VariableSystem.ReplaceVariables(string.Join(" ", Arguments.Skip(2)), script);

            foreach (Player ply in players)
                CountdownHelper.AddCountdown(ply, text, TimeSpan.FromSeconds(duration), script);

            return new(true);
        }
    }
}
