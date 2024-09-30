namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class EvalAction : IScriptAction, IHelpInfo, IReturnValueAction
    {
        /// <inheritdoc/>
        public string Name => "EVAL";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Variable;

        /// <inheritdoc/>
        public string Description => "Returns an evaluated value that was provided (does math and boolean algebra if applicable).";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("value", typeof(object), "The value to store.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string input = Arguments.JoinMessage();
            input = SEParser.ReplaceContaminatedValueSyntax(input, script);

            if (ConditionHelperV2.TryMath(input, out var condition))
            {
                return new(true, variablesToRet: new[] { condition.Result.ToUpper() });
            }

            return new(true, variablesToRet: new[] { input });
        }
    }
}
