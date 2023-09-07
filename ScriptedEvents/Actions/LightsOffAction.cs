namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class LightsOffAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "LIGHTSOFF";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Lights;

        /// <inheritdoc/>
        public string Description => "Turns all the lights off for a given period of time.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("room", typeof(RoomType), "The room(s) to flicker the lights off.", true),
            new Argument("duration", typeof(float), "The duration of the lights out. Variables are supported.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!ScriptHelper.TryGetRooms(Arguments[0], out Room[] rooms, script))
                return new(MessageType.NoRoomsFound, this, "rooms", Arguments[0]);

            if (!VariableSystem.TryParse(Arguments[1], out int duration, script))
            {
                return new(MessageType.NotANumber, this, "duration", Arguments[1]);
            }

            foreach (Room room in rooms)
                room.TurnOffLights(duration);

            return new(true, string.Empty);
        }
    }
}