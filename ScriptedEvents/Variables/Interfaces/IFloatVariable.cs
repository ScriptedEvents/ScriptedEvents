namespace ScriptedEvents.Variables.Interfaces
{
    /// <summary>
    /// Represents a conditional variable that has a float result.
    /// </summary>
    public interface IFloatVariable : IConditionVariable
    {
        /// <summary>
        /// Gets the float value of this variable.
        /// </summary>
        public float Value { get; }
    }
}
