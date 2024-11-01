namespace ScriptedEvents.Actions.Item
{
    using System;
    using System.Linq;
    using Exiled.API.Features;
    using ScriptedEvents.API.Constants;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Enums;
    using ScriptedEvents.Interfaces;
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
        public object?[] Arguments { get; set; }

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
                new Option("Add", "Adds an item to the player's inventory."),
                new Option("Drop", "Drops an item from the player's inventory."),
                new Option("Remove", "Removes an item from the player's inventory.")),
            new Argument("players", typeof(PlayerCollection), "The players to affect.", true),
            new Argument("item", typeof(ItemType), "The item to add/remove.", true),
            new Argument("amount", typeof(int), "The amount of items to add/remove. Default: 1", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            var mode = Arguments[0]!.ToUpper();
            var plys = (PlayerCollection)Arguments[1]!;
            var itemType = (ItemType)Arguments[2]!;
            var amt = (int?)Arguments[3] ?? 1;

            Action<Player> action = mode switch
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
                "DROP" => player =>
                {
                    var item = player.Items.FirstOrDefault(r => r.Type == itemType);
                    if (item is not null)
                    {
                        player.DropItem(item);
                    }
                },
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
