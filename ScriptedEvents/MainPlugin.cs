using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ServerHandler = Exiled.Events.Handlers.Server;
using ScriptedEvents.API.Features.Aliases;
using ScriptedEvents.API.Helpers;
using ScriptedEvents.Handlers;

namespace ScriptedEvents
{
    public class MainPlugin : Plugin<Config>
    {
        public override string Name => "ScriptedEvents";
        public override string Author => "Thunder";
        public override Version Version => new(0, 2, 0);
        public override Version RequiredExiledVersion => new(6, 0, 0);
        public override PluginPriority Priority => PluginPriority.High;


        public static MainPlugin Singleton;
        public static EventHandlers Handlers;

        public override void OnEnabled()
        {
            Singleton = this;
            Handlers = new();

            if (!Directory.Exists(ScriptHelper.ScriptPath))
            {
                var info = Directory.CreateDirectory(ScriptHelper.ScriptPath);
                File.WriteAllText(Path.Combine(info.FullName, "DemoScript.txt"), DemoScript.Demo);
            }

            ServerHandler.RestartingRound += Handlers.OnRestarting;

            ScriptHelper.RegisterActions(Assembly.GetExecutingAssembly());
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            ServerHandler.RestartingRound -= Handlers.OnRestarting;

            ScriptHelper.ActionTypes.Clear();

            Singleton = null;
            Handlers = null;
            
            base.OnDisabled();
        }
    }

    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        public List<Alias> Aliases { get; set; } = new()
        {
            new("TURNOFFLIGHTS", "COMMAND /blackout", "zone", "seconds"),
            new("LOCKDOOR", "COMMAND /lock", "door"),
            new("OPENDOOR", "COMMAND /open", "door"),
            new("CLOSEDOOR", "COMMAND /close", "door"),
            new("LOCK", "COMMAND /lock", "door"),
            new("UNLOCK", "COMMAND /unlock", "door")
        };
    }
}
