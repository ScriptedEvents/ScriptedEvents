namespace ScriptedEvents.Conditions.Floats
{
    using ScriptedEvents.Conditions.Interfaces;

    /// <summary>
    /// Returns whether or not the lefthand is less than the righthand.
    /// </summary>
    public class LessThan : IFloatCondition
    {
        /// <inheritdoc/>
        public string Symbol => "<";

        /// <inheritdoc/>
        public bool Execute(float left, float right) => left < right;
    }
}
