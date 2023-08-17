namespace ScriptedEvents.Variables.Map
{
    using System;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
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
            new EngagedGenerators(),
            new Scp914Active(),
            new DoorState(),
        };
    }

    public class Decontaminated : IBoolVariable
    {
        /// <inheritdoc/>
        public string Name => "{DECONTAMINATED}";

        /// <inheritdoc/>
        public string ReversedName => "{!DECONTAMINATED}";

        /// <inheritdoc/>
        public string Description => "Whether or not Light Containment Zone has been decontaminated.";

        /// <inheritdoc/>
        public bool Value => Map.IsLczDecontaminated;
    }

    public class EngagedGenerators : IFloatVariable
    {
        /// <inheritdoc/>
        public float Value => Generator.Get(GeneratorState.Engaged).Count();

        /// <inheritdoc/>
        public string Name => "{ENGAGEDGENERATORS}";

        /// <inheritdoc/>
        public string Description => "The amount of generators which are fully engaged.";
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
        public string[] Arguments { get; set; }

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
                if (Arguments.Length < 1) return "ERROR: MISSING DOOR TYPE";

                if (!Enum.TryParse(Arguments[0], out DoorType dt))
                    return "ERROR: INVALID DOOR TYPE";

                Door d = Door.Get(dt);

                if (d is null)
                    return "ERROR: INVALID DOOR TYPE";

                return d.IsOpen ? "OPEN" : "CLOSED";
            }
        }
    }
}
