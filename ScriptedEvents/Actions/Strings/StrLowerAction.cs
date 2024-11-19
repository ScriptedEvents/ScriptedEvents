using System;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Strings
{
    public class StrLowerAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "StrToLower";

        /// <inheritdoc/>
        public string Description => "Returns the provided string where all 'UPPERCASE' letters are replaced with 'lowercase' ones.";

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
            new Argument("string", typeof(string), "The string to lowercase.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            return new(true, new(Arguments[0]!.ToString().ToLower()));
        }
    }
}