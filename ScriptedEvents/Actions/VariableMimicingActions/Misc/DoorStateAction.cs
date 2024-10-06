namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features.Doors;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    /// <inheritdoc/>
    public class DoorStateAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "DOORSTATE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Misc;

        /// <inheritdoc/>
        public string Description => "Returns the state of a door (either 'OPEN' or 'CLOSED').";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new Argument("door", typeof(DoorType), "The door to get the state of.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            DoorType dt = (DoorType)Arguments[0];
            Door d = Door.Get(dt);

            return d is null
                ? throw new ArgumentException(ErrorGen.Get(ErrorCode.InvalidEnumGeneric, dt.ToString(), nameof(DoorType)))
                : new(true, variablesToRet: new[] { (d.IsOpen ? "OPEN" : "CLOSED") });
        }
    }
}