namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;

    using ScriptedEvents.API.Constants;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class GotoAction : IScriptAction, ILogicAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "GOTO";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Logic;

        /// <inheritdoc/>
        public string Description => "Moves to the provided label.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("label", typeof(string), "The label to move to, or keyword (START/NEXT/STOP)", false),
        };

        /// <inheritdoc/>
        public string LongDescription => ConstMessages.GotoInput;

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!script.Jump(Arguments.ElementAt(0)))
            {
                if (Arguments[0].ToUpper() == "STOP")
                    return new(true, flags: ActionFlags.StopEventExecution);
                return new(false, "Invalid line or label provided!");
            }

            return new(true);
        }
    }
}
