using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.Actions
{
    public class CassieAction : IAction
    {
        public string Name => "CASSIE";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
        {
            if (Arguments.Length < 1) return new(false, "Missing CASSIE text!");

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
