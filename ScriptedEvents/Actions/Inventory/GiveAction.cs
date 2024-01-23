﻿namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;

    using ScriptedEvents.API.Constants;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class GiveAction : IScriptAction, IHelpInfo, ILongDescription
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
        public string LongDescription => ConstMessages.ItemInput;

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(List<Player>), "The players to give the item to.", true),
            new Argument("item", typeof(ItemType), "The item to give.", true),
            new Argument("amount", typeof(int), "The amount to give. Variables are supported. Default: 1", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            bool useCustom;
            CustomItem item = null;
            ItemType itemType = ItemType.None;

            if (CustomItem.TryGet(Arguments[1], out CustomItem customItem))
            {
                useCustom = true;
                item = customItem;
            }
            else if (VariableSystem.TryParse<ItemType>(Arguments[1], out ItemType itemType2, script))
            {
                useCustom = false;
                itemType = itemType2;
            }
            else
            {
                return new(false, "Invalid ItemType or Custom Item name provided.");
            }

            int amt = 1;

            if (Arguments.Length > 2)
            {
                if (!VariableSystem.TryParse(Arguments[2], out amt, script))
                    return new(MessageType.NotANumber, this, "amount", Arguments[4]);
            }

            if (!ScriptHelper.TryGetPlayers(Arguments[0], null, out PlayerCollection plys, script))
                return new(false, plys.Message);

            foreach (Player player in plys)
            {
                for (int i = 0; i < amt; i++)
                {
                    if (useCustom)
                        item.Give(player);
                    else
                        player.AddItem(itemType);
                }
            }

            return new(true);
        }
    }
}
