﻿namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    /// <inheritdoc/>
    public class WaveInfoAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "WAVEINFO";

        /// <inheritdoc/>
        public string Description => "All-in-one action for getting wave related information.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("NextTeam", "Returns the next SpawnableTeamType to spawn."),
                new("LastTeam", "Returns the last spawned SpawnableTeamType."),
                new("LastUnit", "Returns the last spawned SpawnableTeamType UNIT."),
                new("TotalWaves", "Returns the amount of spawn waves which have occured."),
                new("NtfTickets", "Returns the current NTF tickets."),
                new("ChaosTickets", "Returns the current CI tickets."),
                new("TimeUntilNew", "Returns the amount of seconds remaining until the new spawn wave."),
                new("TimeSinceLast", "Returns the amount of seconds since the last spawn wave."),
                new("RespawnedPlayers", "Returns the players which have spawned with the last spawn wave.")),
        };

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.AllInOneInfo;

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string mode = Arguments[0].ToUpper();

            if (mode is "RESPAWNEDPLAYERS")
            {
                return new(true, variablesToRet: new[] { MainPlugin.Handlers.RecentlyRespawned });
            }

            string ret = mode switch
            {
                "NEXTTEAM" => Respawn.NextKnownTeam.ToString(),
                "LASTTEAM" => MainPlugin.Handlers.MostRecentSpawn.ToString(),
                "LASTUNIT" => MainPlugin.Handlers.MostRecentSpawnUnit.ToString(),
                "TOTALWAVES" => MainPlugin.Handlers.RespawnWaves.ToString(),
                "NTFTICKETS" => Respawn.NtfTickets.ToString(),
                "CHAOSTICKETS" => Respawn.ChaosTickets.ToString(),
                "TIMEUNTILNEXT" => Respawn.TimeUntilSpawnWave.TotalSeconds.ToString(),
                "TIMESINCELAST" => MainPlugin.Handlers.TimeSinceWave.TotalSeconds.ToString(),
                _ => throw new ArgumentException(),
            };

            return new(true, variablesToRet: new[] { ret });
        }
    }
}