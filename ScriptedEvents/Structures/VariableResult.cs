namespace ScriptedEvents.Structures
{
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Variables.Interfaces;

    /// <summary>
    /// Represents the result of attempting to retrieve a variable.
    /// </summary>
    public class VariableResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VariableResult"/> class.
        /// </summary>
        /// <param name="success">Whether or not the result was successful.</param>
        /// <param name="variable">The newly retrieved variable.</param>
        /// <param name="message">The error message, if <paramref name="success"/> is <see langword="false"/>.</param>
        /// <param name="reversed">Whether or not the boolean variable was reversed.</param>
        public VariableResult(bool success, IConditionVariable variable, string message = "", bool reversed = false)
        {
            Success = success;
            Reversed = reversed;
            Message = message;
            Variable = variable;
        }

        /// <summary>
        /// Gets a value indicating whether or not the variable retrieval was successful.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Gets a value indicating whether or not the boolean variable is reversed.
        /// </summary>
        public bool Reversed { get; }

        /// <summary>
        /// Gets the error message, if <see cref="Success"/> is <see langword="false"/>.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the newly retrieved variable.
        /// </summary>
        public IConditionVariable Variable { get; }

        public string String(Script source = null, bool reversed = false) => Variable.String(source, reversed);
    }
}
