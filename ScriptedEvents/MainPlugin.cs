﻿namespace ScriptedEvents
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    using Exiled.Events;
    using Exiled.Events.Features;
    using Exiled.Loader;

    using MEC;
    using RemoteAdmin;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Commands;
    using ScriptedEvents.DemoScripts;
    using ScriptedEvents.Variables;

    using Event = Exiled.Events.Features.Event;
    using MapHandler = Exiled.Events.Handlers.Map;
    using PlayerHandler = Exiled.Events.Handlers.Player;
    using Scp330Handler = Exiled.Events.Handlers.Scp330;
    using Scp914Handler = Exiled.Events.Handlers.Scp914;
    using ServerHandler = Exiled.Events.Handlers.Server;

    public class MainPlugin : Plugin<Config>
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
        /// Gets or sets the Event Handlers singleton.
        /// </summary>
        public static EventHandlers Handlers { get; set; }

        /// <summary>
        /// Gets a list of demo scripts.
        /// </summary>
        public static IDemoScript[] DemoScripts { get; } = new IDemoScript[]
        {
            new About(),
            new DemoScript(),
            new ConditionSamples(),
        };

        /// <summary>
        /// Gets an array of Event "Handler" types defined by Exiled.
        /// </summary>
        public static Type[] HandlerTypes { get; } = Loader.Plugins.First(plug => plug.Name == "Exiled.Events")
            .Assembly.GetTypes().Where(t => t.FullName.Equals($"Exiled.Events.Handlers.{t.Name}")).ToArray();

        public static List<Tuple<EventInfo, Delegate>> StoredDelegates { get; } = new();

        /// <inheritdoc/>
        public override string Name => "ScriptedEvents";

        /// <inheritdoc/>
        public override string Author => "Thunder + Johnodon";

        /// <inheritdoc/>
        public override Version Version => new(2, 4, 1);

        /// <inheritdoc/>
        public override Version RequiredExiledVersion => new(8, 2, 1);

        /// <inheritdoc/>
        public override PluginPriority Priority => PluginPriority.High;

        /// <summary>
        /// Equivalent to <see cref="Log.Info(string)"/>, but checks the EnableLogs ScriptedEvents config first.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Info(string message)
        {
            if (Singleton.Config.EnableLogs)
                Log.Info(message);
        }

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            base.OnEnabled();

            Singleton = this;
            Handlers = new();

            if (!Directory.Exists(ScriptHelper.ScriptPath))
            {
                try
                {
                    DirectoryInfo info = Directory.CreateDirectory(ScriptHelper.ScriptPath);
                    DirectoryInfo demoScriptFolder = Directory.CreateDirectory(Path.Combine(info.FullName, "DemoScripts"));
                    foreach (IDemoScript demo in DemoScripts)
                    {
                        File.WriteAllText(Path.Combine(demoScriptFolder.FullName, $"{demo.FileName}.txt"), demo.Contents);
                    }
                }
                catch (UnauthorizedAccessException e)
                {
                    Log.Error($"Unable to create the required ScriptedEvents directories due to a permission error. Please ensure that ScriptedEvents has proper system permissions to Exiled's Config folder. [Error Code: SE-106] Full error: {e}");
                    return;
                }
                catch (Exception e)
                {
                    Log.Error($"Unable to load ScriptedEvents due to a directory error. [Error Code: SE-107] {e}");
                    return;
                }

                // Welcome message :)
                // 3s delay to show after other console spam
                Timing.CallDelayed(3f, () =>
                {
                    Log.Warn($"Thank you for installing Scripted Events! View the README file located at {ScriptHelper.ScriptPath} for information on how to use and get the most out of this plugin.");
                });
            }

            if (IsExperimental)
            {
                Log.Warn($"This {Name} DLL is marked as Experimental. Use at your own risk; expect bugs and issues.");
            }

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

            // Setup systems
            ApiHelper.RegisterActions();
            VariableSystem.Setup();

            // "On" config
            foreach (KeyValuePair<string, List<string>> ev in Configs.On)
            {
                bool made = false;
                foreach (Type handler in HandlerTypes)
                {
                    // Credit to DevTools for below code.
                    Delegate @delegate = null;
                    PropertyInfo propertyInfo = handler.GetProperty(ev.Key);

                    if (propertyInfo is null)
                        continue;

                    EventInfo eventInfo = propertyInfo.PropertyType.GetEvent("InnerEvent", (BindingFlags)(-1));
                    MethodInfo subscribe = propertyInfo.PropertyType.GetMethod("Subscribe");

                    if (propertyInfo.PropertyType == typeof(Event))
                    {
                        @delegate = new CustomEventHandler(EventHandlers.OnNonArgumentedEvent);
                    }
                    else if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Event<>))
                    {
                        @delegate = typeof(EventHandlers)
                            .GetMethod(nameof(EventHandlers.OnArgumentedEvent))
                            .MakeGenericMethod(eventInfo.EventHandlerType.GenericTypeArguments)
                            .CreateDelegate(typeof(CustomEventHandler<>)
                            .MakeGenericType(eventInfo.EventHandlerType.GenericTypeArguments));
                    }
                    else
                    {
                        Log.Warn(propertyInfo.Name);
                        continue;
                    }

                    subscribe.Invoke(propertyInfo.GetValue(Handlers), new object[] { @delegate });
                    StoredDelegates.Add(new Tuple<EventInfo, Delegate>(eventInfo, @delegate));
                    made = true;

                    if (!made)
                    {
                        Log.Warn($"The specified event '{ev.Key}' in the 'On' config was not found! [Error Code: SE-108]");
                    }

                    // Delete help file on startup
                    string helpPath = Path.Combine(ScriptHelper.ScriptPath, "HelpCommandResponse.txt");
                    if (File.Exists(helpPath))
                        File.Delete(helpPath);
                }
            }
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            Handlers.OnRestarting();
            base.OnDisabled();

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

            for (int i = 0; i < StoredDelegates.Count; i++)
            {
                Tuple<EventInfo, Delegate> tuple = StoredDelegates[i];
                EventInfo eventInfo = tuple.Item1;
                Delegate handler = tuple.Item2;

                if (eventInfo.DeclaringType != null)
                {
                    MethodInfo removeMethod = eventInfo.DeclaringType.GetMethod($"remove_{eventInfo.Name}", BindingFlags.Instance | BindingFlags.NonPublic);
                    removeMethod.Invoke(null, new object[] { handler });
                }
                else
                {
                    MethodInfo removeMethod = eventInfo.GetRemoveMethod(true);
                    removeMethod.Invoke(null, new[] { handler });
                }

                StoredDelegates.Remove(tuple);
            }

            ScriptHelper.StopAllScripts();
            ScriptHelper.ActionTypes.Clear();

            Singleton = null;
            Handlers = null;
        }

        public override void OnRegisteringCommands()
        {
            foreach (Structures.CustomCommand custom in Config.Commands)
            {
                if (custom.Name == string.Empty)
                {
                    Log.Warn($"Custom command is defined without a name. [Error Code: SE-128]");
                    continue;
                }

                if (custom.Run is null || custom.Run.Count == 0)
                {
                    Log.Warn($"Custom command '{custom.Name}' ({custom.Type}) will not be created because it is set to run zero scripts. [Error Code: SE-129]");
                    continue;
                }

                CustomCommand command = new()
                {
                    Command = custom.Name,
                    Description = custom.Description,
                    Aliases = new string[0],
                    Type = custom.Type,
                    Permission = custom.Permission == string.Empty ? "script.command" : "script.command." + custom.Permission,
                    Scripts = custom.Run.ToArray(),
                };

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
