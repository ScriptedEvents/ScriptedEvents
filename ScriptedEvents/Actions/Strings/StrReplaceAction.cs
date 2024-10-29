using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions
{
    using System;
    using ScriptedEvents.Structures;

    /// <inheritdoc/>
    public class StrReplaceAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "STR-REPLACE";

        /// <inheritdoc/>
        public string Description => "Replaces character sequneces in a given string.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.String;

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("variableName", typeof(string), "The string on which the operation will be performed.", true),
            new Argument("targetSequence", typeof(string), "The sequence which will be replaced.", true),
            new Argument("replacingSequence", typeof(string), "The value to replace with.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            return new(true, variablesToRet: new[] { ((string)Arguments[0]).Replace(Arguments[1].ToString(), Arguments[2].ToString()) });
        }
    }
}