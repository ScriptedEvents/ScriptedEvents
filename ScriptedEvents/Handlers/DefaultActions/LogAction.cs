using Exiled.API.Features;
using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.Handlers.Variables;
using System;
using System.Linq;

namespace ScriptedEvents.Actions
{
    public class LogAction : IAction
    {
        public string Name => "LOG";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
        {
            Log.Info(string.Join(" ", Arguments.Select(r => ConditionVariables.ReplaceVariables(r))));
            return new(true);
        }
    }
}