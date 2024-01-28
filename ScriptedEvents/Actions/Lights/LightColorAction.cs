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
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Lights;

        /// <inheritdoc/>
        public string Description => "Sets the lights in the provided room(s) to the given RGB color.";

        /// <inheritdoc/>
        public string LongDescription => ConstMessages.RoomInput;

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("room", typeof(Room[]), "The room to change the color of.", true),
            new Argument("red", typeof(float), "The red component of the color", true),
            new Argument("green", typeof(float), "The green component of the color", true),
            new Argument("blue", typeof(float), "The blue component of the color", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 4) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            Room[] rooms = (Room[])Arguments[0];
            float r = (float)Arguments[1];
            float g = (float)Arguments[2];
            float b = (float)Arguments[3];

            if (r < 0)
                return new(false, "The red component of the color must be greater than 0.");

            if (g < 0)
                return new(false, "The green component of the color must be greater than 0.");

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
