using System;
using Exiled.API.Features;
using ScriptedEvents.API.Constants;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Map
{
    public class LightsOffAction : IScriptAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "LightsOff";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => "Turns all the lights off for a given period of time.";

        /// <inheritdoc/>
        public string LongDescription => ConstMessages.RoomInput;

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("room", typeof(Room[]), "The room to flicker the lights off.", true),
            new Argument("duration", typeof(TimeSpan), "The duration of the lights out.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            var rooms = (Room[])Arguments[0]!;
            var duration = (float)Arguments[1]!;

            foreach (Room room in rooms)
                room.TurnOffLights(duration);

            return new(true);
        }
    }
}