namespace ScriptedEvents.Conditions.Floats
{
    using ScriptedEvents.Conditions.Interfaces;

    /// <summary>
    /// Returns whether or not the lefthand is greater than or equal to the righthand.
    /// </summary>
    public class GreaterThanOrEqualTo : IFloatCondition
    {
        /// <inheritdoc/>
        public string Symbol => ">=";

        /// <inheritdoc/>
        public bool Execute(float left, float right) => left >= right;
    }
}
