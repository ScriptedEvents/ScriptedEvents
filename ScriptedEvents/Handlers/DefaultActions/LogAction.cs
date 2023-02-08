using Exiled.API.Features;
using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.Handlers.Variables;
using System;
using System.Linq;

namespace ScriptedEvents.Actions
{
    public class LogAction : IScriptAction
    {
        public string Name => "LOG";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Creates a console log.";

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("message", typeof(string), "The message.", true),
        };

        public ActionResponse Execute(Script script)
        {
            Log.Info(string.Join(" ", Arguments.Select(r => ConditionVariables.ReplaceVariables(r))));
            return new(true);
        }
    }
}