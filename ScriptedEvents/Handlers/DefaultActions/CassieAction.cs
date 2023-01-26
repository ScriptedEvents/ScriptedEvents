using System;
using Exiled.API.Features;
using ScriptedEvents.API.Features.Actions;

namespace ScriptedEvents.Handlers.DefaultActions
{
    public class CassieAction : IAction
    {
        public string Name => "CASSIE";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
        {
            if (Arguments.Length < 1) return new(false, "Missing text!");

            string text = string.Join(" ", Arguments);
            string[] cassieArgs = text.Split('|');
            if (cassieArgs.Length == 1)
                Cassie.MessageTranslated(text, text);
            else
                Cassie.MessageTranslated(cassieArgs[0], cassieArgs[1]);
            return new(true, string.Empty);
        }
    }
}
