namespace ScriptedEvents.Actions.Interfaces
{
    using ScriptedEvents.API.Enums;

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
