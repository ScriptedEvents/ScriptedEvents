using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.API.Helpers;
using System;
using System.Linq;

namespace ScriptedEvents.Actions
{
    public class GotoIfAction : IScriptAction, IHelpInfo
    {
        public string Name => "GOTOIF";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Reads the condition and jumps to the first provided line if the condition is TRUE, or the second provided line if the condition is FALSE.";
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("trueLine", typeof(int), "The line to jump to if the condition is TRUE. Variables & Math are NOT supported.", true),
            new Argument("falseLine", typeof(int), "The line to jump to if the condition iS FALSE. Variables & Math are NOT supported.", true),
            new Argument("condition", typeof(string), "The condition to check. Variables & Math are supported.", true),
        };

        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 3) return new(false, "Missing arguments: trueLine, falseLine, condition");

            if (!int.TryParse(Arguments[0], out int trueLine))
                return new(false, "trueLine is not a valid integer.");

            if (!int.TryParse(Arguments[1], out int falseLine))
                return new(false, "falseLine is not a valid integer.");

            ConditionResponse outcome = ConditionHelper.Evaluate(string.Join("", Arguments.Skip(2)));
            if (!outcome.Success)
                return new(false, $"IF execution error: {outcome.Message}", ActionFlags.FatalError);

            if (outcome.Passed)
            {
                script.Jump(trueLine);
            }
            else
            {
                script.Jump(falseLine);
            }

            return new(true);
        }
    }
}