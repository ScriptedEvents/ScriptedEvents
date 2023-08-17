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

    public class SetAHPAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "AHP";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Set the AHP of the targeted players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to affect.", true),
            new Argument("health", typeof(float), "The amount of artificial health to set the player to. Math and variables ARE supported.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!ScriptHelper.TryGetPlayers(Arguments[0], null, out Player[] plys, script))
                return new(MessageType.NoPlayersFound, this, "players");

            string formula = VariableSystem.ReplaceVariables(string.Join(" ", Arguments.Skip(1)), script);

            if (!ConditionHelper.TryMath(formula, out MathResult result))
            {
                return new(MessageType.NotANumberOrCondition, this, "health", formula, result);
            }

            if (result.Result < 0)
            {
                return new(MessageType.LessThanZeroNumber, this, "health", result.Result);
            }

            float hp = result.Result;
            foreach (Player ply in plys)
                ply.ArtificialHealth = hp;

            return new(true);
        }
    }
}
