using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using ScriptedEvents.Structures;

    /// <inheritdoc/>
    public class RandomDoorAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "RANDOM-DOOR";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.RandomEnums;

        /// <inheritdoc/>
        public string Description => "Returns a random 'DoorType'. Can be filtered by zone.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("zone", typeof(ZoneType), "A ZoneType to filter by (optional).", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            ZoneType filter = ZoneType.Unspecified;

            if (Arguments.Length > 0)
                filter = (ZoneType)Arguments[0];

            IEnumerable<Door> validDoors = Door.List;

            if (filter is not ZoneType.Unspecified)
            {
                validDoors = validDoors.Where(door => door.Zone.HasFlag(filter));
            }

            return new(true, variablesToRet: new[] { validDoors.GetRandomValue().Type.ToString() });
        }
    }
}