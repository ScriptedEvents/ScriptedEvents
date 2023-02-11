namespace ScriptedEvents
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.DemoScripts;

    using PlayerHandler = Exiled.Events.Handlers.Player;
    using ServerHandler = Exiled.Events.Handlers.Server;

    public class MainPlugin : Plugin<Config>
    {
        public override string Name => "ScriptedEvents";
        public override string Author => "Thunder + Johnodon";
        public override Version Version => new(1, 1, 0);
        public override Version RequiredExiledVersion => new(6, 0, 0);
        public override PluginPriority Priority => PluginPriority.High;

        public const bool IsExperimental = true;

        public static ReadOnlyCollection<IDemoScript> DemoScripts { get; } = new List<IDemoScript>()
        {
            new About(),
            new DemoScript(),
            new ConditionSamples(),
        }.AsReadOnly();

        public static MainPlugin Singleton;
        public static EventHandlers Handlers;

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
                Log.Warn($"Thank you for installing Scripted Events! View the README file located at {ScriptHelper.ScriptPath} for information on how to use and get the most out of this plugin.");
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
        }

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

        public static void Info(string message)
        {
            if (Singleton.Config.EnableLogs)
                Log.Info(message);
        }
    }
}
