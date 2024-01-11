namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    using ScriptedEvents.API.Constants;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class ResetLightColorAction : IScriptAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "RESETLIGHTCOLOR";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Lights;

        /// <inheritdoc/>
        public string Description => "Resets the light color in the given room.";

        /// <inheritdoc/>
        public string LongDescription => ConstMessages.RoomInput;

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("room", typeof(RoomType), "The room(s) to reset the color of.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!ScriptHelper.TryGetRooms(Arguments[0], out Room[] rooms, script))
                return new(MessageType.NoRoomsFound, this, "rooms", Arguments[0]);

            foreach (Room room in rooms)
            {
                if (room.RoomLightController is not null)
                    room.ResetColor();
            }

            return new(true);
        }
    }
}
