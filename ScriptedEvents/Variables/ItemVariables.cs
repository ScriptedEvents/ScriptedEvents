namespace ScriptedEvents.Variables.Chance
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Features.Exceptions;
    using ScriptedEvents.Structures;
#pragma warning disable SA1402 // File may only contain a single type
    using ScriptedEvents.Variables.Interfaces;

    public class ItemVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Items";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new ShowItem(),
            new ItemOwner(),
            new RandomItem(),
        };
    }

    public class RandomItem : IStringVariable
    {
        /// <inheritdoc/>
        public string Name => "{ITEM-RANDOM}";

        /// <inheritdoc/>
        public string Description => "Gets the ItemType of a random item.";

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                ItemType[] items = (ItemType[])Enum.GetValues(typeof(ItemType));
                return items[UnityEngine.Random.Range(0, items.Length)].ToString();
            }
        }
    }

    public class ShowItem : IStringVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{ITEM-INFO}";

        /// <inheritdoc/>
        public string Description => "Returns more information about the item using the item id.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
                new Argument("itemId", typeof(object), "The item id to use.", true),
                new OptionsArgument("mode", false,
                    new("ISVALID", "Is provided item id a valid item."),
                    new("CARRIED", "Is item is in owner's inventory."),
                    new("SCALE", "Item's scale."),
                    new("WEIGHT", "Item's weight."),
                    new("TYPE", "Item's type. Default option.")),
        };

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                string mode = Arguments.Length > 1 ? Arguments[1].ToUpper() : "TYPE";
                string id = Arguments[0].ToString();

                if (!ushort.TryParse(id, out ushort val))
                {
                    throw new ScriptedEventsException($"Provided value '{val}' is not a valid item id.");
                }

                Item item = Item.Get(val);

                if (item is null)
                {
                    if (mode == "ISVALID") return "FALSE";
                    throw new ScriptedEventsException($"Provided value '{val}' is not a valid item id.");
                }

                return mode switch
                {
                    "TYPE" => item.Type.ToString(),
                    "ISVALID" => "TRUE",
                    "CARRIED" => item.IsInInventory.ToUpper(),
                    "SCALE" => item.Scale.ToUpper(),
                    "WEIGHT" => item.Weight.ToUpper(),
                    _ => throw new ArgumentException()
                };
            }
        }
    }

    public class ItemOwner : IFloatVariable, IArgumentVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{ITEM-OWNER}";

        /// <inheritdoc/>
        public string Description => "Returns the player which is the owner of the item.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
                new Argument("itemId", typeof(object), "The item id.", true),
        };

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players
        {
            get
            {
                string id = Arguments[0].ToString();

                if (!ushort.TryParse(id, out ushort val))
                {
                    throw new ScriptedEventsException($"Provided value '{val}' is not a valid item id.");
                }

                return new Player[] { Item.Get(val).Owner };
            }
        }

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }
    }
}
