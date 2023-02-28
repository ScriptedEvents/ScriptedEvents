namespace ScriptedEvents.Actions
{
    using System;
    using Exiled.API.Features;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Handlers;

    public class ClearCassieAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "CLEARCASSIE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Cassie;

        /// <inheritdoc/>
        public string Description => "Clears cassie queue.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => Array.Empty<Argument>();

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Cassie.Clear();
            return new(true);
        }
    }
}
