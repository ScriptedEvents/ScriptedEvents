using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using ScriptedEvents.API.Features.Exceptions;
using ScriptedEvents.API.Modules;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Map
{
    public class RandomRoomAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "GetRandomRoom";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => $"Returns a random {nameof(RoomType)} or {nameof(Room)}. Can be filtered by zone.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("returnType", true,
                new OptionValueDepending("DoorType", $"A random {nameof(RoomType)}.", typeof(RoomType)),
                new OptionValueDepending("Door", $"A random {typeof(Room)} object.", typeof(Room))),
            new Argument("zone", typeof(ZoneType), "An optional ZoneType to filter by.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            ZoneType filter = (ZoneType?)Arguments[1] ?? ZoneType.Unspecified;
            
            IEnumerable<Room> validRooms = Room.List;
            if (filter is not ZoneType.Unspecified)
            {
                validRooms = validRooms.Where(door => door.Zone.HasFlag(filter));
            }

            return Arguments[0] switch
            {
                RoomType => new(true, new(validRooms.GetRandomValue().Type.ToString())),
                Room => new(true, new(ObjectReferenceModule.Singleton!.ToReference(validRooms.GetRandomValue()))),
                _ => throw new ImpossibleException()
            };
        }
    }
}