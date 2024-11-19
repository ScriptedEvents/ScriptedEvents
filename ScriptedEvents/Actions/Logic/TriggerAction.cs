﻿namespace ScriptedEvents.Actions.Logic
{
    using System;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Enums;
    using ScriptedEvents.Interfaces;
    using ScriptedEvents.Structures;

    public class TriggerAction : IScriptAction, ILogicAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "Trigger";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

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
            string ev = (string)Arguments[0]!;

            foreach (string scriptName in ESModule.CurrentCustomEventData[ev])
            {
                ScriptModule.Singleton!.TryReadAndRun(scriptName, null, out _);
            }

            return new(true);
        }
    }
}