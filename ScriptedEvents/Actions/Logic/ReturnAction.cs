namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class ReturnAction : IScriptAction, ILogicAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "RETURN";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Logic;

        /// <inheritdoc/>
        public string Description => "Moves to the last CALL action executed.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments { get; set; }

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (script.CallLines.Count == 0)
                return new(false, "This action must be run after the CALL action.");

            int lastLine = script.CallLines.Count - 1;

            script.Jump(script.CallLines[lastLine] + 1);
            script.CallLines.RemoveAt(lastLine);

            return new(true);
        }
    }
}
