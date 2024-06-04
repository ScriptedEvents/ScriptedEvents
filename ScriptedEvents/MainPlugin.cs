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
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.DemoScripts;
    using ScriptedEvents.Structures;

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

        private static List<SEModule> modules = new();

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

        public static IEnumerable<SEModule> Modules => modules.Where(mod => mod.IsActive);

        public static ScriptModule ScriptModule => GetModule<ScriptModule>();

        public static EventHandlingModule Handlers => GetModule<EventHandlingModule>();

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
            where T : SEModule => (T)Modules.FirstOrDefault(m => m.GetType() == typeof(T));

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
                    modules.Add(module);
                }
            }

            Timing.CallDelayed(6f, () =>
            {
                if (IsExperimental)
                {
#if ADEBUG
                    Log.Warn($"You are using a pre-release version of {Name} by Elektryk_Andrzej. Please report any issues encountered, thank you.");
#else
                    Log.Warn($"You are using a pre-release version of {Name}. Please report any issues encountered, thank you.");
#endif
                }

                if (DateTime.Now.Month == 1 && DateTime.Now.Day == 25)
                {
                    Log.Info(Constants.ItsMyBirthday);
                }

                bool isUpdated = API.Features.ScriptHelpGenerator.Generator.CheckUpdated(out string docMessage);
                if (docMessage != "SKIP")
                {
                    if (isUpdated)
                        Log.Info("[DOCUMENTATION GENERATOR]: " + docMessage);
                    else
                        Log.Warn("[DOCUMENTATION GENERATOR]: " + docMessage);
                }
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
                    Log.Warn($"The removal of the '{helpPath}' file has failed. Reason: {ex}");
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
                    Log.Warn($"The removal of the '{API.Features.ScriptHelpGenerator.Generator.ConfigPath}' file has failed. Reason: {ex}");
                }
            }
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            Handlers.OnRestarting();
            base.OnDisabled();

            foreach (SEModule module in Modules)
            {
                module.Kill();
            }

            Singleton = null;
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
