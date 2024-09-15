namespace ScriptedEvents.Variables.Map
{
#pragma warning disable SA1402 // File may only contain a single type.
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class MapVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Map";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new RandomRoom(),
            new RandomDoor(),
        };
    }

    public class RandomDoor : IStringVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{RANDOMDOOR}";

        /// <inheritdoc/>
        public string Description => "Gets the DoorType of a random door. Can be filtered by zone.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("zone", typeof(ZoneType), "A zone to filter by (optional).", false),
        };

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                ZoneType filter = ZoneType.Unspecified;

                if (Arguments.Length > 0)
                    filter = (ZoneType)Arguments[0];

                IEnumerable<Door> validDoors = Door.List;

                if (filter is not ZoneType.Unspecified)
                {
                    validDoors = validDoors.Where(door => door.Zone.HasFlag(filter));
                }

                List<Door> newList = validDoors.ToList();
                return newList[UnityEngine.Random.Range(0, newList.Count)].Type.ToString();
            }
        }
    }

    public class RandomRoom : IStringVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{RANDOMROOM}";

        /// <inheritdoc/>
        public string Description => "Gets the RoomType of a random room. Can be filtered by zone.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("zone", typeof(ZoneType), "A zone to filter by (optional).", false),
        };

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                ZoneType filter = ZoneType.Unspecified;

                if (Arguments.Length > 0)
                    filter = (ZoneType)Arguments[0];

                IEnumerable<Room> validRooms = Room.List.Where(room => room.Type != RoomType.Pocket);

                if (filter is not ZoneType.Unspecified)
                    validRooms = validRooms.Where(room => room.Zone.HasFlag(filter));

                List<Room> newList = validRooms.ToList();
                return newList[UnityEngine.Random.Range(0, newList.Count)].Type.ToString();
            }
        }
    }

#pragma warning restore SA1402 // File may only contain a single type.
}
