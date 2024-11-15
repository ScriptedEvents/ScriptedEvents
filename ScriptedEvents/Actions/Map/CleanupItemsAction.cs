namespace ScriptedEvents.Actions.Map
{
    using System;
    using System.Linq;
    using Exiled.API.Features.Pickups;
    using ScriptedEvents.Enums;
    using ScriptedEvents.Interfaces;
    using ScriptedEvents.Structures;
    using Item = Exiled.API.Features.Items.Item;

    public class CleanupItemsAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "CleanupItems";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => "Cleans up items.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new MultiTypeArgument(
                "filter", 
                new[] { typeof(ItemType), typeof(Item) }, 
                "Optionally, an ItemType by which to remove the items OR a valid item object.",
                false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            switch (Arguments.Length)
            {
                case > 0 when Arguments[0] is ItemType type:
                    foreach (var x in Pickup.List.Where(p => p.Type == type))
                    {
                        x.Destroy();
                    }
                
                    return new(true);
                
                case > 0 when Arguments[0] is Item item:
                    item.Destroy();
                    return new(true);
                
                default:
                    foreach (var y in Pickup.List)
                    {
                        y.Destroy();
                    }

                    return new(true);
            }
        }
    }
}