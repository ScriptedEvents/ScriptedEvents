namespace ScriptedEvents.Variables.Map
{
#pragma warning disable SA1402 // File may only contain a single type.
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Features.Exceptions;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class MapVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Map";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new Decontaminated(),
            new Scp914Active(),
            new DoorState(),
            new Overcharged(),
            new Generators(),
            new RandomRoom(),
            new RandomDoor(),
            new InRoom(),
            new CassieSpeaking(),
            new Seed(),
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

    public class Generators : IFloatVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{GENERATORS}";

        /// <inheritdoc/>
        public string Description => "Gets the number of generators fulfilling the requirements.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("ENGAGED"),
                new("ACTIVATING"),
                new("UNLOCKED"),
                new("OPENED"),
                new("CLOSED")),
        };

        /// <inheritdoc/>
        public float Value
        {
            get
            {
                return Arguments[0].ToUpper() switch
                {
                    
                    _ => throw new ScriptedEventsException($"Mode {Arguments[0]} is not ENGAGED/ACTIVATING/UNLOCKED/OPENED or CLOSED."),
                };
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

    public class Scp914Active : IBoolVariable
    {
        /// <inheritdoc/>
        public string Name => "{SCP914ACTIVE}";

        /// <inheritdoc/>
        public string ReversedName => "{!SCP914ACTIVE}";

        /// <inheritdoc/>
        public string Description => "Whether or not SCP-914 is currently active.";

        /// <inheritdoc/>
        public bool Value => Scp914.IsWorking;
    }

    public class DoorState : IStringVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{DOORSTATE}";

        /// <inheritdoc/>
        public string Description => "Reveals the state of a door (either 'OPEN' or 'CLOSED').";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("door", typeof(DoorType), "The door to get the state of.", true),
        };

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                DoorType dt = (DoorType)Arguments[0];
                Door d = Door.Get(dt);

                return d is null
                    ? throw new ArgumentException(ErrorGen.Get(ErrorCode.InvalidEnumGeneric, dt.ToString(), nameof(DoorType)))
                    : (d.IsOpen ? "OPEN" : "CLOSED");
            }
        }
    }
#pragma warning restore SA1402 // File may only contain a single type.
}
