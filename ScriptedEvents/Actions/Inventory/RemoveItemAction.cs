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
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Inventory;

        /// <inheritdoc/>
        public string Description => "Removes an item from the targeted players.";

        /// <inheritdoc/>
        public string LongDescription => ConstMessages.ItemInput;

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(List<Player>), "The players to remove the item from.", true),
            new Argument("item", typeof(ItemType), "The item to remove.", true),
            new Argument("amount", typeof(int), "The amount to remove. Variables are supported. Default: 1", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!VariableSystem.TryParse<ItemType>(Arguments[1], out ItemType itemType, script))
                return new(false, "Invalid item provided.");

            int amt = 1;

            if (Arguments.Length > 2)
            {
                if (!VariableSystem.TryParse(Arguments[2], out amt, script))
                    return new(MessageType.NotANumber, this, "amount", Arguments[2]);
                if (amt < 0)
                    return new(MessageType.LessThanZeroNumber, this, "amount", amt);
            }

            if (!ScriptHelper.TryGetPlayers(Arguments[0], null, out PlayerCollection plys, script))
                return new(false, plys.Message);

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
