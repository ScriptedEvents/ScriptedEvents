namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.Actions.Samples.Interfaces;
    using ScriptedEvents.Actions.Samples.Providers;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class EndIfAction : IScriptAction, ILogicAction, IHelpInfo, IIgnoreSkipAction
    {
        /// <inheritdoc/>
        public string Name => "ENDIF";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Logic;

        /// <inheritdoc/>
        public string Description => "dev ver no inf";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments { get; set; }

        /// <inheritdoc/>
        public ISampleProvider Samples { get; } = new IfSamples();

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            script.SkipExecution = false;
            return new(true);
        }
    }
}