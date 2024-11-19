using System;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.API.Features;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Variable
{
    public class EvalAction : IScriptAction, IHelpInfo, IReturnValueAction
    {
        /// <inheritdoc/>
        public string Name => "Eval";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Variable;

        /// <inheritdoc/>
        public string Description => "Returns an evaluated value that was provided (does math and conditions if applicable).";

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
                return new ActionResponse(true, new(response.Passed.ToUpper()));
            }

            return ConditionHelper.TryMath(input, out var res)
                ? new ActionResponse(true, new(res.Result.ToUpper()))
                : new(true, new(Parser.ReplaceContaminatedValueSyntax(input, script)));
        }
    }
}
