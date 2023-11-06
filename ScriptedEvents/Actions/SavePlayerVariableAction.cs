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

    [Obsolete("Use the PLAYERVAR action.")]
    public class SavePlayerVariableAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "SAVEPLAYERS";

        /// <inheritdoc/>
        public string[] Aliases => new[] { "SAVEPLAYERVARIABLE" };

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Variable;

        /// <inheritdoc/>
        public string Description => "Saves a new player variable. Saved variables can be used in ANY script, and are reset when the round ends.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("variableName", typeof(string), "The name of the new variable. Braces will be added automatically if not provided.", true),
            new Argument("players", typeof(List<Player>), "The players to save as the new variable.", true),
            new Argument("max", typeof(int), "The maximum amount of players to save in this variable. Math and variables are supported. (default: unlimited).", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            int max = -1;

            if (Arguments.Length > 2)
            {
                string formula = VariableSystem.ReplaceVariables(string.Join(" ", Arguments.Skip(2)), script);

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

            if (!ScriptHelper.TryGetPlayers(Arguments[1], max, out PlayerCollection plys, script))
                return new(false, plys.Message);

            VariableSystem.DefineVariable(Arguments[0], "Script-defined variable.", plys.GetInnerList());

            return new(true);
        }
    }
}
