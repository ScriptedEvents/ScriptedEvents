using ScriptedEvents.API.Features;

namespace ScriptedEvents.Structures
{
    using System;

    /// <summary>
    /// Represents the result of a call to <see cref="ConditionHelper.TryMath(string, out MathResult)"/>.
    /// </summary>
    public class MathResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether or not the math was executed successfully.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the result of the math equation.
        /// </summary>
        public float Result { get; set; }

        /// <summary>
        /// Gets or sets the exception, if <see cref="Success"/> is <see langword="false"/>.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets the <see cref="Exception.Message"/> of the exception, if <see cref="Exception"/> is not <see langword="null"/>.
        /// </summary>
        public string Message => Exception.Message;
    }
}
