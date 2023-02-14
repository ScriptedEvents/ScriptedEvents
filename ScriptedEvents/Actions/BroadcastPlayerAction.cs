namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Handlers;

    public class BroadcastPlayerAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "BROADCASTPLAYER";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Broadcasts a message to specific player(s).";

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

            if (!ScriptHelper.TryGetPlayers(Arguments[0], null, out Player[] players))
            {
                return new(MessageType.NoPlayersFound, this, "players");
            }

            if (!float.TryParse(Arguments[1], out float duration))
            {
                return new(MessageType.NotANumber, this, "duration", Arguments[0]);
            }

            string message = string.Join(" ", Arguments.Skip(2).Select(arg => ConditionVariables.ReplaceVariables(arg)));
            foreach (Player player in players)
            {
                player.Broadcast((ushort)duration, message);
            }

            return new(true);
        }
    }
}