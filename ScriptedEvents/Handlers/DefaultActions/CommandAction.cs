using System;
using ScriptedEvents.API.Features.Actions;

namespace ScriptedEvents.Handlers.DefaultActions
{
    public class CommandAction : IScriptAction
    {
        public string Name => "COMMAND";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1) return new(false, "Missing Command!");

            string text = string.Join(" ", Arguments);
            GameCore.Console.singleton.TypeCommand(text);
            return new(true, string.Empty);
        }
    }
}
