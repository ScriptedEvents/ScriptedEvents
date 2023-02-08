using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.API.Helpers;
using System;

namespace ScriptedEvents.Actions
{
    public class StopIfAction : IScriptAction, IHelpInfo
    {
        public string Name => "STOPIF";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Reads the condition and stops execution of the script if the result is TRUE.";

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("condition", typeof(string), "The condition to check.", true),
        };

        public ActionResponse Execute(Script scr)
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