namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Remoting.Messaging;
    using Exiled.API.Features;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Handlers;
    using UnityEngine;

    public class SizeAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "SIZE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Sets all players to the given size.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(List<Player>), "The players to rescale.", true),
            new Argument("size X", typeof(float), "The X size to put on the player.", true),
            new Argument("size Y", typeof(float), "The Y size to put on the player.", true),
            new Argument("size Z", typeof(float), "The Z size to put on the player.", true),
            new Argument("max", typeof(int), "The maximum amount of players to rescale (default: unlimited).", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script scr)
        {
            if (Arguments.Length < 4) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!float.TryParse(Arguments.ElementAt(1), out float x))
                return new(MessageType.NotANumber, this, "X", Arguments.ElementAt(1));

            if (!float.TryParse(Arguments.ElementAt(2), out float y))
                return new(MessageType.NotANumber, this, "Y", Arguments.ElementAt(2));

            if (!float.TryParse(Arguments.ElementAt(3), out float z))
                return new(MessageType.NotANumber, this, "Z", Arguments.ElementAt(3));

            int max = -1;

            if (Arguments.Length > 4)
            {
                string formula = ConditionVariables.ReplaceVariables(string.Join(" ", Arguments.Skip(4)));

                if (!ConditionHelper.TryMath(formula, out MathResult result))
                {
                    return new(false, $"Invalid max condition provided! Condition: {formula} Error type: '{result.Exception.GetType().Name}' Message: '{result.Message}'.");
                }

                if (result.Result < 0)
                {
                    return new(false, "A negative number cannot be used as the max argument of the SIZE action.");
                }
            }

            if (!ScriptHelper.TryGetPlayers(Arguments[0], max, out Player[] plys))
                return new(false, "No players matching the criteria were found.");

            foreach (Player player in plys)
                player.Scale = new(x, y, z);

            return new(true);
        }
    }
}
