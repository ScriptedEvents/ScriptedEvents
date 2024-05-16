namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;

    using ScriptedEvents.API.Constants;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class GiveAction : IScriptAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "GIVE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Item;

        /// <inheritdoc/>
        public string Description => "Gives the targeted players an item.";

        /// <inheritdoc/>
        public string LongDescription => ConstMessages.ItemInput;

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to give the item to.", true),
            new Argument("item", typeof(string), "The item to give.", true),
            new Argument("amount", typeof(int), "The amount to give. Variables are supported. Default: 1", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            bool useCustom;
            CustomItem item;
            ItemType itemType = ItemType.None;

            if (CustomItem.TryGet((string)Arguments[1], out item))
            {
                useCustom = true;
            }
            else if (SEParser.TryParse((string)Arguments[1], out itemType, script))
            {
                useCustom = false;
            }
            else
            {
                return new(false, "Invalid ItemType or Custom Item name provided.");
            }

            int amt = 1;

            if (Arguments.Length > 2)
            {
                amt = (int)Arguments[2];
            }

            PlayerCollection plys = (PlayerCollection)Arguments[0];

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
