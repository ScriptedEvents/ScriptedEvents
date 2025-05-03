using System;
using System.Linq;
using ScriptedEvents.API.Constants;
using ScriptedEvents.API.Enums;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.API.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Item
{
    public class ItemAction : IScriptAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "ITEM";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Item;

        /// <inheritdoc/>
        public string Description => "Gives or removes items from players.";

        /// <inheritdoc/>
        public string LongDescription => ConstMessages.ItemInput;

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("ADD", "Add an item to the player's inventory."),
                new("REMOVE", "Remove an item from the player's inventory.")),
            new Argument("players", typeof(PlayerCollection), "The players to affect.", true),
            new Argument("item", typeof(ItemType), "The item to add/remove.", true),
            new Argument("amount", typeof(int), "The amount of items to add/remove. Default: 1", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection plys = (PlayerCollection)Arguments[1];
            ItemType itemType = (ItemType)Arguments[2];
            Action<Exiled.API.Features.Player> action = null;
            int amt = 1;

            if (Arguments.Length >= 4)
            {
                amt = (int)Arguments[3];
            }

            switch (Arguments[0].ToUpper())
            {
                case "REMOVE":
                    action = (player) =>
                    {
                        Exiled.API.Features.Items.Item item = player.Items.FirstOrDefault(r => r.Type == itemType);
                        if (item is not null)
                        {
                            player.RemoveItem(item);
                        }
                    };
                    break;

                case "ADD":
                    action = (player) =>
                    {
                        player.AddItem(itemType);
                    };
                    break;
            }

            foreach (Exiled.API.Features.Player player in plys)
            {
                for (int i = 0; i < amt; i++)
                {
                    action(player);
                }
            }

            return new(true);
        }
    }
}
