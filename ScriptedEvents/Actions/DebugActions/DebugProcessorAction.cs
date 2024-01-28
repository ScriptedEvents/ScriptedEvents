namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;
    using InventorySystem.Items.Usables.Scp330;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class DebugProcessorAction : IScriptAction, IHiddenAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "DEBUGPROCESSOR";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Debug;

        /// <inheritdoc/>
        public string Description => string.Empty;

        /// <inheritdoc/>
        public Argument[] ExpectedArguments { get; } = new[]
        {
            new Argument("input", typeof(string), string.Empty, true),
            new Argument("number", typeof(bool), string.Empty, true),
            new Argument("players", typeof(Player[]), string.Empty, true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            ArgumentProcessResult result = ArgumentProcessor.Process(ExpectedArguments, Arguments, this, script);

            return new(true, result.Message.ToString());
        }
    }
}