namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class Math_PowerAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "MATH-POWER";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Math;

        /// <inheritdoc/>
        public string Description => "Returns a exponentiated version of the input number.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new Argument("value", typeof(float), "Number to exponentiate.", true),
             new Argument("exponent", typeof(float), "The exponentiating number.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            return new(true, variablesToRet: new[] { (object)(float)Math.Pow((float)Arguments[0], (float)Arguments[1]) });
        }
    }
}