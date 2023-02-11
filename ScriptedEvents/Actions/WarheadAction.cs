using Exiled.API.Features;
using ScriptedEvents.Actions.Interfaces;
using System;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions
{
    public class WarheadAction : IAction, IHelpInfo
    {
        public string Name => "WARHEAD";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Forces a specific warhead action.";

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("action", typeof(string), "The action to run. Valid options: START, STOP, LOCK, UNLOCK, DETONATE, BLASTDOORS", true),
        };

        public ActionResponse Execute()
        {
            if (Arguments.Length < 1)
                return new(false, "Missing argument: action (START/STOP/LOCK/UNLOCK/DETONATE/BLASTDOORS)");

            switch (Arguments[0].ToUpper())
            {
                case "START":
                    Warhead.Start();
                    break;
                case "STOP":
                    Warhead.Stop();
                    break;
                case "DETONATE":
                    Warhead.Detonate();
                    break;
                case "LOCK":
                    Warhead.IsLocked = true;
                    break;
                case "UNLOCK":
                    Warhead.IsLocked = false;
                    break;
                case "BLASTDOORS":
                    Warhead.CloseBlastDoors();
                    break;
                default:
                    return new(false, "First argument must be START/STOP/DETONATE/LOCK/UNLOCK/BLASTDOORS!");
            }

            return new(true);
        }
    }
}
