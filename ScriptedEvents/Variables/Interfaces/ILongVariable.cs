namespace ScriptedEvents.Variables.Interfaces
{
    /// <summary>
    /// Represents a conditional variable that has a float result.
    /// </summary>
    public interface ILongVariable : IConditionVariable
    {
        /// <summary>
        /// Gets the long value of this variable.
        /// </summary>
        public long Value { get; }
    }
}
