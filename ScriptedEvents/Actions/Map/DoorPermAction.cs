namespace ScriptedEvents.Actions.Map
{
    using System;

    using Exiled.API.Enums;

    using Exiled.API.Features.Doors;

    using ScriptedEvents.API.Enums;

    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class DoorPermAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "DOORPERM";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => "Allows for setting custom keycard permissions on doors. If a door already has permissions, those will be overwritten.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("door", typeof(Door[]), "The doors to apply permissions for.", true),
            new Argument("permission", typeof(KeycardPermissions), "The permission to apply.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            foreach (Door door in (Door[])Arguments[0])
            {
                door.KeycardPermissions = (KeycardPermissions)Arguments[1];
            }

            return new(true);
        }
    }
}