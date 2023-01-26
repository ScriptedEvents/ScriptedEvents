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
            string subtitle = text;

            var split = text.Split(new char[] { '|' }, 2);
            if (split.Length > 1)
                subtitle = split[1];

            Cassie.MessageTranslated(text, subtitle);
            return new(true, string.Empty);
        }
    }
}
