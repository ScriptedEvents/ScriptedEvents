namespace ScriptedEvents.Conditions.Interfaces
{
    /// <summary>
    /// Represents a condition comparing two floats.
    /// </summary>
    public interface IFloatCondition : ICondition
    {
        /// <summary>
        /// Compares two floats and returns whether or not the condition passed.
        /// </summary>
        /// <param name="left">The lefthand side.</param>
        /// <param name="right">The righthand side.</param>
        /// <returns>Whether or not the condition passed.</returns>
        public bool Execute(float left, float right);
    }
}
