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

    public class LightsOffAction : IScriptAction, IHelpInfo, ILongDescription
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
        public string LongDescription => ConstMessages.RoomInput;

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("room", typeof(RoomType), "The room to flicker the lights off.", true),
            new Argument("duration", typeof(float), "The duration of the lights out. Variables are supported.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!ScriptHelper.TryGetRooms(Arguments[0], out Room[] rooms, script))
                return new(MessageType.NoRoomsFound, this, "rooms", Arguments[0]);

            if (!VariableSystem.TryParse(Arguments[1], out float duration, script))
            {
                return new(MessageType.NotANumber, this, "duration", Arguments[1]);
            }

            foreach (Room room in rooms)
                room.TurnOffLights(duration);

            return new(true, string.Empty);
        }
    }
}