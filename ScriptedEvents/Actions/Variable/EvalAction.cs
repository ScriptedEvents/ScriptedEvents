using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions.VariableActions
{
    using System;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
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
            var input = Arguments.JoinMessage();
            input = Parser.ReplaceContaminatedValueSyntax(input, script);

            var response = ConditionHelper.Evaluate(input, script);
            if (response.Success)
            {
                return new ActionResponse(true, variablesToRet: new object[] { response.Passed.ToUpper() });
            }

            return ConditionHelper.TryMath(input, out var res)
                ? new ActionResponse(true, variablesToRet: new object[] { res.Result.ToUpper() })
                : new(true, variablesToRet: new object[] { Parser.ReplaceContaminatedValueSyntax(input, script) });
        }
    }
}
