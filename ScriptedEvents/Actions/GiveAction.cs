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
    using UnityEngine;

    public class GiveAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "GIVE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Inventory;

        /// <inheritdoc/>
        public string Description => "Gives the targeted players an item.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(List<Player>), "The players to give the item to.", true),
            new Argument("item", typeof(ItemType), "The item to give.", true),
            new Argument("amount", typeof(int), "The amount to give. Variables & Math are supported. Default: 1", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!Enum.TryParse<ItemType>(Arguments[1], true, out ItemType itemType))
                return new(false, "Invalid item provided.");

            int amt = 1;

            if (Arguments.Length > 2)
            {
                string formula = ConditionVariables.ReplaceVariables(string.Join(" ", Arguments.Skip(2)));

                if (ConditionHelper.TryMath(formula, out MathResult result))
                {
                    amt = Mathf.RoundToInt(result.Result);
                }
                else
                {
                    return new(MessageType.NotANumberOrCondition, this, "amount", formula, result);
                }

                if (amt < 0)
                {
                    return new(MessageType.LessThanZeroNumber, this, "amount", amt);
                }
            }

            Player[] plys;

            if (!ScriptHelper.TryGetPlayers(Arguments[0], null, out plys))
                return new(MessageType.NoPlayersFound, this, "players");

            foreach (Player player in plys)
            {
                for (int i = 0; i < amt; i++)
                {
                    player.AddItem(itemType);
                }
            }

            return new(true);
        }
    }
}
