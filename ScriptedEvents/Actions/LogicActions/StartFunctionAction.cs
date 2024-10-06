namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class StartFunctionAction : IScriptAction, IHiddenAction
    {
        /// <inheritdoc/>
        public string Name => "FUNC";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Logic;

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => Array.Empty<Argument>();

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            return new(false, "A function can only work when called using the GOTO action.", ActionFlags.FatalError);
        }
    }
}
