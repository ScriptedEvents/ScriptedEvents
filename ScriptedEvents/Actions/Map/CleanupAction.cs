namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using Exiled.API.Features.Pickups;
    using PlayerRoles;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class CleanupAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "CLEANUP";

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
            new Argument("mode", typeof(string), "The mode (ITEMS, RAGDOLLS).", true),
            new Argument("filter", typeof(string), "Optionally, an ItemType/RoleTypeId of items/ragdolls to remove.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            switch (Arguments[0].ToUpper())
            {
                case "ITEMS":
                    ItemType type = ItemType.None;
                    if (Arguments.Length > 1)
                    {
                        if (!VariableSystem.TryParse(Arguments[1], out type, script))
                            return new(false, "Invalid ItemType provided."); // Todo: Use error code
                    }

                    foreach (var item in Pickup.List.Where(p => type == ItemType.None || p.Type == type))
                    {
                        item.Destroy();
                    }

                    break;
                case "RAGDOLLS":
                    RoleTypeId rType = RoleTypeId.None;
                    if (Arguments.Length > 1)
                    {
                        if (!VariableSystem.TryParse(Arguments[1], out rType, script))
                            return new(MessageType.InvalidRole, this, "filter", Arguments[1]);
                    }

                    foreach (var item in Ragdoll.List.Where(p => rType == RoleTypeId.None || p.Role == rType))
                    {
                        item.Destroy();
                    }

                    break;
                default:
                    return new(MessageType.InvalidOption, this, "mode", "ITEMS/RAGDOLLS");
            }

            return new(true);
        }
    }
}