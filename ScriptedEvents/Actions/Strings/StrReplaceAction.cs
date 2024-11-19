using System;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Strings
{
    public class StrReplaceAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "StrReplace";

        /// <inheritdoc/>
        public string Description => "Replaces character sequneces in a given string.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.String;

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("subjectString", typeof(string), "The string on which the operation will be performed.", true),
            new Argument("targetSequence", typeof(string), "The sequence to replace.", true),
            new Argument("replacingSequence", typeof(string), "The value to replace the sequence with.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            return new(true, new(((string)Arguments[0]!).Replace((string)Arguments[1]!, (string)Arguments[2]!)));
        }
    }
}