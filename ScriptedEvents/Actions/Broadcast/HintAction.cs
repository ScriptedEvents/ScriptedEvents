namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;

    public class HintAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "HINT";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Broadcast;

        /// <inheritdoc/>
        public string Description => "Broadcasts a hint to every player.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("duration", typeof(float), "The duration of the message. Variables are supported.", true),
            new Argument("message", typeof(string), "The message. Variables are supported.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            float duration = (float)Arguments[0];

            string message = VariableSystemV2.ReplaceVariables(Arguments.JoinMessage(1), script);
            MainPlugin.ScriptModule.ShowHint(message, duration);
            return new(true);
        }
    }
}