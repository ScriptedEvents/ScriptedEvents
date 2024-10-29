﻿namespace ScriptedEvents.Actions.ItemActions
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
                    new("IsValid", "Is provided item id a valid item."),
                    new("IsCarried", "Is item is in owner's inventory."),
                    new("Scale", "Item's scale."),
                    new("Weight", "Item's weight."),
                    new("Owner", "Item's owner."),
                    new("Type", "Item's type.")),
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

                return new(
                    false,
                    new ErrorTrace(new ErrorInfo("Item reference error", $"Provided item ID '{id}' is not valid. Use the 'ISVALID' mode to verify the ID before accessing item info.", "ITEMINFO action")));
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