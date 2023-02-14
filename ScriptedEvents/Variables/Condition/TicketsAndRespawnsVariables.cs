namespace ScriptedEvents.Variables.Condition.TicketsAndRespawns
{
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Variables.Interfaces;

    public class TicketsAndRespawnsVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public VariableGroupType GroupType => VariableGroupType.Condition;

        /// <inheritdoc/>
        public IVariable[] Variables => new IVariable[]
        {
            new NtfTickets(),
            new ChaosTickets(),
            new TotalWaves(),
            new TimeUntilNextWave(),
            new TimeSinceLastWave(),
        };
    }

    public class NtfTickets : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{NTFTICKETS}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public float Value => Respawn.NtfTickets;
    }

    public class ChaosTickets : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{CHAOSTICKETS}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public float Value => Respawn.ChaosTickets;
    }

    public class TotalWaves : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{TOTALWAVES}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public float Value => MainPlugin.Handlers.RespawnWaves;
    }

    public class TimeUntilNextWave : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{TIMEUNTILNEXTWAVE}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public float Value => (float)Respawn.TimeUntilSpawnWave.TotalSeconds;
    }

    public class TimeSinceLastWave : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{TIMESINCELASTWAVE}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public float Value => (float)MainPlugin.Handlers.TimeSinceWave.TotalSeconds;
    }
}
