using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions.Map
{
    using System;

    using Exiled.API.Enums;

    using Exiled.API.Features.Doors;
    using ScriptedEvents.Structures;

    public class DoorPermAction : IScriptAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "DoorPerm";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => "Manages permissions of facility doors.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("door", typeof(Door[]), "The doors to apply permissions for.", true),
            new Argument("permission", typeof(KeycardPermissions), "The permission to apply.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            foreach (Door door in (Door[])Arguments[0]!)
            {
                door.KeycardPermissions = (KeycardPermissions)Arguments[1]!;
            }

            return new(true);
        }

        public string LongDescription =>
            "Allows for setting custom keycard permissions on doors. If a door already has permissions, those will be overwritten.";
    }
}