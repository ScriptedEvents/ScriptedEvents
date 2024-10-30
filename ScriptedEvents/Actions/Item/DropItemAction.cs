namespace ScriptedEvents.Actions.Item
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ScriptedEvents.API.Constants;
    using ScriptedEvents.Enums;
    using ScriptedEvents.Interfaces;
    using ScriptedEvents.Structures;

    public class DropItemAction : IScriptAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "Drop";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

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
            new Argument("item", typeof(ItemObjectOrType), "The item to drop. The 'amount' argument will be ignored if you provide an item object instead of a type.", true),
            new Argument("amount", typeof(int), "The amount to drop. Default: 1", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            var plys = (PlayerCollection)Arguments[0]!;
            object theItem = Arguments[1]!;

            if (theItem is Exiled.API.Features.Items.Item actualItem)
            {
                foreach (var plr in plys)
                {
                    if (plr.Items.Contains(actualItem))
                    {
                        plr.DropItem(actualItem);
                    }
                }

                return new(true);
            }

            int amt = (int?)Arguments[2] ?? 1;
            var itemType = (ItemType)theItem;

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
