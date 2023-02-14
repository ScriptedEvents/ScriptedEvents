namespace ScriptedEvents.Conditions.Floats
{
    using ScriptedEvents.Conditions.Interfaces;

    /// <summary>
    /// Returns whether or not the lefthand is less than or greater to the righthand.
    /// </summary>
    public class LessThanOrEqualTo : IFloatCondition
    {
        /// <inheritdoc/>
        public string Symbol => "<=";

        /// <inheritdoc/>
        public bool Execute(float left, float right) => left <= right;
    }
}
