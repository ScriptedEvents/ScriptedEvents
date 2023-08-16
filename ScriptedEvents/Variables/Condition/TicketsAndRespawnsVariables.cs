namespace ScriptedEvents.Variables.TicketsAndRespawns
{
#pragma warning disable SA1402 // File may only contain a single type
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using PlayerRoles;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class TicketsAndRespawnsVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Tickets & Respawn Waves";

        /// <inheritdoc/>
        public VariableGroupType GroupType => VariableGroupType.Condition;

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new NtfTickets(),
            new ChaosTickets(),
            new TotalWaves(),
            new TimeUntilNextWave(),
            new TimeSinceLastWave(),
            new RespawnedPlayers(),
        };
    }

    public class NtfTickets : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{NTFTICKETS}";

        /// <inheritdoc/>
        public string Description => "The total amount of NTF tickets.";

        /// <inheritdoc/>
        public float Value => Respawn.NtfTickets;
    }

    public class ChaosTickets : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{CHAOSTICKETS}";

        /// <inheritdoc/>
        public string Description => "The total amount of Chaos Insurgency tickets.";

        /// <inheritdoc/>
        public float Value => Respawn.ChaosTickets;
    }

    public class TotalWaves : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{TOTALWAVES}";

        /// <inheritdoc/>
        public string Description => "The total amount of respawn waves.";

        /// <inheritdoc/>
        public float Value => MainPlugin.Handlers.RespawnWaves;
    }

    public class TimeUntilNextWave : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{TIMEUNTILNEXTWAVE}";

        /// <inheritdoc/>
        public string Description => "The amount of time until the next respawn wave, in seconds.";

        /// <inheritdoc/>
        public float Value => (float)Respawn.TimeUntilSpawnWave.TotalSeconds;
    }

    public class TimeSinceLastWave : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{TIMESINCELASTWAVE}";

        /// <inheritdoc/>
        public string Description => "The amount of time since the last respawn wave, in seconds. If a respawn wave has not occurred yet, this value will be very large.";

        /// <inheritdoc/>
        public float Value => (float)MainPlugin.Handlers.TimeSinceWave.TotalSeconds;
    }

    public class RespawnedPlayers : IFloatVariable, IArgumentVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{RESPAWNEDPLAYERS}";

        /// <inheritdoc/>
        public string Description => "The amount of players that have respawned in the most recent respawn wave.";

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("roleType", typeof(RoleTypeId), "The role to filter by.", false),
        };

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players
        {
            get
            {
                if (Arguments.Length > 0 && Enum.TryParse(Arguments[0], true, out RoleTypeId rt))
                {
                    Log.Info(Arguments[0]);
                    return MainPlugin.Handlers.RecentlyRespawned.Where(ply => ply.Role == rt);
                }

                return MainPlugin.Handlers.RecentlyRespawned;
            }
        }
    }
}
