﻿using System;
using System.Collections.Generic;
using ScriptedEvents.API.Constants;
using ScriptedEvents.API.Enums;
using ScriptedEvents.API.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Item
{
    public class DropItemAction : IScriptAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "DROPITEM";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Item;

        /// <inheritdoc/>
        public string Description => "Drops a specified item from players' inventories.";

        /// <inheritdoc/>
        public string LongDescription => ConstMessages.ItemInput;

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(PlayerCollection), "The players to drop items for.", true),
            new Argument("item", typeof(ItemType), "The item to drop.", true),
            new Argument("amount", typeof(int), "The amount to drop. Default: 1", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection plys = (PlayerCollection)Arguments[0];
            ItemType itemType = (ItemType)Arguments[1];
            int amt = 1;

            if (Arguments.Length >= 3)
            {
                amt = (int)Arguments[2];
            }

            foreach (Exiled.API.Features.Player player in plys)
            {
                List<Exiled.API.Features.Items.Item> itemsToDrop = new();

                foreach (Exiled.API.Features.Items.Item item in player.Items)
                {
                    if (item.Type == itemType)
                    {
                        itemsToDrop.Add(item);

                        if (itemsToDrop.Count >= amt)
                        {
                            break;
                        }
                    }
                }

                foreach (Exiled.API.Features.Items.Item item in itemsToDrop)
                {
                    player.DropItem(item);
                }
            }

            return new(true);
        }
    }
}
