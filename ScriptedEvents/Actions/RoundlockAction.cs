using Exiled.API.Features;
using ScriptedEvents.API.Features.Actions;
using System;
using System.Linq;

namespace ScriptedEvents.Actions
{
    public class RoundlockAction : IScriptAction, IHelpInfo
    {
        public string Name => "ROUNDLOCK";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Set server's roundlock.";

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("roundlock", typeof(bool), "Whether or not to lock the round.", true)
        };

        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1) return new(false, "Missing argument: TRUE/FALSE");

            Round.IsLocked = Arguments.ElementAt(0).ToUpper() is "TRUE" or "YES";
            return new(true);
        }
    }
}