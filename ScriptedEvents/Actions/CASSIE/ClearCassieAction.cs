namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class ClearCassieAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "CLEARCASSIE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

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
