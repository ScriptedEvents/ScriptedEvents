using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions.AllInOne
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Structures;

    using Item = Exiled.API.Features.Items.Item;

    public class ItemInfoAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "ItemInfo";

        /// <inheritdoc/>
        public string Description => "All-in-one action for getting item related information from the provided item object.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new OptionsArgument("mode", true,
                    new("IsCarried", "Is item is in owner's inventory."),
                    new("Scale", "Item's scale."),
                    new("Weight", "Item's weight."),
                    new("Owner", "Item's owner."),
                    new("Type", "Item's type.")),
             new Argument("item", typeof(Item), "The item to use.", true),
        };

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.AllInOneInfo;

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string mode = Arguments[0].ToUpper();
            Item item = (Item)Arguments[1];

            if (mode is "OWNER")
            {
                Player plr = item.Owner;
                if (plr is null)
                {
                    return new(true, new(Array.Empty<Player>()));
                }

                return new(true, new(plr));
            }

            string ret = mode switch
            {
                "TYPE" => item.Type.ToString(),
                "ISVALID" => "TRUE",
                "CARRIED" => item.IsInInventory.ToUpper(),
                "SCALE" => item.Scale.ToUpper(),
                "WEIGHT" => item.Weight.ToUpper(),
                _ => throw new ArgumentException()
            };

            return new(true, new(ret));
        }
    }
}