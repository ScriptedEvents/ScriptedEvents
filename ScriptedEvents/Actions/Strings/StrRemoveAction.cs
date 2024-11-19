using System;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Strings
{
    public class StrRemoveAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "StrRemove";

        /// <inheritdoc/>
        public string Description => "Returns the provided string where all the occurences of the specified string to remove are removed.";

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
            new Argument("subjectString", typeof(string), "The string to perform the operation on.", true),
            new Argument("stringToRemove", typeof(string), "The string occurance to remove.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            return new(true, new(((string)Arguments[0]!).Replace((string)Arguments[1]!, string.Empty)));
        }
    }
}