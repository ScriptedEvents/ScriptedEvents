namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class Math_RootAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "MATH-ROOT";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Math;

        /// <inheritdoc/>
        public string Description => "Returns a root of the input number.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new Argument("value", typeof(float), "The number to root.", true),
             new Argument("rootLevel", typeof(float), "The root level. Default: 2", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            return new(true, variablesToRet: new[] { (object)(float)Math.Pow((float)Arguments[0], 1 / (Arguments.Length > 1 ? (float)Arguments[1] : 2)) });
        }
    }
}