namespace ScriptedEvents
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Interfaces;
    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Scp049;
    using Exiled.Events.EventArgs.Scp0492;
    using Exiled.Events.EventArgs.Scp079;
    using Exiled.Events.EventArgs.Scp096;
    using Exiled.Events.EventArgs.Scp106;
    using Exiled.Events.EventArgs.Scp173;
    using Exiled.Events.EventArgs.Scp3114;
    using Exiled.Events.EventArgs.Scp939;
    using Exiled.Events.EventArgs.Server;
    using Exiled.Events.EventArgs.Warhead;

    using MapGeneration.Distributors;
    using MEC;
    using PlayerRoles;

    using Respawning;

    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;

    using UnityEngine;

    using MapHandler = Exiled.Events.Handlers.Map;
    using PlayerHandler = Exiled.Events.Handlers.Player;
    using Scp0492Handler = Exiled.Events.Handlers.Scp0492;
    using Scp049Handler = Exiled.Events.Handlers.Scp049;
    using Scp079Handler = Exiled.Events.Handlers.Scp079;
    using Scp096Handler = Exiled.Events.Handlers.Scp096;
    using Scp106Handler = Exiled.Events.Handlers.Scp106;
    using Scp173Handler = Exiled.Events.Handlers.Scp173;
    using Scp3114Handler = Exiled.Events.Handlers.Scp3114;
    using Scp330Handler = Exiled.Events.Handlers.Scp330;
    using Scp914Handler = Exiled.Events.Handlers.Scp914;
    using Scp939Handler = Exiled.Events.Handlers.Scp939;
    using ServerHandler = Exiled.Events.Handlers.Server;

    public class EventHandlingModule : SEModule
    {
        private DateTime lastRespawnWave = DateTime.MinValue;

        public override string Name => "EventHandlingModule";

        /// <summary>
        /// Gets or sets the amount of respawn waves since the round started.
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
        /// Gets or sets the spawns by team.
        /// </summary>
        public Dictionary<SpawnableTeamType, int> SpawnsByTeam { get; set; } = new()
        {
            [SpawnableTeamType.ChaosInsurgency] = 0,
            [SpawnableTeamType.NineTailedFox] = 0,
        };

        /// <summary>
        /// Gets or sets escaped players.
        /// </summary>
        public Dictionary<RoleTypeId, List<Player>> Escapes { get; set; } = new()
        {
            [RoleTypeId.ClassD] = new(),
            [RoleTypeId.Scientist] = new(),
        };

        /// <summary>
        /// Gets or sets the most recent spawn unit.
        /// </summary>
        public string MostRecentSpawnUnit { get; set; } = string.Empty;

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
        /// Gets or sets a list of strings indicating round-disabled features for players.
        /// </summary>
        public List<PlayerDisable> DisabledPlayerKeys { get; set; } = new();

        /// <summary>
        /// Gets a List of infection rules.
        /// </summary>
        public List<InfectRule> InfectionRules { get; } = new();

        /// <summary>
        /// Gets a dictionary of spawn rules.
        /// </summary>
        public Dictionary<RoleTypeId, int> SpawnRules { get; } = new();

        /// <summary>
        /// Gets a dictionary of round kills.
        /// </summary>
        public Dictionary<RoleTypeId, int> Kills { get; } = new();

        /// <summary>
        /// Gets a dictionary of round kills by specific players.
        /// </summary>
        public Dictionary<Player, int> PlayerKills { get; } = new();

        /// <summary>
        /// Gets a dictionary of players with locked radio settings.
        /// </summary>
        public Dictionary<Player, RadioRange> LockedRadios { get; } = new();

        /// <summary>
        /// Gets  a dictionary of permanent player-specific effects.
        /// </summary>
        public Dictionary<Player, List<Effect>> PermPlayerEffects { get; } = new();

        /// <summary>
        /// Gets  a dictionary of permanent team-specific effects.
        /// </summary>
        public Dictionary<Team, List<Effect>> PermTeamEffects { get; } = new();

        /// <summary>
        /// Gets a dictionary of permanent role-specific effects.
        /// </summary>
        public Dictionary<RoleTypeId, List<Effect>> PermRoleEffects { get; } = new();

        public List<DamageRule> DamageRules { get; } = new();

        public override void Init()
        {
            base.Init();
            PlayerHandler.ChangingRole += OnChangingRole;
            PlayerHandler.Hurting += OnHurting;
            PlayerHandler.Died += OnDied;
            PlayerHandler.Dying += OnDying;
            PlayerHandler.TriggeringTesla += OnTriggeringTesla;
            PlayerHandler.Shooting += OnShooting;
            PlayerHandler.DroppingAmmo += OnDroppingItem;
            PlayerHandler.DroppingItem += OnDroppingItem;
            PlayerHandler.SearchingPickup += OnSearchingPickup;
            PlayerHandler.InteractingDoor += OnInteractingDoor;
            PlayerHandler.InteractingLocker += OnInteractingLocker;
            PlayerHandler.InteractingElevator += OnInteractingElevator;
            PlayerHandler.Escaping += OnEscaping;
            PlayerHandler.Spawned += OnSpawned;

            PlayerHandler.PickingUpItem += OnPickingUpItem;
            PlayerHandler.ChangingRadioPreset += OnChangingRadioPreset;

            PlayerHandler.ActivatingWarheadPanel += OnActivatingWarheadPanel;
            Exiled.Events.Handlers.Warhead.Starting += OnStartingWarhead; // why is this located specially??

            PlayerHandler.ActivatingGenerator += GeneratorEvent;
            PlayerHandler.OpeningGenerator += GeneratorEvent;
            PlayerHandler.StoppingGenerator += GeneratorEvent;
            PlayerHandler.UnlockingGenerator += GeneratorEvent;

            PlayerHandler.EnteringEnvironmentalHazard += OnHazardEvent;
            PlayerHandler.ExitingEnvironmentalHazard += OnHazardEvent;

            PlayerHandler.ActivatingWorkstation += OnWorkStationEvent;
            PlayerHandler.DeactivatingWorkstation += OnWorkStationEvent;

            MapHandler.AnnouncingNtfEntrance += OnAnnouncingNtfEntrance;
            MapHandler.AnnouncingScpTermination += OnAnnouncingScpTermination;
            Scp330Handler.InteractingScp330 += OnScp330Event;

            Scp914Handler.Activating += OnScp914Event;
            Scp914Handler.ChangingKnobSetting += OnScp914Event;
            Scp914Handler.UpgradingPickup += OnScp914Event;
            Scp914Handler.UpgradingInventoryItem += OnScp914Event;
            Scp914Handler.UpgradingPlayer += OnScp914Event;

            ServerHandler.RestartingRound += OnRestarting;
            ServerHandler.RoundStarted += OnRoundStarted;
            ServerHandler.RespawningTeam += OnRespawningTeam;

            // SCP abilities
            Scp049Handler.ActivatingSense += OnScpAbility;
            Scp049Handler.Attacking += OnScpAbility;
            Scp049Handler.StartingRecall += OnScpAbility;
            Scp049Handler.SendingCall += OnScpAbility;
            Scp0492Handler.ConsumingCorpse += OnScpAbility;
            Scp0492Handler.TriggeringBloodlust += OnScpAbility;
            Scp079Handler.ChangingCamera += OnScpAbility;
            Scp079Handler.ChangingSpeakerStatus += OnScpAbility;
            Scp079Handler.ElevatorTeleporting += OnScpAbility;
            Scp079Handler.GainingExperience += OnScpAbility;
            Scp079Handler.GainingLevel += OnScpAbility;
            Scp079Handler.InteractingTesla += OnScpAbility;
            Scp079Handler.LockingDown += OnScpAbility;
            Scp079Handler.Pinging += OnScpAbility;
            Scp079Handler.RoomBlackout += OnScpAbility;
            Scp079Handler.TriggeringDoor += OnScpAbility;
            Scp079Handler.ZoneBlackout += OnScpAbility;
            Scp096Handler.AddingTarget += OnScpAbility;
            Scp096Handler.Charging += OnScpAbility;
            Scp096Handler.Enraging += OnScpAbility;
            Scp096Handler.TryingNotToCry += OnScpAbility;
            Scp106Handler.Attacking += OnScpAbility;
            Scp106Handler.Teleporting += OnScpAbility;
            Scp106Handler.Stalking += OnScpAbility;
            Scp173Handler.Blinking += OnScpAbility;
            Scp173Handler.PlacingTantrum += OnScpAbility;
            Scp173Handler.UsingBreakneckSpeeds += OnScpAbility;
            Scp939Handler.ChangingFocus += OnScpAbility;
            Scp939Handler.PlacingAmnesticCloud += OnScpAbility;
            Scp939Handler.PlayingSound += OnScpAbility;
            Scp939Handler.PlayingVoice += OnScpAbility;
            Scp939Handler.SavingVoice += OnScpAbility;
            Scp3114Handler.TryUseBody += OnScpAbility;
        }

        public override void Kill()
        {
            base.Kill();
            PlayerHandler.ChangingRole -= OnChangingRole;
            PlayerHandler.Hurting -= OnHurting;
            PlayerHandler.Died -= OnDied;
            PlayerHandler.Dying -= OnDying;
            PlayerHandler.TriggeringTesla -= OnTriggeringTesla;
            PlayerHandler.Shooting -= OnShooting;
            PlayerHandler.DroppingAmmo -= OnDroppingItem;
            PlayerHandler.DroppingItem -= OnDroppingItem;
            PlayerHandler.SearchingPickup -= OnSearchingPickup;
            PlayerHandler.InteractingDoor -= OnInteractingDoor;
            PlayerHandler.InteractingLocker -= OnInteractingLocker;
            PlayerHandler.InteractingElevator -= OnInteractingElevator;
            PlayerHandler.Escaping -= OnEscaping;
            PlayerHandler.Spawned -= OnSpawned;

            PlayerHandler.PickingUpItem -= OnPickingUpItem;
            PlayerHandler.ChangingRadioPreset -= OnChangingRadioPreset;

            PlayerHandler.ActivatingWarheadPanel -= OnActivatingWarheadPanel;
            Exiled.Events.Handlers.Warhead.Starting -= OnStartingWarhead; // why is this located specially??

            PlayerHandler.ActivatingGenerator -= GeneratorEvent;
            PlayerHandler.OpeningGenerator -= GeneratorEvent;
            PlayerHandler.StoppingGenerator -= GeneratorEvent;
            PlayerHandler.UnlockingGenerator -= GeneratorEvent;

            PlayerHandler.EnteringEnvironmentalHazard -= OnHazardEvent;
            PlayerHandler.ExitingEnvironmentalHazard -= OnHazardEvent;

            PlayerHandler.ActivatingWorkstation -= OnWorkStationEvent;
            PlayerHandler.DeactivatingWorkstation -= OnWorkStationEvent;

            MapHandler.AnnouncingNtfEntrance -= OnAnnouncingNtfEntrance;
            MapHandler.AnnouncingScpTermination -= OnAnnouncingScpTermination;

            Scp330Handler.InteractingScp330 -= OnScp330Event;

            Scp914Handler.Activating -= OnScp914Event;
            Scp914Handler.ChangingKnobSetting -= OnScp914Event;
            Scp914Handler.UpgradingPickup -= OnScp914Event;
            Scp914Handler.UpgradingInventoryItem -= OnScp914Event;
            Scp914Handler.UpgradingPlayer -= OnScp914Event;

            ServerHandler.RestartingRound -= OnRestarting;
            ServerHandler.RoundStarted -= OnRoundStarted;
            ServerHandler.RespawningTeam -= OnRespawningTeam;

            // SCP abilities
            Scp049Handler.ActivatingSense -= OnScpAbility;
            Scp049Handler.Attacking -= OnScpAbility;
            Scp049Handler.StartingRecall -= OnScpAbility;
            Scp049Handler.SendingCall -= OnScpAbility;
            Scp0492Handler.ConsumingCorpse -= OnScpAbility;
            Scp0492Handler.TriggeringBloodlust -= OnScpAbility;
            Scp079Handler.ChangingCamera -= OnScpAbility;
            Scp079Handler.ChangingSpeakerStatus -= OnScpAbility;
            Scp079Handler.ElevatorTeleporting -= OnScpAbility;
            Scp079Handler.GainingExperience -= OnScpAbility;
            Scp079Handler.GainingLevel -= OnScpAbility;
            Scp079Handler.InteractingTesla -= OnScpAbility;
            Scp079Handler.LockingDown -= OnScpAbility;
            Scp079Handler.Pinging -= OnScpAbility;
            Scp079Handler.RoomBlackout -= OnScpAbility;
            Scp079Handler.TriggeringDoor -= OnScpAbility;
            Scp079Handler.ZoneBlackout -= OnScpAbility;
            Scp096Handler.AddingTarget -= OnScpAbility;
            Scp096Handler.Charging -= OnScpAbility;
            Scp096Handler.Enraging -= OnScpAbility;
            Scp096Handler.TryingNotToCry -= OnScpAbility;
            Scp106Handler.Attacking -= OnScpAbility;
            Scp106Handler.Teleporting -= OnScpAbility;
            Scp106Handler.Stalking -= OnScpAbility;
            Scp173Handler.Blinking -= OnScpAbility;
            Scp173Handler.PlacingTantrum -= OnScpAbility;
            Scp173Handler.UsingBreakneckSpeeds -= OnScpAbility;
            Scp939Handler.ChangingFocus -= OnScpAbility;
            Scp939Handler.PlacingAmnesticCloud -= OnScpAbility;
            Scp939Handler.PlayingSound -= OnScpAbility;
            Scp939Handler.PlayingVoice -= OnScpAbility;
            Scp939Handler.SavingVoice -= OnScpAbility;
            Scp3114Handler.TryUseBody -= OnScpAbility;
        }

        // Helpful method
        public PlayerDisable? GetPlayerDisableRule(string key)
        {
            foreach (PlayerDisable playerDisable in DisabledPlayerKeys)
            {
                if (key.Equals(playerDisable.Key))
                {
                    return playerDisable;
                }
            }

            return null;
        }

        public PlayerDisable? GetPlayerDisableRule(string key, Player player)
        {
            if (player is null)
                return null;

            foreach (PlayerDisable playerDisable in DisabledPlayerKeys)
            {
                if (key.Equals(playerDisable.Key) && playerDisable.Players.Contains(player))
                {
                    return playerDisable;
                }
            }

            return null;
        }

        public bool DisabledForPlayer(string key, Player player)
        {
            return GetPlayerDisableRule(key, player).HasValue;
        }

        public void OnRestarting()
        {
            RespawnWaves = 0;
            lastRespawnWave = DateTime.MinValue;
            TeslasDisabled = false;
            MostRecentSpawnUnit = string.Empty;

            SpawnsByTeam[SpawnableTeamType.NineTailedFox] = 0;
            SpawnsByTeam[SpawnableTeamType.ChaosInsurgency] = 0;

            foreach (var escapedRole in Escapes)
            {
                escapedRole.Value.Clear();
            }

            foreach (Commands.CustomCommand cmd in MainPlugin.CustomCommands)
            {
                cmd.ResetCooldowns();
            }

            MainPlugin.ScriptModule.StopAllScripts();
            VariableSystemV2.ClearVariables();
            Kills.Clear();
            PlayerKills.Clear();
            LockedRadios.Clear();

            DisabledPlayerKeys.Clear();
            DisabledKeys.Clear();

            PermPlayerEffects.Clear();
            PermTeamEffects.Clear();
            PermRoleEffects.Clear();

            DamageRules.Clear();

            InfectionRules.Clear();
            SpawnRules.Clear();
            RecentlyRespawned.Clear();

            MostRecentSpawn = SpawnableTeamType.None;
        }

        public void OnRoundStarted()
        {
            if (SpawnRules.Count > 0)
            {
                List<Player> players = Player.List.Where(p => p.IsConnected && p.Role.Type is not RoleTypeId.Overwatch).ToList();
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
        }

        public void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            if (DisabledKeys.Contains("RESPAWNS")) ev.IsAllowed = false;

            if (!ev.IsAllowed) return;

            RespawnWaves++;
            lastRespawnWave = DateTime.UtcNow;

            MostRecentSpawn = ev.NextKnownTeam;
            SpawnsByTeam[ev.NextKnownTeam]++;

            RecentlyRespawned.Clear();
            RecentlyRespawned.AddRange(ev.Players);
        }

        // Perm Effects: Spawned
        public void OnSpawned(SpawnedEventArgs ev)
        {
            if (PermPlayerEffects.TryGetValue(ev.Player, out var effects))
            {
                effects.ForEach(eff => ev.Player.SyncEffect(eff));
            }

            if (PermTeamEffects.TryGetValue(ev.Player.Role.Team, out var effects2))
            {
                effects2.ForEach(eff => ev.Player.SyncEffect(eff));
            }

            if (PermRoleEffects.TryGetValue(ev.Player.Role.Type, out var effects3))
            {
                effects3.ForEach(eff => ev.Player.SyncEffect(eff));
            }
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
            if (TeslasDisabled || DisabledKeys.Contains("TESLAS") || DisabledForPlayer("TESLAS", ev.Player))
            {
                ev.IsAllowed = false;
            }
        }

        // Locked Radios
        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (!ev.IsAllowed) return;

            if (LockedRadios.ContainsKey(ev.Player))
            {
                LockedRadios.Remove(ev.Player);
            }
        }

        // Disable Stuff
        public void OnDying(DyingEventArgs ev)
        {
            if (DisabledKeys.Contains("DYING") || DisabledForPlayer("DYING", ev.Player))
                ev.IsAllowed = false;

            if (!ev.IsAllowed)
                return;

            if (ev.Attacker is not null)
            {
                if (Kills.ContainsKey(ev.Attacker.Role.Type))
                    Kills[ev.Attacker.Role.Type]++;
                else
                    Kills.Add(ev.Attacker.Role.Type, 1);

                if (PlayerKills.ContainsKey(ev.Attacker))
                    PlayerKills[ev.Attacker]++;
                else
                    PlayerKills.Add(ev.Attacker, 1);
            }
        }

        public void OnHurting(HurtingEventArgs ev)
        {
            if (DisabledKeys.Contains("HURTING") || DisabledForPlayer("HURTING", ev.Player))
                ev.IsAllowed = false;

            if (ev.Attacker is null || ev.Player is null || ev.Attacker == Server.Host)
                return;

            // SCP-049 & SCP-106 handled by OnScpAbility method
            if ((ev.Attacker.Role.Type is RoleTypeId.Scp0492 && DisabledKeys.Contains("SCP0492ATTACK")) ||
                (ev.Attacker.Role.Type is RoleTypeId.Scp096 && DisabledKeys.Contains("SCP096ATTACK")) ||
                (ev.Attacker.Role.Type is RoleTypeId.Scp173 && DisabledKeys.Contains("SCP173ATTACK")) ||
                (ev.Attacker.Role.Type is RoleTypeId.Scp939 && DisabledKeys.Contains("SCP939ATTACK")) ||
                (ev.Attacker.Role.Type is RoleTypeId.Scp3114 && DisabledKeys.Contains("SCP3114ATTACK")) ||
                (ev.Attacker.Role.Team is Team.SCPs && DisabledKeys.Contains("SCPATTACK")) ||
                (ev.Attacker.Role.Team is Team.SCPs && DisabledKeys.Contains("SCPALLABILITIES")))
                ev.IsAllowed = false;

            // Damage Rules
            foreach (DamageRule rule in DamageRules)
            {
                float multiplier = rule.DetermineMultiplier(ev.Attacker, ev.Player);
                ev.Amount *= multiplier;
            }
        }

        public void GeneratorEvent(IGeneratorEvent ev)
        {
            if (ev is IDeniableEvent deniable)
            {
                if (DisabledKeys.Contains("GENERATORS"))
                    deniable.IsAllowed = false;
                if (ev is IPlayerEvent plrEvent && DisabledForPlayer("GENERATORS", plrEvent.Player))
                    deniable.IsAllowed = false;
            }
        }

        public void OnShooting(ShootingEventArgs ev)
        {
            if (DisabledKeys.Contains("SHOOTING") || DisabledForPlayer("SHOOTING", ev.Player))
                ev.IsAllowed = false;
        }

        public void OnDroppingItem(IDeniableEvent ev)
        {
            if (DisabledKeys.Contains("DROPPING"))
                ev.IsAllowed = false;
            if (ev is IPlayerEvent plrEv && DisabledForPlayer("DROPPING", plrEv.Player))
                ev.IsAllowed = false;
        }

        public void OnSearchingPickup(SearchingPickupEventArgs ev)
        {
            if (DisabledKeys.Contains("ITEMPICKUPS") || DisabledForPlayer("ITEMPICKUPS", ev.Player))
                ev.IsAllowed = false;

            if ((DisabledKeys.Contains("MICROPICKUPS") || DisabledForPlayer("MICROPICKUPS", ev.Player)) && ev.Pickup.Type is ItemType.MicroHID)
                ev.IsAllowed = false;
        }

        public void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (DisabledKeys.Contains("DOORS") || DisabledForPlayer("DOORS", ev.Player))
                ev.IsAllowed = false;
        }

        public void OnHandcuffing(HandcuffingEventArgs ev)
        {
            if (DisabledKeys.Contains("CUFFING") || DisabledForPlayer("CUFFING", ev.Player))
                ev.IsAllowed = false;
        }

        public void OnInteractingLocker(InteractingLockerEventArgs ev)
        {
            if (ev.Locker is PedestalScpLocker && (DisabledKeys.Contains("PEDESTALS") || DisabledForPlayer("PEDESTALS", ev.Player)))
            {
                ev.IsAllowed = false;
            }
            else if (ev.Locker is not PedestalScpLocker && (DisabledKeys.Contains("LOCKERS") || DisabledForPlayer("LOCKERS", ev.Player)))
            {
                ev.IsAllowed = false;
            }
        }

        public void OnEscaping(EscapingEventArgs ev)
        {
            if (DisabledKeys.Contains("ESCAPING") || DisabledForPlayer("ESCAPING", ev.Player))
                ev.IsAllowed = false;

            if (!ev.IsAllowed) return;

            if (!Escapes.ContainsKey(ev.Player.Role.Type))
                Escapes.Add(ev.Player.Role.Type, new());

            Escapes[ev.Player.Role.Type].Add(ev.Player);
        }

        // Radio locks
        public void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            if (!ev.IsAllowed) return;

            if (ev.Pickup is RadioPickup radio && LockedRadios.TryGetValue(ev.Player, out RadioRange range))
            {
                radio.Range = range;
            }
        }

        public void OnChangingRadioPreset(ChangingRadioPresetEventArgs ev)
        {
            if (LockedRadios.ContainsKey(ev.Player))
                ev.IsAllowed = false;
        }

        public void OnInteractingElevator(InteractingElevatorEventArgs ev)
        {
            if (DisabledKeys.Contains("ELEVATORS") || DisabledForPlayer("ELEVATORS", ev.Player))
                ev.IsAllowed = false;
        }

        public void OnHazardEvent(IHazardEvent ev)
        {
            if (ev is IDeniableEvent deny)
            {
                if (DisabledKeys.Contains("HAZARDS"))
                    deny.IsAllowed = false;
                else if (ev is IPlayerEvent plrEv && DisabledForPlayer("HAZARDS", plrEv.Player))
                    deny.IsAllowed = false;
            }
        }

        public void OnWorkStationEvent(IDeniableEvent ev)
        {
            if (DisabledKeys.Contains("WORKSTATIONS"))
                ev.IsAllowed = false;
            else if (ev is IPlayerEvent plrEv && DisabledForPlayer("WORKSTATIONS", plrEv.Player))
                ev.IsAllowed = false;
        }

        public void OnScp330Event(IDeniableEvent ev)
        {
            if (DisabledKeys.Contains("SCP330"))
                ev.IsAllowed = false;
            else if (ev is IPlayerEvent plrEv && DisabledForPlayer("SCP330", plrEv.Player))
                ev.IsAllowed = false;
        }

        public void OnScp914Event(IDeniableEvent ev)
        {
            if (DisabledKeys.Contains("SCP914"))
                ev.IsAllowed = false;
            else if (ev is IPlayerEvent plrEv && DisabledForPlayer("SCP914", plrEv.Player))
                ev.IsAllowed = false;
        }

