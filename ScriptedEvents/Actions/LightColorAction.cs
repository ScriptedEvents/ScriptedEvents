namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;

    using UnityEngine;

    public class LightColorAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "LIGHTCOLOR";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Lights;

        /// <inheritdoc/>
        public string Description => "Sets the lights in the provided room(s) to the given RGB color.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("room", typeof(RoomType), "The room(s) to change the color of.", true),
            new Argument("r", typeof(byte), "The R component of the color", true),
            new Argument("g", typeof(byte), "The G component of the color", true),
            new Argument("b", typeof(byte), "The B component of the color", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 4) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!ScriptHelper.TryGetRooms(Arguments[0], out Room[] rooms))
                return new(MessageType.NoRoomsFound, this, "rooms", Arguments[0]);

            if (!byte.TryParse(Arguments[1], out byte r))
                return new(false, $"Invalid RGB combination. Each number must be in the range of 0-255.");

            if (!byte.TryParse(Arguments[2], out byte g))
                return new(false, $"Invalid RGB combination. Each number must be in the range of 0-255.");

            if (!byte.TryParse(Arguments[3], out byte b))
                return new(false, $"Invalid RGB combination. Each number must be in the range of 0-255.");

            Color c = new(r / 255f, g / 255f, b / 255f);

            foreach (Room room in rooms)
            {
                if (room.FlickerableLightController is not null)
                    room.Color = c;
            }

            return new(true);
        }
    }
}
