namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class DebugMathAction : IScriptAction, IHiddenAction
    {
        /// <inheritdoc/>
        public string Name => "DEBUGMATH";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Debug;

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("math", typeof(string), "The math to debug", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string formula = SEParser.ReplaceContaminatedValueSyntax(RawArguments.JoinMessage(), script);
            if (!ConditionHelperV2.TryMath(formula, out MathResult result))
            {
                return new(MessageType.NotANumberOrCondition, this, "condition", null, formula, result);
            }

            return new(true, result.Result.ToString());
        }
    }
}