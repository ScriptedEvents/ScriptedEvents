namespace ScriptedEvents.Variables.Interfaces
{
    /// <summary>
    /// Represents a conditional variable that has a boolean result.
    /// </summary>
    public interface IItemVariable : IConditionVariable
    {
        /// <summary>
        /// Gets XXXX.
        /// </summary>
        public ushort Value { get; }
    }
}
