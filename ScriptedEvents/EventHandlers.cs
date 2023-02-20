namespace ScriptedEvents
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Cassie;
    using Exiled.Events.EventArgs.Interfaces;
    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Server;
    using Exiled.Events.EventArgs.Warhead;
    using MapGeneration.Distributors;
    using MEC;
    using PlayerRoles;
    using Respawning;
    using ScriptedEvents.API.Features.Exceptions;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Handlers;
    using UnityEngine;

    public class EventHandlers
    {
        private DateTime lastRespawnWave = DateTime.MinValue;

        /// <summary>
        /// Gets or sets the total amount of respawn waves since the round started.
        /// </summary>
        public int RespawnWaves { get; set; } = 0;

        /// <summary>
        /// Gets the amount of time since the last wave.
        /// </summary>
        public TimeSpan TimeSinceWave => DateTime.UtcNow - lastRespawnWave;

        /// <summary>
        /// Gets a value indicating whether or not a wave just spawned.
        /// </summary>
        public bool IsRespawning => TimeSinceWave.TotalSeconds < 5;

        /// <summary>
        /// Gets or sets the most recent respawn type.
        /// </summary>
        public SpawnableTeamType MostRecentSpawn { get; set; }

        /// <summary>
        /// Gets a list of players that most recently respawned.
        /// </summary>
        public List<Player> RecentlyRespawned { get; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether or not tesla gates are disabled.
        /// </summary>
        public bool TeslasDisabled { get; set; } = false;

        /// <summary>
        /// Gets or sets a list of strings indicating round-disabled features.
        /// </summary>
        public List<string> DisabledKeys { get; set; } = new();

        /// <summary>
        /// Gets a List of infection rules.
        /// </summary>
        public List<InfectRule> InfectionRules { get; } = new();

        /// <summary>
        /// Gets a dictionary of spawn rules.
        /// </summary>
        public Dictionary<RoleTypeId, int> SpawnRules { get; } = new();

        public void OnRestarting()
        {
            RespawnWaves = 0;
            lastRespawnWave = DateTime.MinValue;
            TeslasDisabled = false;

            ScriptHelper.StopAllScripts();
            ConditionVariables.ClearVariables();
            PlayerVariables.ClearVariables();
            DisabledKeys.Clear();

            if (CountdownHelper.MainHandle is not null && CountdownHelper.MainHandle.Value.IsRunning)
            {
                Timing.KillCoroutines(CountdownHelper.MainHandle.Value);
                CountdownHelper.MainHandle = null;
                CountdownHelper.Countdowns.Clear();
            }

            InfectionRules.Clear();
            SpawnRules.Clear();
            RecentlyRespawned.Clear();

            MostRecentSpawn = SpawnableTeamType.None;
        }

        public void OnWaitingForPlayers()
        {
            CountdownHelper.Start();
        }

        public void OnRoundStarted()
        {
            if (SpawnRules.Count > 0)
            {
                List<Player> players = Player.List.ToList();
                players.ShuffleList();

                int iterator = 0;

                foreach (KeyValuePair<RoleTypeId, int> rule in SpawnRules.Where(rule => rule.Value > 0))
                {
                    for (int i = iterator; i < iterator + rule.Value; i++)
                    {
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
                    Player[] newPlayers = players.Skip(iterator).ToArray();

                    KeyValuePair<RoleTypeId, int> rule = SpawnRules.FirstOrDefault(rule => rule.Value == -1);
                    foreach (Player player in newPlayers)
                    {
                        player.Role.Set(rule.Key);
                    }
                }
            }

            foreach (string name in MainPlugin.Singleton.Config.AutoRunScripts)
            {
                try
                {
                    Script scr = ScriptHelper.ReadScript(name, null);

                    if (scr.AdminEvent)
                    {
                        Log.Warn($"The '{name}' script is set to run each round, but the script is marked as an admin event!");
                        continue;
                    }

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
            lastRespawnWave = DateTime.UtcNow;

            MostRecentSpawn = ev.NextKnownTeam;

            RecentlyRespawned.Clear();
            RecentlyRespawned.AddRange(ev.Players);
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

        // Tesla
        public void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            if (TeslasDisabled || DisabledKeys.Contains("TESLAS"))
            {
                ev.IsInIdleRange = false;
                ev.IsInHurtingRange = false;
                ev.IsAllowed = false;
            }
        }

        // Disable Stuff
        public void OnDying(DyingEventArgs ev)
        {
            if (DisabledKeys.Contains("DYING"))
                ev.IsAllowed = false;
        }

        public void OnHurting(HurtingEventArgs ev)
        {
            if (DisabledKeys.Contains("HURTING"))
                ev.IsAllowed = false;
        }

        public void GeneratorEvent(IGeneratorEvent ev)
        {
            if (DisabledKeys.Contains("GENERATORS") && ev is IDeniableEvent deniable)
                deniable.IsAllowed = false;
        }

        public void OnShooting(ShootingEventArgs ev)
        {
            if (DisabledKeys.Contains("SHOOTING"))
                ev.IsAllowed = false;
        }

        public void OnSearchingPickup(SearchingPickupEventArgs ev)
        {
            if (DisabledKeys.Contains("ITEMPICKUPS"))
                ev.IsAllowed = false;

            if (DisabledKeys.Contains("MICROPICKUPS") && ev.Pickup.Type is ItemType.MicroHID)
                ev.IsAllowed = false;
        }

        public void OnInteractingLocker(InteractingLockerEventArgs ev)
        {
            if (ev.Locker is PedestalScpLocker && DisabledKeys.Contains("PEDESTALS"))
            {
                ev.IsAllowed = false;
            }
            else if (ev.Locker is not PedestalScpLocker && DisabledKeys.Contains("LOCKERS"))
            {
                ev.IsAllowed = false;
            }
        }

        public void OnEscaping(EscapingEventArgs ev)
        {
            if (DisabledKeys.Contains("ESCAPING"))
                ev.IsAllowed = false;
        }

        public void OnInteractingElevator(InteractingElevatorEventArgs ev)
        {
            if (DisabledKeys.Contains("ELEVATORS"))
                ev.IsAllowed = false;
        }

        public void OnHazardEvent(IHazardEvent ev)
        {
            if (DisabledKeys.Contains("HAZARDS") && ev is IDeniableEvent deny)
                deny.IsAllowed = false;
        }

        public void OnScp330Event(IDeniableEvent ev)
        {
            if (DisabledKeys.Contains("SCP330"))
                ev.IsAllowed = false;
        }

        public void OnScp914Event(IDeniableEvent ev)
        {
            if (DisabledKeys.Contains("SCP914"))
                ev.IsAllowed = false;
        }

        public void OnAnnouncingNtfEntrance(AnnouncingNtfEntranceEventArgs ev)
        {
            if (DisabledKeys.Contains("NTFANNOUNCEMENT"))
                ev.IsAllowed = false;
        }

        public void OnStartingWarhead(StartingEventArgs ev)
        {
            if (DisabledKeys.Contains("WARHEAD"))
                ev.IsAllowed = false;
        }

        public void OnActivatingWarheadPanel(ActivatingWarheadPanelEventArgs ev)
        {
            if (DisabledKeys.Contains("WARHEAD"))
                ev.IsAllowed = false;
        }
    }
}
