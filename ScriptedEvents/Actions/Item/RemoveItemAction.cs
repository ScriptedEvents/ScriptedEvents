namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;

    using ScriptedEvents.API.Constants;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class RemoveItemAction : IScriptAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "REMOVEITEM";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Item;

        /// <inheritdoc/>
        public string Description => "Removes an item from the targeted players.";

        /// <inheritdoc/>
        public string LongDescription => ConstMessages.ItemInput;

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to remove the item from.", true),
            new Argument("item", typeof(ItemType), "The item to remove.", true),
            new Argument("amount", typeof(int), "The amount to remove. Variables are supported. Default: 1", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            ItemType itemType = (ItemType)Arguments[1];
            int amt = 1;

            if (Arguments.Length > 2)
            {
                amt = (int)Arguments[2];

                if (amt < 0)
                    return new(MessageType.LessThanZeroNumber, this, "amount", amt);
            }

            PlayerCollection plys = (PlayerCollection)Arguments[0];

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
