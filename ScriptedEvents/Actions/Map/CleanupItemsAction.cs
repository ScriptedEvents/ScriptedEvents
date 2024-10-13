namespace ScriptedEvents.Actions.Map
{
    using System;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.API.Features.Pickups;

    using PlayerRoles;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class CleanupItemsAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "CLEANUPITEMS";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => "Clean up items/ragdolls.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("filter", typeof(ItemType), "Optionally, an ItemType by which to remove the items.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            ItemType type = ItemType.None;
            if (Arguments.Length > 0)
            {
                type = (ItemType)Arguments[0];
            }

            foreach (var item in Pickup.List.Where(p => type == ItemType.None || p.Type == type))
            {
                item.Destroy();
            }

            return new(true);
        }
    }
}