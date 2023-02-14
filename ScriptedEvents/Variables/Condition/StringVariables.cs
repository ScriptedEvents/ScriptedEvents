namespace ScriptedEvents.Variables.Condition.Strings
{
#pragma warning disable SA1402 // File may only contain a single type
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Variables.Interfaces;

    public class StringVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Strings";

        /// <inheritdoc/>
        public VariableGroupType GroupType => VariableGroupType.Condition;

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new NextWave(),
            new LastRespawnTeam(),
        };
    }

    public class LastRespawnTeam : IStringVariable
    {
        /// <inheritdoc/>
        public string Name => "{LASTRESPAWNTEAM}";

        /// <inheritdoc/>
        public string Description => "The most recent team that spawn.";

        /// <inheritdoc/>
        public string Value => MainPlugin.Handlers.MostRecentSpawn.ToString();
    }

    public class NextWave : IStringVariable
    {
        /// <inheritdoc/>
        public string Name => "{NEXTWAVE}";

        /// <inheritdoc/>
        public string Description => "The next team to spawn, either NineTailedFox, ChaosInsurgency, or None.";

        /// <inheritdoc/>
        public string Value => Respawn.NextKnownTeam.ToString();
    }
}
