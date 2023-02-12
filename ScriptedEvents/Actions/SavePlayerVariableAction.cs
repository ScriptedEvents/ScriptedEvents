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
    using ScriptedEvents.Variables;
    using UnityEngine;

    public class SavePlayerVariableAction : IScriptAction, IHelpInfo
    {
        public string Name => "SAVEPLAYERVARIABLE";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Saves a new player variable. Saved variables can be used in ANY script, and are reset when the round ends.";

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("variableName", typeof(string), "The name of the new variable. Braces will be added automatically if not provided.", true),
            new Argument("players", typeof(List<Player>), "The players to save as the new variable.", true),
            new Argument("max", typeof(int), "The maximum amount of players to save in this variable (default: unlimited).", false),
        };

        public ActionResponse Execute(Script scr)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            int max = -1;

            if (Arguments.Length > 2)
            {
                string formula = ConditionVariables.ReplaceVariables(string.Join(" ", Arguments.Skip(2)));

                if (!ConditionHelper.TryMath(formula, out MathResult result))
                {
                    return new(MessageType.NotANumberOrCondition, this, "max", formula, result);
                }

                if (result.Result < 0)
                {
                    return new(MessageType.LessThanZeroNumber, this, "max", result.Result);
                }

                max = Mathf.RoundToInt(result.Result);
            }

            List<Player> plys;

            if (!ScriptHelper.TryGetPlayers(Arguments[1], max, out plys))
                return new(MessageType.NoPlayersFound, this, "players");

            PlayerVariables.DefineVariable(Arguments[0], plys);

            return new(true);
        }
    }
}
