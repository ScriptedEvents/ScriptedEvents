namespace ScriptedEvents.Conditions.Interfaces
{
    /// <summary>
    /// Represents a condition.
    /// </summary>
    public interface ICondition
    {
        /// <summary>
        /// Gets the symbol to detect.
        /// </summary>
        public string Symbol { get; }
    }
}
