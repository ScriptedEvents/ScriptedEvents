namespace ScriptedEvents.Actions
{
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;
    using ScriptedEvents.Variables.Interfaces;
    using UnityEngine;

    public class PlayerVariableAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "GLOBALPLAYERVAR";

        /// <inheritdoc/>
        public string[] Aliases => new[] { "GPVAR" };

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Variable;

        /// <inheritdoc/>
        public string Description => "Allows manipulation of player variables.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("SET", "Saves a new player variable."),
                new("DEL", "Deletes a previously-saved player variable."),
                new("ADD", "Adds player(s) to an established player variable."),
                new("REMOVE", "Removes player(s) from an established player variable.")),
            new Argument("variableName", typeof(string), "The name of the variable.", true),
            new Argument("players", typeof(PlayerCollection), "The players. Not required if mode is 'DELETE', but required otherwise.", false),
            new Argument("max", typeof(int), "The maximum amount of players to save/add/remove. No effect if mode is 'DELETE'. Math is supported. (default: unlimited).", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string mode = Arguments[0].ToUpper();
            string varName = RawArguments[1];
            PlayerCollection players = null;

            int max = -1;

            if (Arguments.Length > 3)
            {
                string formula = VariableSystemV2.ReplaceVariables(Arguments.JoinMessage(3), script);

                if (!ConditionHelperV2.TryMath(formula, out MathResult result))
                {
                    return new(MessageType.NotANumberOrCondition, this, "max", formula, result);
                }

                if (result.Result < 0)
                {
                    return new(MessageType.LessThanZeroNumber, this, "max", result.Result);
                }

                max = Mathf.RoundToInt(result.Result);
            }

            if (mode != "DELETE")
            {
                // Todo: Need to find a better solution where the 'max' parameter is required
                // Math does not work inside of variables
                if (Arguments[2] is IPlayerVariable)
                {
                    if (!ScriptModule.TryGetPlayers(RawArguments[2], max, out players, script))
                        return new(false, players.Message);
                }

                if (!ScriptModule.TryGetPlayers(VariableSystemV2.ReplaceVariable(RawArguments[2], script), max, out players, script))
                    return new(false, players.Message);
            }

            switch (mode)
            {
                case "SET":
                    VariableSystemV2.DefineVariable(varName, "Script-defined variable", players.GetInnerList(), script);
                    return new(true);
                case "DEL":
                    if (VariableSystemV2.DefinedPlayerVariables.ContainsKey(varName))
                    {
                        VariableSystemV2.DefinedPlayerVariables.Remove(varName);
                        break;
                    }
                    else
                    {
                        return new(false, $"'{varName}' is not a valid variable.");
                    }

                case "ADD":
                    if (VariableSystemV2.DefinedPlayerVariables.TryGetValue(varName, out CustomPlayerVariable var))
                    {
                        var.Add(players.GetArray());
                        break;
                    }
                    else
                    {
                        return new(false, $"'{varName}' is not a valid variable.");
                    }

                case "REMOVE":
                    if (VariableSystemV2.DefinedPlayerVariables.TryGetValue(varName, out CustomPlayerVariable var2))
                    {
                        var2.Remove(players.GetArray());
                        break;
                    }
                    else
                    {
                        return new(false, $"'{varName}' is not a valid variable.");
                    }
            }

            return new(true);
        }
    }
}