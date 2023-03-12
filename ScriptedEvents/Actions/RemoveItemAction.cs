namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Handlers;
    using UnityEngine;

    public class RemoveItemAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "REMOVEITEM";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Inventory;

        /// <inheritdoc/>
        public string Description => "Removes an item from the targeted players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(List<Player>), "The players to remove the item from.", true),
            new Argument("item", typeof(ItemType), "The item to remove.", true),
            new Argument("amount", typeof(int), "The amount to remove. Variables & Math are supported. Default: 1", false),
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
                string formula = ConditionVariables.ReplaceVariables(string.Join(" ", Arguments.Skip(2)), script);

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

            if (!ScriptHelper.TryGetPlayers(Arguments[0], null, out plys, script))
                return new(MessageType.NoPlayersFound, this, "players");

            foreach (Player player in plys)
            {
                for (int i = 0; i < amt; i++)
                {
                    Item item = player.Items.FirstOrDefault(r => r.Type == itemType);
                    if (item is not null)
                    {
                        player.RemoveItem(item);
                    }
                }
            }

            return new(true);
        }
    }
}
