using ScriptedEvents.API.Modules;

namespace ScriptedEvents.Actions.AllInOne
{
    using System;

    using Exiled.API.Features;
    using Respawning;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Enums;
    using ScriptedEvents.Interfaces;
    using ScriptedEvents.Structures;

    public class WaveInfoAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "GetWaveInfo";

        /// <inheritdoc/>
        public string Description => "All-in-one action for getting wave related information.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new OptionValueDepending("NextTeam", "Next team to spawn.", typeof(SpawnableTeamType)),
                new OptionValueDepending("LastTeam", "Last spawned team.", typeof(SpawnableTeamType)),
                new OptionValueDepending("LastUnit", "Last spawned team UNIT.", typeof(string)),
                new OptionValueDepending("TotalWaves", "Amount of spawn waves which have occured.", typeof(int)),
                new OptionValueDepending("NtfTickets", "Current NTF tickets.", typeof(float)),
                new OptionValueDepending("ChaosTickets", "Current CI tickets.", typeof(float)),
                new OptionValueDepending("TimeUntilNew", "Amount of seconds remaining until the new spawn wave.", typeof(double)),
                new OptionValueDepending("TimeSinceLast", "Returns the amount of seconds since the last spawn wave.", typeof(double)),
                new OptionValueDepending("RespawnedPlayers", "Returns the players which have spawned with the last spawn wave.", typeof(PlayerCollection))),
        };

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.AllInOneInfo;

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string mode = Arguments[0]!.ToUpper();

            if (mode is "RESPAWNEDPLAYERS")
            {
                return new(true, new(EventHandlingModule.Singleton!.RecentlyRespawned));
            }

            string ret = mode switch
            {
                "NEXTTEAM" => Respawn.NextKnownTeam.ToString(),
                "LASTTEAM" => EventHandlingModule.Singleton!.MostRecentSpawn.ToString(),
                "LASTUNIT" => EventHandlingModule.Singleton!.MostRecentSpawnUnit,
                "TOTALWAVES" => EventHandlingModule.Singleton!.RespawnWaves.ToString(),
                "NTFTICKETS" => Respawn.NtfTickets.ToString(),
                "CHAOSTICKETS" => Respawn.ChaosTickets.ToString(),
                "TIMEUNTILNEW" => Respawn.TimeUntilSpawnWave.TotalSeconds.ToString(),
                "TIMESINCELAST" => EventHandlingModule.Singleton!.TimeSinceWave.TotalSeconds.ToString(),
                _ => throw new ArgumentException(),
            };

            return new(true, new(ret));
        }
    }
}