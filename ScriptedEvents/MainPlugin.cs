namespace ScriptedEvents
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using MEC;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.DemoScripts;
    using ScriptedEvents.Variables.Handlers;
    using PlayerHandler = Exiled.Events.Handlers.Player;
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

        /// <inheritdoc/>
        public override string Name => "ScriptedEvents";

        /// <inheritdoc/>
        public override string Author => "Thunder + Johnodon";

        /// <inheritdoc/>
        public override Version Version => new(2, 0, 0);

        /// <inheritdoc/>
        public override Version RequiredExiledVersion => new(6, 0, 0);

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
                DirectoryInfo info = Directory.CreateDirectory(ScriptHelper.ScriptPath);
                DirectoryInfo demoScriptFolder = Directory.CreateDirectory(Path.Combine(info.FullName, "DemoScripts"));
                foreach (IDemoScript demo in DemoScripts)
                {
                    File.WriteAllText(Path.Combine(demoScriptFolder.FullName, $"{demo.FileName}.txt"), demo.Contents);
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
                Log.Warn($"This ScriptedEvents DLL is marked as Experimental. Use at your own risk; expect bugs and issues.");
            }

            PlayerHandler.Died += Handlers.OnDied;
            PlayerHandler.TriggeringTesla += Handlers.OnTriggeringTesla;

            ServerHandler.RestartingRound += Handlers.OnRestarting;
            ServerHandler.RoundStarted += Handlers.OnRoundStarted;
            ServerHandler.RespawningTeam += Handlers.OnRespawningTeam;

            ApiHelper.RegisterActions();
            ConditionVariables.Setup();
            PlayerVariables.Setup();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            base.OnDisabled();

            PlayerHandler.Died -= Handlers.OnDied;
            PlayerHandler.TriggeringTesla -= Handlers.OnTriggeringTesla;

            ServerHandler.RestartingRound -= Handlers.OnRestarting;
            ServerHandler.RoundStarted -= Handlers.OnRoundStarted;
            ServerHandler.RespawningTeam -= Handlers.OnRespawningTeam;

            ScriptHelper.ActionTypes.Clear();

            Singleton = null;
            Handlers = null;
        }
    }
}
