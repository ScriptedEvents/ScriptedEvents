using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions
{
    using System;
    using ScriptedEvents.Structures;

    /// <inheritdoc/>
    public class StrUpperAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "STR-UPPER";

        /// <inheritdoc/>
        public string Description => "Returns the provided string where all lowercase letters are replaced with UPPERCASE ones.";

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
            new Argument("string", typeof(string), "The string to UPPERCASE.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            return new(true, variablesToRet: new[] { Arguments[0].ToString().ToUpper() });
        }
    }
}