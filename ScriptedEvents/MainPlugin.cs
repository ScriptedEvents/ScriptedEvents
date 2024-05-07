namespace ScriptedEvents
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    using Exiled.Events.Handlers;

    using MEC;
    using RemoteAdmin;
    using ScriptedEvents.API.Constants;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.DemoScripts;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    using MapHandler = Exiled.Events.Handlers.Map;
    using PlayerHandler = Exiled.Events.Handlers.Player;
    using Scp330Handler = Exiled.Events.Handlers.Scp330;
    using Scp914Handler = Exiled.Events.Handlers.Scp914;
    using ServerHandler = Exiled.Events.Handlers.Server;

    public class MainPlugin : Plugin<Config, Translations>
    {
        /// <summary>
        /// Whether or not this build of the plugin is experimental.
        /// </summary>
        public const bool IsExperimental = true;

        /// <summary>
        /// Gets a link to the Scripted Events Discord server.
        /// </summary>
        public const string DiscordUrl = "https://discord.gg/3j54zBnbbD";

        /// <summary>
        /// Gets a link to the plugin's GitHub page.
        /// </summary>
        public const string GitHub = "https://github.com/Thundermaker300/ScriptedEvents";

        /// <summary>
        /// Gets or sets the plugin singleton.
        /// </summary>
        public static MainPlugin Singleton { get; set; }

        /// <summary>
        /// Gets the plugin Config singleton.
        /// </summary>
        public static Config Configs => Singleton?.Config;

        /// <summary>
        /// Gets the plugin Translations singleton.
        /// </summary>
        public static Translations Translations => Singleton?.Translation;

        /// <summary>
        /// Gets or sets the Event Handlers singleton.
        /// </summary>
        public static EventHandlers Handlers { get; set; }

        /// <summary>
        /// Gets a list of demo scripts.
        /// </summary>
        public static IDemoScript[] DemoScripts { get; } = new IDemoScript[]
        {
            new DemoScript(),
            new ConditionSamples(),
        };

        public static DateTime Epoch => new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static List<Commands.CustomCommand> CustomCommands { get; } = new();

        /// <summary>
        /// The base path to SE files.
        /// </summary>
        public static readonly string BaseFilePath = Path.Combine(Paths.Configs, "ScriptedEvents");

        /// <inheritdoc/>
        public override string Name => "ScriptedEvents";

        /// <inheritdoc/>
        public override string Author => "Thunder + Elektryk_Andrzej";

        /// <inheritdoc/>
        public override Version Version => new(3, 0, 0);

        /// <inheritdoc/>
        public override Version RequiredExiledVersion => new(8, 7, 0);

        /// <inheritdoc/>
        public override PluginPriority Priority => PluginPriority.High;

        internal static List<string> AutorunScripts { get; set; }

        public static List<IModule> Modules { get; } = new()
        {
            new ScriptModule(),
            new EventScriptModule(),
        };

        /// <summary>
        /// Equivalent to <see cref="Log.Info(string)"/>, but checks the EnableLogs ScriptedEvents config first.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Info(string message)
        {
            if (Singleton.Config.EnableLogs)
                Log.Info(message);
        }

        public static T GetModule<T>()
            where T : IModule => (T)Modules.FirstOrDefault(m => m.GetType() == typeof(T));

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            base.OnEnabled();

            Singleton = this;
            Handlers = new();

            foreach (IModule module in Modules)
            {
                if (module.ShouldGenerateFiles)
                    module.GenerateFiles();

                module.Init();
            }

            Timing.CallDelayed(6f, () =>
            {
                if (IsExperimental)
                {
#if ADEBUG
                    Log.Warn($"This DLL is an experimental pre-release version of {Name} by Elektryk_Andrzej.");
#else
                    Log.Warn($"This {Name} DLL is marked as Experimental. Use at your own risk; expect bugs and issues.");
#endif
                }

                if (DateTime.Now.Month == 1 && DateTime.Now.Day == 25)
                {
                    Log.Info(Constants.ItsMyBirthday);
                }
            });

            PlayerHandler.ChangingRole += Handlers.OnChangingRole;
            PlayerHandler.Hurting += Handlers.OnHurting;
            PlayerHandler.Died += Handlers.OnDied;
            PlayerHandler.Dying += Handlers.OnDying;
            PlayerHandler.TriggeringTesla += Handlers.OnTriggeringTesla;
            PlayerHandler.Shooting += Handlers.OnShooting;
            PlayerHandler.DroppingAmmo += Handlers.OnDroppingItem;
            PlayerHandler.DroppingItem += Handlers.OnDroppingItem;
            PlayerHandler.SearchingPickup += Handlers.OnSearchingPickup;
            PlayerHandler.InteractingDoor += Handlers.OnInteractingDoor;
            PlayerHandler.InteractingLocker += Handlers.OnInteractingLocker;
            PlayerHandler.InteractingElevator += Handlers.OnInteractingElevator;
            PlayerHandler.Escaping += Handlers.OnEscaping;
            PlayerHandler.Spawned += Handlers.OnSpawned;

            PlayerHandler.PickingUpItem += Handlers.OnPickingUpItem;
            PlayerHandler.ChangingRadioPreset += Handlers.OnChangingRadioPreset;

            PlayerHandler.ActivatingWarheadPanel += Handlers.OnActivatingWarheadPanel;
            Exiled.Events.Handlers.Warhead.Starting += Handlers.OnStartingWarhead; // why is this located specially??

            PlayerHandler.ActivatingGenerator += Handlers.GeneratorEvent;
            PlayerHandler.OpeningGenerator += Handlers.GeneratorEvent;
            PlayerHandler.StoppingGenerator += Handlers.GeneratorEvent;
            PlayerHandler.UnlockingGenerator += Handlers.GeneratorEvent;

            PlayerHandler.EnteringEnvironmentalHazard += Handlers.OnHazardEvent;
            PlayerHandler.ExitingEnvironmentalHazard += Handlers.OnHazardEvent;

            PlayerHandler.ActivatingWorkstation += Handlers.OnWorkStationEvent;
            PlayerHandler.DeactivatingWorkstation += Handlers.OnWorkStationEvent;

            MapHandler.AnnouncingNtfEntrance += Handlers.OnAnnouncingNtfEntrance;
            MapHandler.AnnouncingScpTermination += Handlers.OnAnnouncingScpTermination;

            Scp330Handler.InteractingScp330 += Handlers.OnScp330Event;

            Scp914Handler.Activating += Handlers.OnScp914Event;
            Scp914Handler.ChangingKnobSetting += Handlers.OnScp914Event;
            Scp914Handler.UpgradingPickup += Handlers.OnScp914Event;
            Scp914Handler.UpgradingInventoryItem += Handlers.OnScp914Event;
            Scp914Handler.UpgradingPlayer += Handlers.OnScp914Event;

            ServerHandler.RestartingRound += Handlers.OnRestarting;
            ServerHandler.WaitingForPlayers += Handlers.OnWaitingForPlayers;
            ServerHandler.RoundStarted += Handlers.OnRoundStarted;
            ServerHandler.RespawningTeam += Handlers.OnRespawningTeam;

            // SCP abilities
            Scp049.ActivatingSense += Handlers.OnScpAbility;
            Scp049.Attacking += Handlers.OnScpAbility;
            Scp049.StartingRecall += Handlers.OnScpAbility;
            Scp049.SendingCall += Handlers.OnScpAbility;
            Scp0492.ConsumingCorpse += Handlers.OnScpAbility;
            Scp0492.TriggeringBloodlust += Handlers.OnScpAbility;
            Scp079.ChangingCamera += Handlers.OnScpAbility;
            Scp079.ChangingSpeakerStatus += Handlers.OnScpAbility;
            Scp079.ElevatorTeleporting += Handlers.OnScpAbility;
            Scp079.GainingExperience += Handlers.OnScpAbility;
            Scp079.GainingLevel += Handlers.OnScpAbility;
            Scp079.InteractingTesla += Handlers.OnScpAbility;
            Scp079.LockingDown += Handlers.OnScpAbility;
            Scp079.Pinging += Handlers.OnScpAbility;
            Scp079.RoomBlackout += Handlers.OnScpAbility;
            Scp079.TriggeringDoor += Handlers.OnScpAbility;
            Scp079.ZoneBlackout += Handlers.OnScpAbility;
            Scp096.AddingTarget += Handlers.OnScpAbility;
            Scp096.Charging += Handlers.OnScpAbility;
            Scp096.Enraging += Handlers.OnScpAbility;
            Scp096.TryingNotToCry += Handlers.OnScpAbility;
            Scp106.Attacking += Handlers.OnScpAbility;
            Scp106.Teleporting += Handlers.OnScpAbility;
            Scp106.Stalking += Handlers.OnScpAbility;
            Scp173.Blinking += Handlers.OnScpAbility;
            Scp173.PlacingTantrum += Handlers.OnScpAbility;
            Scp173.UsingBreakneckSpeeds += Handlers.OnScpAbility;
            Scp939.ChangingFocus += Handlers.OnScpAbility;
            Scp939.PlacingAmnesticCloud += Handlers.OnScpAbility;
            Scp939.PlayingSound += Handlers.OnScpAbility;
            Scp939.PlayingVoice += Handlers.OnScpAbility;
            Scp939.SavingVoice += Handlers.OnScpAbility;
            Scp3114.TryUseBody += Handlers.OnScpAbility;

            // Setup systems
            ApiHelper.RegisterActions();
            VariableSystem.Setup();

            // Delete help file on startup
            string helpPath = Path.Combine(BaseFilePath, "HelpCommandResponse.txt");
            if (File.Exists(helpPath))
                File.Delete(helpPath);
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            Handlers.OnRestarting();
            base.OnDisabled();

            foreach (IModule module in Modules)
            {
                module.Kill();
            }

            PlayerHandler.ChangingRole -= Handlers.OnChangingRole;
            PlayerHandler.Hurting -= Handlers.OnHurting;
            PlayerHandler.Died -= Handlers.OnDied;
            PlayerHandler.Dying -= Handlers.OnDying;
            PlayerHandler.TriggeringTesla -= Handlers.OnTriggeringTesla;
            PlayerHandler.Shooting -= Handlers.OnShooting;
            PlayerHandler.DroppingAmmo -= Handlers.OnDroppingItem;
            PlayerHandler.DroppingItem -= Handlers.OnDroppingItem;
            PlayerHandler.SearchingPickup -= Handlers.OnSearchingPickup;
            PlayerHandler.InteractingDoor -= Handlers.OnInteractingDoor;
            PlayerHandler.InteractingLocker -= Handlers.OnInteractingLocker;
            PlayerHandler.InteractingElevator -= Handlers.OnInteractingElevator;
            PlayerHandler.Escaping -= Handlers.OnEscaping;
            PlayerHandler.Spawned -= Handlers.OnSpawned;

            PlayerHandler.PickingUpItem -= Handlers.OnPickingUpItem;
            PlayerHandler.ChangingRadioPreset -= Handlers.OnChangingRadioPreset;

            PlayerHandler.ActivatingWarheadPanel -= Handlers.OnActivatingWarheadPanel;
            Exiled.Events.Handlers.Warhead.Starting -= Handlers.OnStartingWarhead; // why is this located specially??

            PlayerHandler.ActivatingGenerator -= Handlers.GeneratorEvent;
            PlayerHandler.OpeningGenerator -= Handlers.GeneratorEvent;
            PlayerHandler.StoppingGenerator -= Handlers.GeneratorEvent;
            PlayerHandler.UnlockingGenerator -= Handlers.GeneratorEvent;

            PlayerHandler.EnteringEnvironmentalHazard -= Handlers.OnHazardEvent;
            PlayerHandler.ExitingEnvironmentalHazard -= Handlers.OnHazardEvent;

            PlayerHandler.ActivatingWorkstation -= Handlers.OnWorkStationEvent;
            PlayerHandler.DeactivatingWorkstation -= Handlers.OnWorkStationEvent;

            MapHandler.AnnouncingNtfEntrance -= Handlers.OnAnnouncingNtfEntrance;
            MapHandler.AnnouncingScpTermination -= Handlers.OnAnnouncingScpTermination;

            Scp330Handler.InteractingScp330 -= Handlers.OnScp330Event;

            Scp914Handler.Activating -= Handlers.OnScp914Event;
            Scp914Handler.ChangingKnobSetting -= Handlers.OnScp914Event;
            Scp914Handler.UpgradingPickup -= Handlers.OnScp914Event;
            Scp914Handler.UpgradingInventoryItem -= Handlers.OnScp914Event;
            Scp914Handler.UpgradingPlayer -= Handlers.OnScp914Event;

            ServerHandler.RestartingRound -= Handlers.OnRestarting;
            ServerHandler.WaitingForPlayers -= Handlers.OnWaitingForPlayers;
            ServerHandler.RoundStarted -= Handlers.OnRoundStarted;
            ServerHandler.RespawningTeam -= Handlers.OnRespawningTeam;

            // SCP abilities
            Scp049.ActivatingSense -= Handlers.OnScpAbility;
            Scp049.Attacking -= Handlers.OnScpAbility;
            Scp049.StartingRecall -= Handlers.OnScpAbility;
            Scp049.SendingCall -= Handlers.OnScpAbility;
            Scp0492.ConsumingCorpse -= Handlers.OnScpAbility;
            Scp0492.TriggeringBloodlust -= Handlers.OnScpAbility;
            Scp079.ChangingCamera -= Handlers.OnScpAbility;
            Scp079.ChangingSpeakerStatus -= Handlers.OnScpAbility;
            Scp079.ElevatorTeleporting -= Handlers.OnScpAbility;
            Scp079.GainingExperience -= Handlers.OnScpAbility;
            Scp079.GainingLevel -= Handlers.OnScpAbility;
            Scp079.InteractingTesla -= Handlers.OnScpAbility;
            Scp079.LockingDown -= Handlers.OnScpAbility;
            Scp079.Pinging -= Handlers.OnScpAbility;
            Scp079.RoomBlackout -= Handlers.OnScpAbility;
            Scp079.TriggeringDoor -= Handlers.OnScpAbility;
            Scp079.ZoneBlackout -= Handlers.OnScpAbility;
            Scp096.AddingTarget -= Handlers.OnScpAbility;
            Scp096.Charging -= Handlers.OnScpAbility;
            Scp096.Enraging -= Handlers.OnScpAbility;
            Scp096.TryingNotToCry -= Handlers.OnScpAbility;
            Scp106.Attacking -= Handlers.OnScpAbility;
            Scp106.Teleporting -= Handlers.OnScpAbility;
            Scp106.Stalking -= Handlers.OnScpAbility;
            Scp173.Blinking -= Handlers.OnScpAbility;
            Scp173.PlacingTantrum -= Handlers.OnScpAbility;
            Scp173.UsingBreakneckSpeeds -= Handlers.OnScpAbility;
            Scp939.ChangingFocus -= Handlers.OnScpAbility;
            Scp939.PlacingAmnesticCloud -= Handlers.OnScpAbility;
            Scp939.PlayingSound -= Handlers.OnScpAbility;
            Scp939.PlayingVoice -= Handlers.OnScpAbility;
            Scp939.SavingVoice -= Handlers.OnScpAbility;
            Scp3114.TryUseBody -= Handlers.OnScpAbility;

            Singleton = null;
            Handlers = null;
        }

        public override void OnRegisteringCommands()
        {
            foreach (CustomCommand custom in Config.Commands)
            {
                if (!custom.Enabled) continue;

                if (custom.Name == string.Empty)
                {
                    Log.Warn(ErrorGen.Get(ErrorCode.CustomCommand_NoName));
                    continue;
                }

                if (custom.Run is null || custom.Run.Count == 0)
                {
                    Log.Warn(ErrorGen.Get(ErrorCode.CustomCommand_NoScripts, custom.Name, custom.Type));
                    continue;
                }

                if (custom.Cooldown != -1 && custom.PlayerCooldown != -1)
                {
                    Log.Warn(ErrorGen.Get(ErrorCode.CustomCommand_MultCooldowns, custom.Name, custom.Type));
                    continue;
                }

                CommandCooldownMode cooldownMode = CommandCooldownMode.None;
                int cooldown = -1;
                if (custom.Cooldown != -1)
                {
                    cooldownMode = CommandCooldownMode.Global;
                    cooldown = custom.Cooldown;
                }
                else if (custom.PlayerCooldown != -1)
                {
                    cooldownMode = CommandCooldownMode.Player;
                    cooldown = custom.PlayerCooldown;
                }

                Commands.CustomCommand command = new()
                {
                    Command = custom.Name,
                    Description = custom.Description,
                    Aliases = new string[0],
                    Type = custom.Type,
                    CooldownMode = cooldownMode,
                    Cooldown = cooldown,
                    DoResponse = custom.DefaultResponse,
                    Permission = custom.Permission == string.Empty ? string.Empty : "script.command." + custom.Permission,
                    Scripts = custom.Run.ToArray(),
                };

                CustomCommands.Add(command);

                switch (command.Type)
                {
                    case CommandType.PlayerConsole:
                        QueryProcessor.DotCommandHandler.RegisterCommand(command);
                        break;
                    case CommandType.ServerConsole:
                        GameCore.Console.singleton.ConsoleCommandHandler.RegisterCommand(command);
                        break;
                    case CommandType.RemoteAdmin:
                        CommandProcessor.RemoteAdminCommandHandler.RegisterCommand(command);
                        break;
                }
            }

            base.OnRegisteringCommands();
        }
    }
}
