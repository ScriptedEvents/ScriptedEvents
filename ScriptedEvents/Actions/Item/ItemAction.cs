using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions.Item
{
    using System;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;

    using ScriptedEvents.API.Constants;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Structures;

    public class ItemAction : IScriptAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "Item";

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
                new("Add", "Add an item to the player's inventory."),
                new("Remove", "Remove an item from the player's inventory.")),
            new Argument("players", typeof(PlayerCollection), "The players to affect.", true),
            new Argument("item", typeof(ItemType), "The item to add/remove.", true),
            new Argument("amount", typeof(int), "The amount of items to add/remove. Default: 1", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            var plys = (PlayerCollection)Arguments[1];
            var itemType = (ItemType)Arguments[2];
            var amt = 1;

            if (Arguments.Length >= 4)
            {
                amt = (int)Arguments[3];
            }

            Action<Player> action = Arguments[0].ToUpper() switch
            {
                "REMOVE" => (player) =>
                {
                    var item = player.Items.FirstOrDefault(r => r.Type == itemType);
                    if (item is not null)
                    {
                        player.RemoveItem(item);
                    }
                },
                "ADD" => (player) => { player.AddItem(itemType); },
                _ => throw new Exception($"Unknown mode: {Arguments[0]}"),
            };

            foreach (var player in plys)
            {
                for (var i = 0; i < amt; i++)
                {
                    action(player);
                }
            }

            return new(true);
        }
    }
}
