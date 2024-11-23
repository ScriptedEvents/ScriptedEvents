using ScriptedEvents.Enums;

namespace ScriptedEvents
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    using MEC;
    using RemoteAdmin;
    using ScriptedEvents.API.Constants;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.DemoScripts;
    using ScriptedEvents.Structures;

    public class MainPlugin : Plugin<Config, Translations>
    {
        /// <summary>
        /// Whether this build of the plugin is experimental.
        /// </summary>
#if ADEBUG || DEBUG
        public const bool IsExperimental = true;
#else
        public const bool IsExperimental = false;
#endif

        /// <summary>
        /// Gets a link to the Scripted Events Discord server.
        /// </summary>
        public const string DiscordUrl = "https://discord.gg/3j54zBnbbD";

        /// <summary>
        /// Gets a link to the plugin's GitHub page.
        /// </summary>
        public const string GitHub = "https://github.com/Thundermaker300/ScriptedEvents";

        private static readonly List<SEModule> InternalModules = new();

        /// <summary>
        /// Gets the plugin singleton.
        /// </summary>
        public static MainPlugin? Singleton { get; private set; }

        /// <summary>
        /// Gets the plugin Config singleton.
        /// </summary>
        public static Config Configs => Singleton!.Config;

        /// <summary>
        /// Gets the plugin Translations singleton.
        /// </summary>
        public static Translations Translations => Singleton!.Translation;

        /// <summary>
        /// Gets a list of demo scripts.
        /// </summary>
        public static IDemoScript[] DemoScripts { get; } = new IDemoScript[]
        {
            new About(),
            new ConditionSamples(),
            new DemoScript(),
            new DogHideAndSeek(),
            new HitMarker(),
            new JoinBroadcast(),
            new PeanutRun(),
            new ScpLeftInfo()
        };

        public static DateTime Epoch => new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static List<Commands.CustomCommand> CustomCommands { get; } = new();

        /// <summary>
        /// The base path to SE files.
        /// </summary>
        public static readonly string BaseFilePath = Path.Combine(Paths.Configs, "ScriptedEvents");

        /// <inheritdoc/>
        public override string Name => "ScriptedEvents";

        /// <inheritdoc/>
        public override string Author => "Elektryk_Andrzej and Thunder";

        /// <inheritdoc/>
        public override Version Version => new(4, 0, 0);

        /// <inheritdoc/>
        public override Version RequiredExiledVersion => new(8, 7, 0);

        /// <inheritdoc/>
        public override PluginPriority Priority => PluginPriority.High;

        public static IEnumerable<SEModule> Modules => InternalModules.Where(mod => mod.IsActive);

        /// <summary>
        /// Equivalent to <see cref="Logger.Info(string)"/>, but checks the EnableLogs ScriptedEvents config first.
        /// </summary>
        /// <param name="message">The message to Logger.</param>
        public static void Info(string message)
        {
            if (Singleton.Config.EnableLogs)
                Logger.Info(message);
        }

        public static T? GetModule<T>()
            where T : SEModule => Modules.FirstOrDefault(m => m.GetType() == typeof(T)) as T;

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            base.OnEnabled();

            Singleton = this;

            foreach (Type type in Assembly.GetTypes())
            {
                if (type.BaseType == typeof(SEModule) && type.IsClass && type.GetConstructors().Length > 0)
                {
                    SEModule module = (SEModule)Activator.CreateInstance(type);

                    if (module.ShouldGenerateFiles)
                        module.GenerateFiles();

                    module.Init();
                    InternalModules.Add(module);
                }
            }

            Timing.CallDelayed(6f, () =>
            {
                if (IsExperimental)
                {
#if ADEBUG
                    Logger.Warn($"You are using a pre-release version of {Name} by Elektryk_Andrzej. Please report any issues encountered, thank you.");
#else
                    Logger.Warn($"You are using a pre-release version of {Name}. Please report any issues encountered, thank you.");
#endif
                }

                if (DateTime.Now.Month == 1 && DateTime.Now.Day == 25)
                {
                    Logger.Info(Constants.ItsMyBirthday);
                }

                var isUpdated = API.Features.ScriptHelpGenerator.Generator.CheckUpdated(out var docMessage);
                if (docMessage == "SKIP") return;

                if (isUpdated)
                    Logger.Info("[DOCUMENTATION GENERATOR]: " + docMessage);
                else
                    Logger.Warn("[DOCUMENTATION GENERATOR]: " + docMessage);
            });

            // Delete help file on startup
            string helpPath = Path.Combine(BaseFilePath, "HelpCommandResponse.txt");
            if (File.Exists(helpPath))
            {
                try
                {
                    File.Delete(helpPath);
                }
                catch (Exception ex)
                {
                    Logger.Warn($"The removal of the '{helpPath}' file has failed. Reason: {ex}");
                }
            }

            // Delete leftover doc config on startup
            if (File.Exists(API.Features.ScriptHelpGenerator.Generator.ConfigPath))
            {
                try
                {
                    File.Delete(API.Features.ScriptHelpGenerator.Generator.ConfigPath);
                }
                catch (Exception ex)
                {
                    Logger.Warn($"The removal of the '{API.Features.ScriptHelpGenerator.Generator.ConfigPath}' file has failed. Reason: {ex}");
                }
            }

#if ADEBUG
            API.ScriptedEventsIntegration.RegisterCustomActions();
#endif
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            EventHandlingModule.Singleton!.OnRestarting();
            base.OnDisabled();

            foreach (SEModule module in Modules)
            {
                module.Kill();
            }

            Singleton = null;

#if ADEBUG
            API.ScriptedEventsIntegration.UnregisterCustomActions();
#endif
        }

        public override void OnRegisteringCommands()
        {
            foreach (CustomCommand custom in Config.Commands)
            {
                if (!custom.Enabled) continue;

                if (custom.Name == string.Empty)
                {
                    Logger.Error("One of your custom commands doesnt have a name!");
                    continue;
                }

                if (custom.Run is null || custom.Run.Count == 0)
                {
                    Logger.Error($"Your custom command '{custom.Name}' doesnt have an action to run!");
                    continue;
                }

                if (custom.Cooldown != -1 && custom.PlayerCooldown != -1)
                {
                    Logger.Error($"Your custom command '{custom.Name}' has misconfigured cooldown! Both cooldown and player cooldown can't be set to -1.");
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
                    Aliases = Array.Empty<string>(),
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
