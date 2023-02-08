using Exiled.API.Features;
using ScriptedEvents.API.Features.Actions;
using System;
using System.Linq;

namespace ScriptedEvents.Actions
{
    public class RoundlockAction : IScriptAction
    {
        public string Name => "ROUNDLOCK";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1) return new(false, "Missing argument: true/false");

            Round.IsLocked = Arguments.ElementAt(0).ToLower() is "true" or "t" or "yes" or "y";
            return new(true);
        }
    }
}