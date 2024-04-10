namespace ScriptedEvents.Variables.Chance
{
    using System;

    using Exiled.API.Features.Items;
    using ScriptedEvents.API.Extensions;
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
        };
    }

    public class ShowItem : IStringVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{SHOWITEM}";

        /// <inheritdoc/>
        public string Description => "Returns more information about the item using the item id.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
                new Argument("mode", typeof(object), "The mode to use. (TYPE, ISVALID)", true),
                new Argument("itemId", typeof(string), "The item id to use. If the item id is invalid, '<invalid item id>' will be returned.", true),
        };

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                string mode = Arguments[0].ToUpper();

                Item item = Item.Get((ushort)Arguments[1]);
                if (item is null)
                {
                    if (mode == "ISVALID") return "FALSE";
                    throw new ArgumentException($"Invalid item id.", Arguments[1].ToString());
                }

                return mode switch
                {
                    "TYPE" => item.Type.ToString(),
                    "ISVALID" => "TRUE",
                    _ => throw new ArgumentException($"Invalid mode.", mode)
                };
            }
        }
    }
}
