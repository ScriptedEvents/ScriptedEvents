namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    /// <inheritdoc/>
    public class ItemInfoAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "ITEMINFO";

        /// <inheritdoc/>
        public string Description => "All-in-one action for getting item related information from the provided item id.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new OptionsArgument("mode", true,
                    new("ISVALID", "Is provided item id a valid item."),
                    new("ISCARRIED", "Is item is in owner's inventory."),
                    new("SCALE", "Item's scale."),
                    new("WEIGHT", "Item's weight."),
                    new("OWNER", "Item's owner."),
                    new("TYPE", "Item's type.")),
             new Argument("itemId", typeof(ushort), "The item id to use.", true),
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
            ushort id = (ushort)Arguments[1];

            Item item = Item.Get(id);

            if (item is null)
            {
                if (mode == "ISVALID")
                    return new(true, variablesToRet: new[] { "FALSE" });

                return new(false, $"Provided item ID '{id}' is not valid. Use the 'ISVALID' mode to verify the ID before accessing item info.");
            }

            if (mode is "OWNER")
            {
                Player plr = item.Owner;
                if (plr is null)
                {
                    return new(true);
                }

                return new(true, variablesToRet: new[] { plr });
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

            return new(true, variablesToRet: new[] { ret });
        }
    }
}