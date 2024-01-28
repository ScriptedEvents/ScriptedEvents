using System.Collections.Generic;

namespace ScriptedEvents.Structures
{
    public class ArgumentProcessResult
    {
        public ArgumentProcessResult(bool success, string argument = "", string message = "")
        {
            Success = success;
            FailedArgument = argument;
            Message = message;

            NewParameters = new();
        }

        public bool Success { get; }

        public string FailedArgument { get; }

        public string Message { get; }

        public List<object> NewParameters { get; }
    }
}
