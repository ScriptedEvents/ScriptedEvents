using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.Actions
{
    public class RoundlockAction : IAction
    {
        public string Name => "ROUNDLOCK";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
        {
            if (Arguments.Length < 1) return new(false, "Missing argument: true/false");

            Round.IsLocked = Arguments.ElementAt(0).ToLower() is "true" or "t" or "yes" or "y";
            return new(true);
        }
    }
}
