namespace ScriptedEvents.Variables.Map
{
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Variables.Interfaces;

    public class MapVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Map";

        /// <inheritdoc/>
        public VariableGroupType GroupType => VariableGroupType.Condition;

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new EngagedGenerators(),
            new DetonationTime(),
        };
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

    public class DetonationTime : IFloatVariable
    {
        /// <inheritdoc/>
        public float Value => Warhead.DetonationTimer;

        /// <inheritdoc/>
        public string Name => "{DETONATIONTIME}";

        /// <inheritdoc/>
        public string Description => "The amount of time until the warhead detonates.";
    }
}
