using Exiled.API.Features;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptedEvents.API.Features.Actions;

namespace ScriptedEvents.Handlers.DefaultActions
{
    public class WaitUntilAction : ITimingAction
    {
        public string Name => "WAITUNTIL";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public Dictionary<string, Func<bool>> ValidInputs = new()
        {
            { "ROUNDSTART", () => Round.IsStarted },
            { "ROUNDEND", () => Round.IsEnded },
        };

        public ActionResponse Execute()
        {
            return new(true);
        }

        public float GetDelay(out ActionResponse message)
        {
            if (Arguments.Length < 1)
            {
                message = new(false, "Missing argument: type");
                return -1;
            }

            if (!ValidInputs.TryGetValue(Arguments.ElementAt(0).Trim(), out Func<bool> executor))
            {
                message = new(false, "Invalid type provided.", true);
                return -1;
            }

            message = new(true);
            return Timing.WaitUntilTrue(executor);
        }
    }
}
