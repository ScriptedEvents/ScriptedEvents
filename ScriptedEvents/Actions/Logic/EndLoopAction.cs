namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class EndLoopAction : IScriptAction, IHiddenAction
    {
        /// <inheritdoc/>
        public string Name => "ENDLOOP";

        /// <inheritdoc/>
        public string[] Aliases => new[] { "]", "CONTINUE" };

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
            if (script.IsInsideLoopStatement)
            {
                script.Jump(script.LoopStatementStart - 1);
            }

            return new(true);
        }
    }
}
