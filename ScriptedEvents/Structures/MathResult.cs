namespace ScriptedEvents.Structures
{
    using System;

    public class MathResult
    {
        public bool Success { get; set; }

        public float Result { get; set; }

        public Exception Exception { get; set; }

        public string Message => Exception.Message;
    }
}
