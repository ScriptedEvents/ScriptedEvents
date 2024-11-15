using System;
using Exiled.API.Enums;
using Exiled.API.Features.Doors;
using ScriptedEvents.API.Features.Exceptions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Map
{
    public class GetDoorStateAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "GetDoorState";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => "Returns the state of a door (either 'OPEN' or 'CLOSED').";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new MultiTypeArgument(
                 "door", 
                 new[]
                 {
                     typeof(Door), 
                     typeof(DoorType)
                 }, 
                 "The door to get the state of.",
                 true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            return Arguments[0] switch
            {
                Door door => new(true, new((door.IsOpen ? "OPEN" : "CLOSED").ToUpper())),
                DoorType doorType => new(true, new((Door.Get(doorType).IsOpen ? "OPEN" : "CLOSED").ToUpper())),
                _ => throw new ImpossibleException()
            };
        }
    }
}