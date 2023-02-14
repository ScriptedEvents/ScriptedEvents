namespace ScriptedEvents.Variables.Condition.KillsAndDeaths
{
#pragma warning disable SA1402 // File may only contain a single type
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Variables.Interfaces;

    public class KillsAndDeathsVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public VariableGroupType GroupType => VariableGroupType.Condition;

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new Kills(),
            new ScpKills(),
        };
    }

    public class Kills : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{KILLS}";

        /// <inheritdoc/>
        public string Description => "The total amount of kills.";

        /// <inheritdoc/>
        public float Value => Round.Kills;
    }

    public class ScpKills : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{SCPKILLS}";

        /// <inheritdoc/>
        public string Description => "The total amount of SCP-related kills.";

        /// <inheritdoc/>
        public float Value => Round.KillsByScp;
    }
}
