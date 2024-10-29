using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions
{
    using System;
    using ScriptedEvents.Structures;

    /// <inheritdoc/>
    public class StrCountAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "STR-COUNT";

        /// <inheritdoc/>
        public string Description => "Returns the number of occurences of a string are in a given string.";

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
            new Argument("variable", typeof(string), "The variable on which the operation will be performed.", true),
            new Argument("sequence", typeof(string), "The sequence which will be counted in the given string.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            // c# really has no method for this for fuck sake
            static int CountOccurrences(string text, string substring)
            {
                int count = 0;
                int index = 0;

                while ((index = text.IndexOf(substring, index)) != -1)
                {
                    count++;
                    index += substring.Length;
                }

                return count;
            }

            return new(true, variablesToRet: new[] { CountOccurrences((string)Arguments[0], (string)Arguments[1]).ToString() });
        }
    }
}