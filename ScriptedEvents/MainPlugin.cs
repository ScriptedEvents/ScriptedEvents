﻿using Exiled.API.Enums;
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
using System.ComponentModel;

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
            ServerHandler.RoundStarted += Handlers.OnRoundStarted;

            ScriptHelper.RegisterActions(Assembly.GetExecutingAssembly());
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            ServerHandler.RestartingRound -= Handlers.OnRestarting;
            ServerHandler.RoundStarted -= Handlers.OnRoundStarted;

            ScriptHelper.ActionTypes.Clear();

            Singleton = null;
            Handlers = null;
            
            base.OnDisabled();
        }
    }

    public class Config : IConfig
    {
        [Description("Whether or not to enable the Scripted Events plugin.")]
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("List of scripts to run as soon as the round starts.")]
        public List<string> AutoRunScripts { get; set; } = new();

        // todo: un-alias door commands, because they dont have duration anymore
        public List<Alias> Aliases { get; set; } = new()
        {
            new("LIGHTSOFF", "COMMAND /overcharge", "zone", "seconds"),
            new("LOCKDOOR", "COMMAND /lock", "door"),
            new("OPENDOOR", "COMMAND /open", "door"),
            new("CLOSEDOOR", "COMMAND /close", "door"),
            new("LOCKDOOR", "COMMAND /lock", "door"),
            new("UNLOCKDOOR", "COMMAND /unlock", "door"),
            new("DESTROYDOOR", "COMMAND /destroy", "door"),
            new("DETONATEWARHEAD", "COMMAND /SERVER_EVENT DETONATION_INSTANT"),
        };
    }
}
