namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class EndFunctionAction : IScriptAction, IHiddenAction
    {
        /// <inheritdoc/>
        public string Name => "ENDFUNC";

        /// <inheritdoc/>
        public string[] Aliases => new[] { "<-" };

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
            if (script.FunctionLabelHistory.Count == 0)
                return new(false, ErrorGen.Generate(ErrorCode.InvalidActionUsage, "Closing function syntax.", "Closing syntax must be used after a function was called using the GOTO action."));

            int lineNum = script.FunctionLabelHistory.Last();
            script.Jump(lineNum + 1);
            script.FunctionLabelHistory.Remove(lineNum);

            return new(true);
        }
    }
}
