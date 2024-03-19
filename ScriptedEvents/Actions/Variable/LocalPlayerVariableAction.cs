namespace ScriptedEvents.Actions
{
    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    using UnityEngine;

    public class LocalPlayerVariableAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "LOCALPLAYERVAR";

        /// <inheritdoc/>
        public string[] Aliases => new[] { "LOCALPVAR" };

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Variable;

        /// <inheritdoc/>
        public string Description => "Allows manipulation of player variables accessible in this script only..";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("mode", typeof(string), "The action to perform (SAVE/DELETE/ADD/REMOVE).", true),
            new Argument("variableName", typeof(string), "The name of the variable.", true),
            new Argument("players", typeof(Player[]), "The players. Not required if mode is 'DELETE', but required otherwise.", false),
            new Argument("max", typeof(int), "The maximum amount of players to save/add/remove. No effect if mode is 'DELETE'. Math and variables are supported. (default: unlimited).", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string mode = ((string)Arguments[0]).ToUpper();
            string varName = RawArguments[1];
            PlayerCollection players = null;

            int max = -1;

            if (Arguments.Length > 3)
            {
                string formula = VariableSystem.ReplaceVariables(Arguments.JoinMessage(3), script);

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
                if (!ScriptHelper.TryGetPlayers(RawArguments[2], max, out players, script))
                    return new(false, players.Message);
            }

            switch (mode)
            {
                case "SAVE":
                    VariableSystem.DefineVariable(varName, "Script-defined variable", players.GetInnerList(), script, true);
                    return new(true);
                case "DELETE":
                    if (script.UniquePlayerVariables.ContainsKey(varName))
                    {
                        script.UniquePlayerVariables.Remove(varName);
                        return new(true);
                    }
                    else
                    {
                        return new(false, $"'{varName}' is not a valid variable.");
                    }

                case "ADD":
                    if (script.UniquePlayerVariables.TryGetValue(varName, out CustomPlayerVariable var))
                    {
                        var.Add(players.GetArray());
                        return new(true);
                    }
                    else
                    {
                        return new(false, $"'{varName}' is not a valid variable.");
                    }

                case "REMOVE":
                    if (script.UniquePlayerVariables.TryGetValue(varName, out CustomPlayerVariable var2))
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