namespace ScriptedEvents.Conditions.Interfaces
{
    /// <summary>
    /// Represents a condition comparing two strings.
    /// </summary>
    public interface IStringCondition : ICondition
    {
        /// <summary>
        /// Compares two strings and returns whether or not the condition passed.
        /// </summary>
        /// <param name="left">The lefthand side.</param>
        /// <param name="right">The righthand side.</param>
        /// <returns>Whether or not the condition passed.</returns>
        public bool Execute(string left, string right);
    }
}
