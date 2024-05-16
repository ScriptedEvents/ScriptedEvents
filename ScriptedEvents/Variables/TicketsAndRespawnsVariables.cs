namespace ScriptedEvents.Variables.TicketsAndRespawns
{
#pragma warning disable SA1402 // File may only contain a single type
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using PlayerRoles;
    using Respawning;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class TicketsAndRespawnsVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Tickets & Respawn Waves";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new NextWave(),
            new LastRespawnTeam(),
            new LastUnitName(),
            new WaveRespawning(),
            new NtfTickets(),
            new ChaosTickets(),
            new TotalWaves(),
            new TimeUntilNextWave(),
            new TimeSinceLastWave(),
            new RespawnedPlayers(),
            new ChaosSpawns(),
            new MtfSpawns(),
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

    public class LastUnitName : IStringVariable
    {
        /// <inheritdoc/>
        public string Name => "{LASTRESPAWNUNIT}";

        /// <inheritdoc/>
        public string Description => "The most recent team's unit name.";

        /// <inheritdoc/>
        public string Value => MainPlugin.Handlers.MostRecentSpawnUnit;
    }

    public class WaveRespawning : IBoolVariable
    {
        /// <inheritdoc/>
        public string Name => "{WAVERESPAWNING}";

        /// <inheritdoc/>
        public string ReversedName => "{!WAVERESPAWNING}";

        /// <inheritdoc/>
        public string Description => "Whether or not a wave has respawned within the last 5 seconds.";

        /// <inheritdoc/>
        public bool Value => MainPlugin.Handlers.IsRespawning;
    }

    public class NtfTickets : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{NTFTICKETS}";

        /// <inheritdoc/>
        public string Description => "The amount of NTF tickets.";

        /// <inheritdoc/>
        public float Value => Respawn.NtfTickets;
    }

    public class ChaosTickets : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{CHAOSTICKETS}";

        /// <inheritdoc/>
        public string Description => "The amount of Chaos Insurgency tickets.";

        /// <inheritdoc/>
        public float Value => Respawn.ChaosTickets;
    }

    public class TotalWaves : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{TOTALWAVES}";

        /// <inheritdoc/>
        public string Description => "The amount of respawn waves.";

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

    public class RespawnedPlayers : IFloatVariable, IArgumentVariable, IPlayerVariable, INeedSourceVariable
    {
        /// <inheritdoc/>
        public string Name => "{RESPAWNEDPLAYERS}";

        /// <inheritdoc/>
        public string Description => "The amount of players that have respawned in the most recent respawn wave.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("roleType", typeof(RoleTypeId), "The role to filter by.", false),
        };

        /// <inheritdoc/>
        public Script Source { get; set; }

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players
        {
            get
            {
                // This is technically not necessary anymore since {FILTER} exists. TODO Think about this
                if (Arguments.Length > 0)
                {
                    return MainPlugin.Handlers.RecentlyRespawned.Where(ply => ply.Role == (RoleTypeId)Arguments[0]);
                }

                return MainPlugin.Handlers.RecentlyRespawned;
            }
        }
    }

    public class ChaosSpawns : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{CHAOSSPAWNS}";

        /// <inheritdoc/>
        public string Description => "Total amount of Chaos Insurgency respawns.";

        /// <inheritdoc/>
        public float Value => MainPlugin.Handlers.SpawnsByTeam[SpawnableTeamType.ChaosInsurgency];
    }

    public class MtfSpawns : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{MTFSPAWNS}";

        /// <inheritdoc/>
        public string Description => "Total amount of Mobile Task Force respawns.";

        /// <inheritdoc/>
        public float Value => MainPlugin.Handlers.SpawnsByTeam[SpawnableTeamType.NineTailedFox];
    }
}
