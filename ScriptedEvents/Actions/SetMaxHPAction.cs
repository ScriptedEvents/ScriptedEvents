namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;
    using Exiled.API.Features;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Handlers;

    public class SetMaxHPAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "MAXHP";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Set the Maximum HP of the targeted players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to affect.", true),
            new Argument("maxhealth", typeof(float), "The amount of max health to set the player to. Math and variables ARE supported.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!ScriptHelper.TryGetPlayers(Arguments[0], null, out Player[] plys))
                return new(MessageType.NoPlayersFound, this, "players");

            string formula = ConditionVariables.ReplaceVariables(string.Join(" ", Arguments.Skip(1)));

            if (!ConditionHelper.TryMath(formula, out MathResult result))
            {
                return new(MessageType.NotANumberOrCondition, this, "duration", formula, result);
            }

            if (result.Result < 0)
            {
                return new(MessageType.LessThanZeroNumber, this, "duration", result.Result);
            }

            float hp = result.Result;
            foreach (Player ply in plys)
                ply.MaxHealth = hp;

            return new(true);
        }
    }
}
