using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions.Item
{
    using System;
    using System.Collections.Generic;

    using ScriptedEvents.API.Constants;
    using ScriptedEvents.Structures;

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
            var plys = (PlayerCollection)Arguments[0];
            var itemType = (ItemType)Arguments[1];
            var amt = 1;

            if (Arguments.Length >= 3)
            {
                amt = (int)Arguments[2];
            }

            foreach (var player in plys)
            {
                List<Exiled.API.Features.Items.Item> itemsToDrop = new();

                foreach (var item in player.Items)
                {
                    if (item.Type != itemType)
                    {
                        continue;
                    }

                    itemsToDrop.Add(item);

                    if (itemsToDrop.Count >= amt)
                    {
                        break;
                    }
                }

                foreach (var item in itemsToDrop)
                {
                    player.DropItem(item);
                }
            }

            return new(true);
        }
    }
}
