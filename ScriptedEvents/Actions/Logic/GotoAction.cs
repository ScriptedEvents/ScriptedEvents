namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Constants;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class GotoAction : IScriptAction, ILogicAction, IHelpInfo, ILongDescription, IIgnoresSkipAction
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
        public string Description => "Moves to the provided label.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("label", typeof(string), "The label to move to.", true),
        };

        /// <inheritdoc/>
        public string LongDescription => ConstMessages.GotoInput;

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            script.SkipExecution = false;
            string label = (string)Arguments[0];

            if (!script.Jump(label))
            {
                return new(false, "Invalid label provided.");
            }

            return new(true);
        }
    }
}
