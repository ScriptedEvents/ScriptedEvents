namespace ScriptedEvents.Variables.Map
{
#pragma warning disable SA1402 // File may only contain a single type.
    using System;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using ScriptedEvents.API.Features;
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
            new Generators(),
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

    public class Generators : IFloatVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{GENERATORS}";

        /// <inheritdoc/>
        public string Description => "Gets the number of generators fulfilling the requirements.";

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("mode", typeof(string), "The mode for which to check for generators. Valid modes are ENGAGED/ACTIVATING/UNLOCKED/OPENED/CLOSED.", true),
        };

        /// <inheritdoc/>
        public float Value
        {
            get
            {
                if (Arguments.Length < 1)
                {
                    throw new ArgumentException(MsgGen.VariableArgCount(Name, new[] { "name" }));
                }

                switch (Arguments[0].ToUpper())
                {
                    case "ENGAGED":
                        return Generator.Get(GeneratorState.Engaged).Count();
                    case "ACTIVATING":
                        return Generator.Get(GeneratorState.Activating).Count();
                    case "UNLOCKED":
                        return Generator.Get(GeneratorState.Unlocked).Count();
                    case "OPENED":
                        return Generator.Get(GeneratorState.Open).Count();
                    case "CLOSED":
                        return Generator.Get(gen => gen.IsOpen is false).Count();
                    default:
                        throw new Exception($"Mode {Arguments[0]} is not ENGAGED/ACTIVATING/UNLOCKED/OPENED or CLOSED.");
                }
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

    public class DoorState : IStringVariable, IArgumentVariable, INeedSourceVariable
    {
        /// <inheritdoc/>
        public string Name => "{DOORSTATE}";

        /// <inheritdoc/>
        public string Description => "Reveals the state of a door (either 'OPEN' or 'CLOSED').";

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public Script Source { get; set; }

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
                if (Arguments.Length < 1)
                {
                    throw new ArgumentException(MsgGen.VariableArgCount(Name, new[] { "door" }));
                }

                if (!VariableSystem.TryParse(Arguments[0], out DoorType dt, Source, false))
                    throw new ArgumentException($"Provided value '{Arguments[0]}' is not a valid DoorType.");

                Door d = Door.Get(dt);

                return d is null
                    ? throw new ArgumentException($"Provided value '{Arguments[0]}' is not a valid DoorType.")
                    : (d.IsOpen ? "OPEN" : "CLOSED");
            }
        }
    }
#pragma warning restore SA1402 // File may only contain a single type.
}
