using ScriptedEvents.API.Features;
using ScriptedEvents.API.Features.Actions;
using System;
using System.Linq;

namespace ScriptedEvents.Handlers.DefaultActions
{
    public class GotoAction : IScriptAction
    {
        public string Name => "GOTO";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute(Script script)
        {
            switch (Arguments.ElementAtOrDefault(0).ToUpper())
            {
                case "ADD":
                    if (int.TryParse(Arguments.ElementAtOrDefault(1), out int newline))
                    {
                        script.CurrentLine += newline;
                        return new(true);
                    }

                    break;
                case "NUMBER":
                    if (int.TryParse(Arguments.ElementAtOrDefault(1), out newline))
                    {
                        script.CurrentLine = newline;
                        return new(true);
                    }
                    break;
            }
            return new(false);
        }
    }
}
