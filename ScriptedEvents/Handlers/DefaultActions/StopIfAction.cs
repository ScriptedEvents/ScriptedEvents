using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.Actions
{
    public class StopIfAction : IAction
    {
        public string Name => "STOPIF";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
        {
            ConditionResponse outcome = ConditionHelper.Evaluate(string.Join("", Arguments));
            if (!outcome.Success)
                return new(false, $"STOPIF execution error: {outcome.Message}", ActionFlags.FatalError);

            if (outcome.Passed)
                return new(true, flags: ActionFlags.StopEventExecution);

            return new(true);
        }
    }
}