namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using ScriptedEvents.API.Constants;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
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
            new OptionsArgument("mode", true,
                new("SET", "Sets the light color. Red, Green and Blue values need to be provided when using."),
                new("RESET", "Resets the light color.")),
            new Argument("room", typeof(Room[]), "The room to change the color of.", true),
            new Argument("red", typeof(float), "The red component of the color.", false),
            new Argument("green", typeof(float), "The green component of the color.", false),
            new Argument("blue", typeof(float), "The blue component of the color.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Room[] rooms = (Room[])Arguments[1];

            switch (Arguments[0].ToUpper())
            {
                case "SET":
                    if (Arguments.Length < 5) return new(false, ErrorGen.Generate(ErrorCode.MissingArguments));

                    float r = (float)Arguments[2];
                    float g = (float)Arguments[3];
                    float b = (float)Arguments[4];

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

                    break;
                case "RESET":
                    foreach (Room room in rooms)
                    {
                        if (room.RoomLightController is not null)
                            room.ResetColor();
                    }

                    break;
            }

            return new(true);
        }
    }
}
