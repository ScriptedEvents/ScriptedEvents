using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features.Doors;
using ScriptedEvents.API.Features.Exceptions;
using ScriptedEvents.API.Modules;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Map
{
    public class RandomDoorAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "GetRandomDoor";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => $"Returns a random {nameof(DoorType)} or {nameof(Door)}. Can be filtered by zone.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("returnType", true,
                new OptionValueDepending("DoorType", "A random DoorType.", typeof(DoorType)),
                new OptionValueDepending("Door", "A random Door object.", typeof(Door))),
            new Argument("zone", typeof(ZoneType), "An optional ZoneType to filter by.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            ZoneType filter = (ZoneType?)Arguments[1] ?? ZoneType.Unspecified;
            
            IEnumerable<Door> validDoors = Door.List;
            if (filter is not ZoneType.Unspecified)
            {
                validDoors = validDoors.Where(door => door.Zone.HasFlag(filter));
            }

            return Arguments[0] switch
            {
                DoorType => new(true, new(validDoors.GetRandomValue().Type.ToString())),
                Door => new(true, new(ObjectReferenceModule.Singleton!.ToReference(validDoors.GetRandomValue()))),
                _ => throw new ImpossibleException()
            };
        }
    }
}