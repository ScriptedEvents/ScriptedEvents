namespace ScriptedEvents.Structures
{
    public class ConditionResponse
    {
        public bool Success { get; set; }
        public bool Passed { get; set; }
        public string Message { get; set; }
        public object ObjectResult { get; set; }

        public ConditionResponse(bool success, bool passed, string message, object objectResult = null)
        {
            Success = success;
            Passed = passed;
            Message = message;
            ObjectResult = objectResult;
        }

        public override string ToString()
        {
            return $"SUCCESS: {Success} | PASSED: {Passed} | MESSAGE: {(string.IsNullOrWhiteSpace(Message) ? "N/A" : Message)}";
        }
    }
}
