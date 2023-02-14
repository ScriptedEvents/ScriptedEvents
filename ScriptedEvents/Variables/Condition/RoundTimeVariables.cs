namespace ScriptedEvents.Variables.Condition.RoundTime
{
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Variables.Interfaces;

    public class RoundTimeVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public VariableGroupType GroupType => VariableGroupType.Condition;

        /// <inheritdoc/>
        public IVariable[] Variables => new IVariable[]
        {
            new RoundMinutes(),
            new RoundSeconds(),
        };
    }

    public class RoundMinutes : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{ROUNDMINUTES}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public float Value => (float)Round.ElapsedTime.TotalMinutes;
    }

    public class RoundSeconds : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{ROUNDSECONDS}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public float Value => (float)Round.ElapsedTime.TotalSeconds;
    }
}
