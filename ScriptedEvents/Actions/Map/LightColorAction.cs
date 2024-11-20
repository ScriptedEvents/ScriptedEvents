﻿using System;
using System.Collections.Generic;
using Exiled.API.Features;
using ScriptedEvents.API.Constants;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.API.Features.Exceptions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;
using UnityEngine;

namespace ScriptedEvents.Actions.Map
{
    public class LightColorAction : IScriptAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "LightColor";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => "Sets the lights in the provided room(s) to the given RGB color, from 0 to 255, but can be set higher.";

        /// <inheritdoc/>
        public string LongDescription => ConstMessages.RoomInput;

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new Option("Set", "Sets the light color. Red, Green and Blue values need to be provided when using."),
                new Option("Reset", "Resets the light color. No need to provide RGB values.")),
            new MultiTypeArgument(
                "room", 
                new []
                {
                    typeof(Room),
                    typeof(Room[])
                }, 
                "The room to change the color of.",
                true),
            new Argument("red", typeof(float), "The red component of the color.", false, ArgFlag.BiggerOrEqual0),
            new Argument("green", typeof(float), "The green component of the color.", false, ArgFlag.BiggerOrEqual0),
            new Argument("blue", typeof(float), "The blue component of the color.", false, ArgFlag.BiggerOrEqual0),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            var rooms = Arguments[1]! switch
            {
                Room room => new List<Room>(new[] { room }),
                Room[] roomArray => new List<Room>(roomArray),
                _ => throw new ImpossibleException()
            };

            switch (Arguments[0]!.ToUpper())
            {
                case "SET":
                    if (Arguments.Length < 5)
                    {
                        return new(
                        false,
                        null,
                        new ErrorInfo(
                            "RGB components missing", 
                            "When using the 'Set' mode, RGB values need to be provided.",
                            Name)
                            .ToTrace()
                        );
                    }

                    float r = (float)Arguments[2]!;
                    float g = (float)Arguments[3]!;
                    float b = (float)Arguments[4]!;
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
