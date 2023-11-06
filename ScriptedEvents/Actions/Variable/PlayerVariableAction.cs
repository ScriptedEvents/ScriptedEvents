namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;
    using UnityEngine;

    public class PlayerVariableAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "PLAYERVAR";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Variable;

        /// <inheritdoc/>
        public string Description => "Allows manipulation of player variables.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("mode", typeof(string), "The action to perform (SAVE/DELETE/ADD/REMOVE).", true),
            new Argument("variableName", typeof(string), "The name of the variable.", true),
            new Argument("players", typeof(Player[]), "The players. Not required if mode is 'DELETE', but required otherwise.", true),
            new Argument("max", typeof(int), "The maximum amount of players to save/add/remove. No effect if mode is 'DELETE'. Math and variables are supported. (default: unlimited).", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2)
                return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            string mode = Arguments[0].ToUpper();
            string varName = Arguments[1];
            PlayerCollection players = null;

            int max = -1;

            if (Arguments.Length > 3)
            {
                string formula = VariableSystem.ReplaceVariables(string.Join(" ", Arguments.Skip(3)), script);

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

            if (mode != "DELETE")
            {
                if (Arguments.Length < 3)
                    return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

                if (!ScriptHelper.TryGetPlayers(Arguments[2], max, out players, script))
                    return new(false, players.Message);
            }

            switch (mode)
            {
                case "SAVE":
                    VariableSystem.DefineVariable(varName, "Script-defined variable", players.GetInnerList());
                    return new(true);
                case "DELETE":
                    if (VariableSystem.DefinedPlayerVariables.ContainsKey(varName))
                    {
                        VariableSystem.RemoveVariable(varName);
                        return new(true);
                    }
                    else
                    {
                        return new(false, $"'{varName}' is not a valid variable.");
                    }

                case "ADD":
                    if (VariableSystem.DefinedPlayerVariables.TryGetValue(varName, out CustomPlayerVariable var))
                    {
                        var.Add(players.GetArray());
                        return new(true);
                    }
                    else
                    {
                        return new(false, $"'{varName}' is not a valid variable.");
                    }

                case "REMOVE":
                    if (VariableSystem.DefinedPlayerVariables.TryGetValue(varName, out CustomPlayerVariable var2))
                    {
                        var2.Remove(players.GetArray());
                        return new(true);
                    }
                    else
                    {
                        return new(false, $"'{varName}' is not a valid variable.");
                    }
            }

            return new(MessageType.InvalidOption, this, "mode", "SAVE/DELETE/ADD/REMOVE");
        }
    }
}