#pragma warning disable SA1201
        public static Dictionary<Type, string> EventToDisableKey { get; } = new()
#pragma warning restore SA1201
        {
            // SCP-049
            [typeof(ActivatingSenseEventArgs)] = "SCP049SENSE",
            [typeof(Exiled.Events.EventArgs.Scp049.AttackingEventArgs)] = "SCP049ATTACK",
            [typeof(StartingRecallEventArgs)] = "SCP049RECALL",
            [typeof(SendingCallEventArgs)] = "SCP049CALL",

            // SCP-049-2
            [typeof(ConsumingCorpseEventArgs)] = "SCP0492CONSUMECORPSE",
            [typeof(TriggeringBloodlustEventArgs)] = "SCP0492BLOODLUST",

            // SCP-079
            // [typeof(ChangingCameraEventArgs)] = "SCP079CHANGECAMERA",
            [typeof(ChangingSpeakerStatusEventArgs)] = "SCP079SPEAKER",
            [typeof(ElevatorTeleportingEventArgs)] = "SCP079ELEVATOR",
            // [typeof(GainingExperienceEventArgs)] = "SCP079GAINEXPERIENCE",
            // [typeof(GainingLevelEventArgs)] = "SCP079GAINLEVEL",
            [typeof(InteractingTeslaEventArgs)] = "SCP079TESLA",
            [typeof(LockingDownEventArgs)] = "SCP079LOCKDOWN",
            [typeof(PingingEventArgs)] = "SCP079PING",
            [typeof(RoomBlackoutEventArgs)] = "SCP079BLACKOUT",
            [typeof(TriggeringDoorEventArgs)] = "SCP079DOOR",
            [typeof(ZoneBlackoutEventArgs)] = "SCP079ZONEBLACKOUT",

            // SCP-096
            [typeof(AddingTargetEventArgs)] = "SCP096ADDTARGET",
            [typeof(ChargingEventArgs)] = "SCP096CHARGE",
            [typeof(EnragingEventArgs)] = "SCP096ENRAGE",
            [typeof(TryingNotToCryEventArgs)] = "SCP096TRYNOTCRY",

            // SCP-106
            [typeof(Exiled.Events.EventArgs.Scp106.AttackingEventArgs)] = "SCP106ATTACK",
            [typeof(TeleportingEventArgs)] = "SCP106ATLAS",
            [typeof(StalkingEventArgs)] = "SCP106STALK",

            // SCP-173
            [typeof(BlinkingEventArgs)] = "SCP173BLINK",
            [typeof(PlacingTantrumEventArgs)] = "SCP173TANTRUM",
            [typeof(UsingBreakneckSpeedsEventArgs)] = "SCP173BREAKNECKSPEED",

            // SCP-939
            [typeof(ChangingFocusEventArgs)] = "SCP939FOCUS",
            [typeof(PlacingAmnesticCloudEventArgs)] = "SCP939CLOUD",
            [typeof(PlayingSoundEventArgs)] = "SCP939PLAYSOUND",
            [typeof(PlayingVoiceEventArgs)] = "SCP939PLAYVOICE",
            [typeof(SavingVoiceEventArgs)] = "SCP939SAVEVOICE",

            // SCP-3114
            [typeof(TryUseBodyEventArgs)] = "SCP3114DISGUISE",
            [typeof(DisguisingEventArgs)] = "SCP3114DISGUISE",
        };

        public void OnScpAbility(IDeniableEvent ev)
        {
            if (ev is IPlayerEvent playerEv)
            {
                if (EventToDisableKey.TryGetValue(ev.GetType(), out string key2) && (DisabledForPlayer(key2, playerEv.Player) || (key2 is "SCP106ATTACK" or "SCP049ATTACK" && DisabledForPlayer("SCPATTACK", playerEv.Player))))
                    ev.IsAllowed = false;
                if (DisabledForPlayer("SCPALLABILITIES", playerEv.Player))
                    ev.IsAllowed = false;
            }

            if (EventToDisableKey.TryGetValue(ev.GetType(), out string key) && (DisabledKeys.Contains(key) || (key is "SCP106ATTACK" or "SCP049ATTACK" && DisabledKeys.Contains("SCPATTACK"))))
                ev.IsAllowed = false;
            if (DisabledKeys.Contains("SCPALLABILITIES"))
                ev.IsAllowed = false;
        }

        public void OnAnnouncingNtfEntrance(AnnouncingNtfEntranceEventArgs ev)
        {
            MostRecentSpawnUnit = ev.UnitName;

            if (DisabledKeys.Contains("NTFANNOUNCEMENT"))
                ev.IsAllowed = false;
        }

        public void OnAnnouncingScpTermination(AnnouncingScpTerminationEventArgs ev)
        {
            if (DisabledKeys.Contains("SCPANNOUNCEMENT"))
                ev.IsAllowed = false;
        }

        public void OnStartingWarhead(StartingEventArgs ev)
        {
            if (DisabledKeys.Contains("WARHEAD") || DisabledForPlayer("WARHEAD", ev.Player))
                ev.IsAllowed = false;
        }

        public void OnActivatingWarheadPanel(ActivatingWarheadPanelEventArgs ev)
        {
            if (DisabledKeys.Contains("WARHEAD") || DisabledForPlayer("WARHEAD", ev.Player))
                ev.IsAllowed = false;
        }
    }
}
