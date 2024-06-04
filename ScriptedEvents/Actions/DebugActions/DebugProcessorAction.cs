namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class DebugProcessorAction : IScriptAction, IHiddenAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "DEBUGPROCESSOR";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Debug;

        /// <inheritdoc/>
        public string Description => string.Empty;

        /// <inheritdoc/>
        public Argument[] ExpectedArguments { get; } = new[]
        {
            new Argument("input", typeof(string), string.Empty, true),
            new Argument("number", typeof(bool), string.Empty, true),
            new Argument("players", typeof(PlayerCollection), string.Empty, true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            ArgumentProcessResult result = ArgumentProcessor.Process(ExpectedArguments, RawArguments, this, script);
            return new(true, result.Message.ToString());
        }
    }
}