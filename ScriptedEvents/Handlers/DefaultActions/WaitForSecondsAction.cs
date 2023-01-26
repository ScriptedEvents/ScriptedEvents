using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptedEvents.API.Helpers;
using ScriptedEvents.API.Features.Actions;

namespace ScriptedEvents.Handlers.DefaultActions
{
    public class WaitForSecondsAction : ITimingAction
    {
        public string Name => "WAITSEC";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
        {
            return new(true);
        }

        public float GetDelay(out ActionResponse message)
        {
            if (Arguments.Length < 1)
            {
                message = new(false, "Missing argument: duration");
                return -1;
            }
            if (!ScriptHelper.TryConvertNumber(Arguments[0], out int duration))
            {
                message = new(false, "First argument must be an int or range of ints!");
                return -1;
            }

            message = new(true);
            return Timing.WaitForSeconds(duration);
        }
    }
}
