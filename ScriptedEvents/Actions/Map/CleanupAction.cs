namespace ScriptedEvents.Actions
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
            new OptionsArgument("mode", true,
                new("ITEMS", "Clean items."),
                new("RAGDOLLS", "Clean ragdolls.")),
            new Argument("filter", typeof(string), "Optionally, an ItemType/RoleTypeId of items/ragdolls to remove.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            switch (Arguments[0].ToUpper())
            {
                case "ITEMS":
                    ItemType type = ItemType.None;
                    if (Arguments.Length > 1)
                    {
                        if (!SEParser.TryParse((string)Arguments[1], out type, script))
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
                        if (!SEParser.TryParse((string)Arguments[1], out rType, script))
                            return new(MessageType.InvalidRole, this, "filter", Arguments[1]);
                    }

                    foreach (var item in Ragdoll.List.Where(p => rType == RoleTypeId.None || p.Role == rType))
                    {
                        item.Destroy();
                    }

                    break;
            }

            return new(true);
        }
    }
}