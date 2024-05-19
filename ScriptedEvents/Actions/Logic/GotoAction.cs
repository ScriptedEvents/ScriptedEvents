namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class GotoAction : IScriptAction, ILogicAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "GOTO";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Logic;

        /// <inheritdoc/>
        public string Description => "Moves to the provided label. Use 'START' to go to the start of the script.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("label", typeof(string), "The label to move to.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string label = (string)Arguments[0];

            if (script.JumpToLabel(label))
            {
                return new(true);
            }

            int prevLine = script.CurrentLine;
            if (script.JumpToFunctionLabel(label))
            {
                script.JumpLines.Add(prevLine);
                return new(true);
            }

            return new(false, "Invalid label provided.");
        }
    }
}
