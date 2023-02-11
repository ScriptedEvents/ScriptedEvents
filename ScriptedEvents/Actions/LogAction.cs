using Exiled.API.Features;
using ScriptedEvents.Actions.Interfaces;
using ScriptedEvents.Variables;
using System;
using System.Linq;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions
{
    public class LogAction : IScriptAction, IHelpInfo
    {
        public string Name => "LOG";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Creates a console log. Variables are supported.";

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