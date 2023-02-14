namespace ScriptedEvents.API.Enums
{
    using System;

    [Flags]
    public enum ActionFlags
    {
        None = 0,
        FatalError = 1,
        StopEventExecution,
    }
}
