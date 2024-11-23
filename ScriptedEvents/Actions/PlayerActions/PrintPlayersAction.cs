using System;
using Exiled.API.Features;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.PlayerActions
{
    public class PrintPLayersAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "AdvPrint";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => "Prints a message in the specified players' console.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "Players to affect.", true),
            new Argument("message", typeof(string), "The message content.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Player[] players = (Player[])Arguments[0]!;
            string message = Arguments.JoinMessage(1).Replace("\\n", "\n");

            foreach (Player player in players)
            {
                player.SendConsoleMessage(message, "green");
            }

            return new(true);
        }
    }
}