namespace ScriptedEvents.Variables.Interfaces
{
    /// <summary>
    /// Represents a conditional variable that has a boolean result.
    /// </summary>
    public interface IBoolVariable : IConditionVariable
    {
        /// <summary>
        /// Gets the variable name that will flip the <see cref="Value"/> if used.
        /// </summary>
        public string ReversedName { get; }

        /// <summary>
        /// Gets a value indicating the boolean value of this variable.
        /// </summary>
#pragma warning disable SA1623 // Property summary documentation should match accessors
        public bool Value { get; }
#pragma warning restore SA1623 // Property summary documentation should match accessors
    }
}
