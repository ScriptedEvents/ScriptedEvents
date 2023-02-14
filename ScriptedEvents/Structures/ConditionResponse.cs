namespace ScriptedEvents.Structures
{
    /// <summary>
    /// Represents the result of a call to <see cref="API.Helpers.ConditionHelper.Evaluate(string)"/>.
    /// </summary>
    public class ConditionResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionResponse"/> class.
        /// </summary>
        /// <param name="success">Whether or not the condition was executed successfully.</param>
        /// <param name="passed">Whether or not the condition returned TRUE.</param>
        /// <param name="message">An error message, if <paramref name="success"/> is <see langword="false"/>.</param>
        /// <param name="objectResult">An object representing the outcome of the condition (such as a float). May be null.</param>
        public ConditionResponse(bool success, bool passed, string message, object objectResult = null)
        {
            Success = success;
            Passed = passed;
            Message = message;
            ObjectResult = objectResult;
        }

        /// <summary>
        /// Gets a value indicating whether or not the condition was executed successfully.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Gets a value indicating whether or not the condition returned TRUE.
        /// </summary>
        public bool Passed { get; }

        /// <summary>
        /// Gets an error message, if <see cref="Success"/> is <see langword="false"/>.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets an object representing the outcome of the condition (such as a float). May be null.
        /// </summary>
        public object ObjectResult { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"SUCCESS: {Success} | PASSED: {Passed} | MESSAGE: {(string.IsNullOrWhiteSpace(Message) ? "N/A" : Message)}";
        }
    }
}
