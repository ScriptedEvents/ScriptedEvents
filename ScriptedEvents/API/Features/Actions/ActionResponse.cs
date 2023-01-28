using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptedEvents;

namespace ScriptedEvents.API.Features.Actions
{
    [Flags]
    public enum ActionFlags
    {
        None = 0,
        FatalError = 1,
        StopEventExecution,
    }

    public class ActionResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ActionFlags ResponseFlags { get; set; }

        public ActionResponse(bool success, string message = "", ActionFlags flags = ActionFlags.None)
        {
            Success = success;
            Message = message;
            ResponseFlags = flags;
        }
    }
}
