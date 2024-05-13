namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class GoBackAction : IScriptAction, ILogicAction, IHelpInfo, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "GOBACK";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Logic;

        /// <inheritdoc/>
        public string Description => "Moves to the last GOTO action executed which used the 'goBackSave' setting. When multiple GOTO actions were used with 'goBackSave', GOBACK will go to the most recent one.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => Array.Empty<Argument>();

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (script.CallLines.Count == 0)
                return new(false, "No lines to go back to!");

            int lineNum = script.CallLines.Last();
            script.Jump(lineNum);
            script.CallLines.Remove(lineNum);

            return new(true);
        }
    }
}
