using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using System;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Strings
{
    public class StrSliceAction : IScriptAction, IHelpInfo, IReturnValueAction
    {
        /// <inheritdoc/>
        public string Name => "StrSlice";

        /// <inheritdoc/>
        public string Description => "Slices the provided string using custom rules.";

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
            new Argument("startSliceAmount", typeof(int), "The amount of characters to slice off from the START of the string.", true, ArgFlag.BiggerOrEqual0),
            new Argument("endSliceAmount", typeof(int), "The amount of characters to slice off from the END of the string.", true, ArgFlag.BiggerOrEqual0),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            var subject = (string)Arguments[0]!;
            var startSlice = (int)Arguments[1]!;
            var endSlice = (int)Arguments[2]!;

            if (subject.Length - startSlice - endSlice <= 0)
            {
                var err = new ErrorInfo(
                    "Slicing not possible",
                    $"The provided string '{subject}' doesnt contain enough characters to slice it with values 'startSlice {startSlice}' and endSlice '{endSlice}'.",
                    Name).ToTrace();
                return new(false, null, err);
            }

            var toRet = subject.Substring(startSlice, subject.Length - startSlice - endSlice);
            return new(true, new(toRet));
        }
    }
}