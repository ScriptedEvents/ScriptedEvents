namespace ScriptedEvents.Actions.Strings
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    /// <inheritdoc/>
    public class StrSliceAction : IScriptAction, IHelpInfo, IReturnValueAction
    {
        /// <inheritdoc/>
        public string Name => "STR-SLICE";

        /// <inheritdoc/>
        public string Description => "Slices the provided string using custom rules.";

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
            new Argument("string", typeof(string), "The string to perform the operation on.", true),
            new Argument("startSlice", typeof(int), "The amount of characters to slice off from the START of the string. Value cannot be negative!", true),
            new Argument("endSlice", typeof(int), "The amount of characters to slice off from the END of the string. Value cannot be negative!", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            var @string = (string)Arguments[0];
            var startSlice = (int)Arguments[1];
            var endSlice = (int)Arguments[2];

            if (startSlice < 0)
            {
                return new(false,
                    $"Argument 'startSlice' must be a non-negative value, but was '{startSlice}'.");
            }

            if (endSlice < 0)
            {
                return new(false,
                    $"Argument 'endSlice' must be a non-negative value, but was '{endSlice}'.");
            }

            if (@string.Length - startSlice - endSlice <= 0)
            {
                return new(false,
                    $"The provided string '{@string}' doesnt contain enough characters to slice it with values 'startSlice {startSlice}' and endSlice '{endSlice}'.");
            }

            var toRet = @string.Substring(startSlice, @string.Length - startSlice - endSlice);
            return new(true, variablesToRet: new object[] { toRet });
        }
    }
}