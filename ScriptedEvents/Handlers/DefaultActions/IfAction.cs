using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.API.Helpers;
using System;

namespace ScriptedEvents.Actions
{
    public class IfAction : IAction
    {
        public string Name => "IF";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
        {
            ConditionResponse outcome = ConditionHelper.Evaluate(string.Join("", Arguments));
            if (!outcome.Success)
                return new(false, $"IF execution error: {outcome.Message}", ActionFlags.FatalError);

            if (!outcome.Passed)
                return new(true, flags: ActionFlags.StopEventExecution);

            return new(true);
        }
    }
}