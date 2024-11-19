using System;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Strings
{
    public class StrCountAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "StrCount";

        /// <inheritdoc/>
        public string Description => "Returns the number of occurences of a string that are in a given string.";

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
            new Argument("subject", typeof(string), "The value on which the operation will be performed.", true),
            new Argument("searchedSequence", typeof(string), "The sequence which will be counted in the given subject string.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            return new(true, new(CountOccurrences((string)Arguments[0]!, (string)Arguments[1]!).ToString()));

            // c# really has no method for this for fuck sake
            static int CountOccurrences(string text, string substring)
            {
                int count = 0;
                int index = 0;

                while ((index = text.IndexOf(substring, index, StringComparison.Ordinal)) != -1)
                {
                    count++;
                    index += substring.Length;
                }

                return count;
            }
        }
    }
}