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
            new Argument("goBackSave", typeof(bool), "If TRUE, the line in which this action is situated in will be saved to the GOBACK stack. To learn more, read GOBACK action.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            int curLine = script.CurrentLine;
            string label = (string)Arguments[0];

            if (!script.Jump(label))
            {
                return new(false, "Invalid label provided.");
            }

            if (Arguments.Length >= 2 && (bool)Arguments[1])
            {
                script.CallLines.Add(curLine);
            }

            return new(true);
        }
    }
}
