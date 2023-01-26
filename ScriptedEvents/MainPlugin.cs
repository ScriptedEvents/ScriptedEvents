using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using System;
using System.IO;

using ServerHandler = Exiled.Events.Handlers.Server;

namespace ScriptedEvents
{
    public class MainPlugin : Plugin<Config>
    {
        public override string Name => "ScriptedEvents";
        public override string Author => "Thunder";
        public override Version Version => new(0, 1, 0);
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

            ScriptHelper.Setup();
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
    }
}
