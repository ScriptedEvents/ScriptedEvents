using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using ScriptedEvents.API.Features.Aliases;
using ScriptedEvents.API.Helpers;
using ScriptedEvents.Handlers;
using System.ComponentModel;
using System.Collections.ObjectModel;
using ScriptedEvents.DemoScripts;

using PlayerHandler = Exiled.Events.Handlers.Player;
using ServerHandler = Exiled.Events.Handlers.Server;

namespace ScriptedEvents
{
    public class MainPlugin : Plugin<Config>
    {
        public override string Name => "ScriptedEvents";
        public override string Author => "Thunder+Johnodon";
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
                var info = Directory.CreateDirectory(ScriptHelper.ScriptPath);
                foreach (var demo in DemoScripts)
                {
                    File.WriteAllText(Path.Combine(info.FullName, $"{demo.FileName}.txt"), demo.Contents);
                }

                // Welcome message :)
                Log.Warn($"Thank you for installing Scripted Events! View the README file located at {ScriptHelper.ScriptPath} for information on how to use and get the most out of this plugin.");
            }

            if (IsExperimental)
            {
                Log.Warn($"This ScriptedEvents DLL is marked as Experimental. Use at your own risk; expect bugs and issues.");
            }

            PlayerHandler.Died += Handlers.OnDied;

            ServerHandler.RestartingRound += Handlers.OnRestarting;
            ServerHandler.RoundStarted += Handlers.OnRoundStarted;
            ServerHandler.RespawningTeam += Handlers.OnRespawningTeam;

            ApiHelper.RegisterActions();
        }

        public override void OnDisabled()
        {
            base.OnDisabled();

            PlayerHandler.Died -= Handlers.OnDied;

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

    public class Config : IConfig
    {
        [Description("Whether or not to enable the Scripted Events plugin.")]
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("Enable logs for starting/stopping scripts.")]
        public bool EnableLogs { get; set; } = true;

        [Description("List of scripts to run as soon as the round starts.")]
        public List<string> AutoRunScripts { get; set; } = new();

        [Description("List of scripts to automatically re-run as soon as they finish.")]
        public List<string> LoopScripts { get; set; } = new();

        // todo: un-alias door commands, because they dont have duration anymore
        [Description("Define a custom set of actions and the action they run when used.")]
        public List<Alias> Aliases { get; set; } = new()
        {
            new("LOCKDOORBRIEF", "DOOR LOCK * 10")
        };

        [Description("Define a custom set of permissions used to run a certain script. The provided permission will be added AFTER script.execute (eg. script.execute.examplepermission for the provided example).")]
        public Dictionary<string, string> RequiredPermissions { get; set; } = new()
        {
            { "ExampleScriptNameHere", "examplepermission" },
        };
    }
}
