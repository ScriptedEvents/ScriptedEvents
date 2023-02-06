using Exiled.API.Features;
using ScriptedEvents.API.Features.Actions;
using System;

namespace ScriptedEvents.Handlers.DefaultActions
{
    public class WarheadAction : IAction
    {
        public string Name => "WARHEAD";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

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
