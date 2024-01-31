namespace ScriptedEvents.Structures
{
    using ScriptedEvents.Variables.Interfaces;

    public class VariableResult
    {
        public VariableResult(bool success, IConditionVariable variable, string message = "", bool reversed = false)
        {
            Success = success;
            Reversed = reversed;
            Message = message;
            Variable = variable;
        }

        public bool Success { get; }

        public bool Reversed { get; }

        public string Message { get; }

        public IConditionVariable Variable { get; }
    }
}
