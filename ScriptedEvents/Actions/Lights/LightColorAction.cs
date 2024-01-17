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
    using ScriptedEvents.Variables;
    using UnityEngine;

    public class LightColorAction : IScriptAction, IHelpInfo, ILongDescription
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
        public string LongDescription => ConstMessages.RoomInput;

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("room", typeof(RoomType), "The room to change the color of.", true),
            new Argument("red", typeof(byte), "The red component of the color", true),
            new Argument("green", typeof(byte), "The green component of the color", true),
            new Argument("blue", typeof(byte), "The blue component of the color", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 4) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!ScriptHelper.TryGetRooms(Arguments[0], out Room[] rooms, script))
                return new(MessageType.NoRoomsFound, this, "rooms", Arguments[0]);

            if (!VariableSystem.TryParse(Arguments[1], out float r, script))
                return new(false, $"The red component of the color is invalid.");
            if (r < 0)
                return new(false, "The red component of the color must be greater than 0.");

            if (!VariableSystem.TryParse(Arguments[2], out float g, script))
                return new(false, $"The green component of the color is invalid.");
            if (g < 0)
                return new(false, "The green component of the color must be greater than 0.");

            if (!VariableSystem.TryParse(Arguments[3], out float b, script))
                return new(false, $"The blue component of the color is invalid.");
            if (b < 0)
                return new(false, "The blue component of the color must be greater than 0.");

            Color c = new(r / 255f, g / 255f, b / 255f);

            foreach (Room room in rooms)
            {
                room.Color = c;
            }

            return new(true);
        }
    }
}
