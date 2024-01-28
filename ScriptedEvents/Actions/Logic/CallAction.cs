namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;

    using ScriptedEvents.API.Constants;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Strings;

    public class CallAction : IScriptAction, ILogicAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "CALL";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Logic;

        /// <inheritdoc/>
        public string Description => "Moves to the provided label. When RETURN action is used, script will continue to execute from the last CALL action.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("label", typeof(string), "The label to move to.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            int curLine = script.CurrentLine;

            if (!script.Jump(Arguments.ElementAt(0)))
            {
                return new(false, "Invalid line or label provided!");
            }

            script.CallLines.Add(curLine);

            script.DebugLog(script.CallLines[0].ToString());

            return new(true);
        }
    }
}