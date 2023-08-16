namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class HintPlayerAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "HINTPLAYER";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Broadcast;

        /// <inheritdoc/>
        public string Description => "Broadcasts a hint to specific player(s).";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(List<Player>), "The players to show. Variables are supported.", true),
            new Argument("duration", typeof(float), "The duration of the message. Variables & Math are NOT supported.", true),
            new Argument("message", typeof(string), "The message. Variables are supported.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!ScriptHelper.TryGetPlayers(Arguments[0], null, out Player[] players, script))
            {
                return new(MessageType.NoPlayersFound, this, "players");
            }

            if (!float.TryParse(Arguments[1], out float duration))
            {
                return new(MessageType.NotANumber, this, "duration", Arguments[0]);
            }

            string message = string.Join(" ", Arguments.Skip(2).Select(arg => VariableSystem.ReplaceVariables(arg, script)));
            foreach (Player player in players)
            {
                player.ShowHint(message, duration);
            }

            return new(true);
        }
    }
}