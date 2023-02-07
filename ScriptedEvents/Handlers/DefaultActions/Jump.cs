using ScriptedEvents.API.Features;
using ScriptedEvents.API.Features.Actions;
using System;
using System.Linq;

namespace ScriptedEvents.Handlers.DefaultActions
{
    public class JumpAction : IScriptAction
    {
        public string Name => "JUMP";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute(Script script)
        {
            if (int.TryParse(Arguments.ElementAtOrDefault(0), out int newline))
            {
                script.CurrentLine = newline;
                return new(true);
            }
            return new(false);
        }
    }
}
