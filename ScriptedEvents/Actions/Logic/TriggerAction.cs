namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

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

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string ev = (string)Arguments[0];

            foreach (string scriptName in MainPlugin.CurrentCustomEventData[ev])
            {
                ScriptModule.ReadAndRun(scriptName, script.Sender);
            }

            return new(true);
        }
    }
}