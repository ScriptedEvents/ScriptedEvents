namespace ScriptedEvents.Conditions.Floats
{
    using ScriptedEvents.Conditions.Interfaces;

    /// <summary>
    /// Returns whether or not the lefthand is greater than the righthand.
    /// </summary>
    public class GreaterThan : IFloatCondition
    {
        /// <inheritdoc/>
        public string Symbol => ">";

        /// <inheritdoc/>
        public bool Execute(float left, float right) => left > right;
    }
}
