namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class StrLowerAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "STR-LOWER";

        /// <inheritdoc/>
        public string Description => "Returns the provided string where all UPPERCASE letters are replaced with lowercase ones.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.MapInfo;

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("string", typeof(string), "The string to lowercase.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            return new(true, variablesToRet: new[] { Arguments[0].ToString().ToLower() });
        }
    }
}