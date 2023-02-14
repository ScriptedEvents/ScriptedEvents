namespace ScriptedEvents.Variables.Condition.Strings
{
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Variables.Interfaces;

    public class StringVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public VariableGroupType GroupType => VariableGroupType.Condition;

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new NextWave(),
        };
    }

    public class NextWave : IStringVariable
    {
        /// <inheritdoc/>
        public string Name => "{NEXTWAVE}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public string Value => Respawn.NextKnownTeam.ToString();
    }
}
