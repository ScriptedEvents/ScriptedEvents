using System;

namespace ScriptedEvents.Actions.Interfaces
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
