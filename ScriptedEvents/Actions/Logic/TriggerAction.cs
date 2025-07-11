﻿using System;
using ScriptedEvents.API.Enums;
using ScriptedEvents.API.Interfaces;
using ScriptedEvents.API.Modules;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Logic
{
    public class TriggerAction : IScriptAction, ILogicAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "TRIGGER";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Logic;

        /// <inheritdoc/>
        public string Description => "Triggers a custom event, running scripts subscribed to that event.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("event", typeof(string), "Custom event name. This does not trigger Exiled events.", true),
        };

        public ScriptModule ScriptModule => MainPlugin.GetModule<ScriptModule>();

        public EventScriptModule ESModule => MainPlugin.GetModule<EventScriptModule>();

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string ev = (string)Arguments[0];

            if (!ESModule.CurrentCustomEventData.ContainsKey(ev))
            {
                return new(false, $"Provided custom event '{ev}' is not used in any scripts. Please ensure that there are scripts assigned to this event using the !-- CUSTOMEVENT flag.");
            }

            foreach (string scriptName in ESModule.CurrentCustomEventData[ev])
            {
                MainPlugin.ScriptModule.ReadAndRun(scriptName, script.Sender);
            }

            return new(true);
        }
    }
}