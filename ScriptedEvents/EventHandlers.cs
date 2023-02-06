using Exiled.API.Features;
using ScriptedEvents.API.Helpers;
using MEC;
using System.IO;
using ScriptedEvents.API.Features;
using ScriptedEvents.API.Features.Exceptions;
using Exiled.Events.EventArgs.Server;
using System;
using ScriptedEvents.Handlers.Variables;
using System.Collections.Generic;
using ScriptedEvents.Structures;
using Exiled.Events.EventArgs.Player;
using System.Linq;
using UnityEngine;
using PlayerRoles;

namespace ScriptedEvents
{
    public class EventHandlers
    {
        public int RespawnWaves = 0;
        public DateTime LastRespawnWave = DateTime.MinValue;

        public TimeSpan TimeSinceWave => DateTime.UtcNow - LastRespawnWave;
        public bool IsRespawning => TimeSinceWave.TotalSeconds < 5;

        public List<InfectRule> InfectionRules { get; } = new();

        public Dictionary<RoleTypeId, int> SpawnRules { get; } = new();

        public void OnRestarting()
        {
            RespawnWaves = 0;
            LastRespawnWave = DateTime.MinValue;

            ScriptHelper.StopAllScripts();
            ConditionVariables.ClearVariables();
            PlayerVariables.ClearVariables();

            InfectionRules.Clear();
            SpawnRules.Clear();
        }

        public void OnRoundStarted()
        {
            if (SpawnRules.Count > 0)
            {
                List<Player> players = Player.List.ToList();
                players.ShuffleList();

                int iterator = 0;

                foreach (var rule in SpawnRules.Where(rule => rule.Value > 0))
                {
                    for (int i = iterator; i < iterator+rule.Value; i++)
                    {
                        Log.Info(i);
                        Player p;
                        try
                        {
                            p = players[i];
                        }
                        catch (IndexOutOfRangeException)
                        {
                            break;
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            break;
                        }

                        if (!p.IsConnected)
                            continue;

                        p.Role.Set(rule.Key);
                    }
                    iterator += rule.Value;
                }

                if (SpawnRules.Any(rule => rule.Value == -1))
                {
                    List<Player> newPlayers = players.Skip(iterator).ToList();

                    var rule = SpawnRules.FirstOrDefault(rule => rule.Value == -1);
                    foreach (var player in newPlayers)
                    {
                        player.Role.Set(rule.Key);
                    }
                }
            }

            foreach (string name in MainPlugin.Singleton.Config.AutoRunScripts)
            {
                try
                {
                    Script scr = ScriptHelper.ReadScript(name);
                    ScriptHelper.RunScript(scr);
                }
                catch (DisabledScriptException)
                {
                    Log.Warn($"The '{name}' script is set to run each round, but the script is disabled!");
                }
                catch (FileNotFoundException)
                {
                    Log.Warn($"The '{name}' script is set to run each round, but the script is not found!");
                }
            }
        }

        public void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            if (!ev.IsAllowed) return;

            RespawnWaves++;
            LastRespawnWave = DateTime.UtcNow;

            ConditionVariables.DefineVariable("{LASTRESPAWNTEAM}", ev.NextKnownTeam.ToString());
            ConditionVariables.DefineVariable("{RESPAWNEDPLAYERS}", ev.Players.Count);
            PlayerVariables.DefineVariable("{RESPAWNEDPLAYERS}", ev.Players);
        }

        // Infection
        public void OnDied(DiedEventArgs ev)
        {
            if (ev.Player is null || ev.Attacker is null || ev.DamageHandler.Attacker is null)
                return;

            if (!InfectionRules.Any(r => r.OldRole == ev.TargetOldRole))
                return;

            InfectRule? ruleNullable = InfectionRules.FirstOrDefault(r => r.OldRole == ev.TargetOldRole);

            InfectRule rule = ruleNullable.Value;
            Vector3 pos = ev.Attacker.Position;

            Timing.CallDelayed(0.5f, () =>
            {
                ev.Player.Role.Set(rule.NewRole);

                if (rule.MovePlayer)
                    ev.Player.Teleport(pos);
            });
        }
    }
}
