namespace ScriptedEvents.Conditions.Floats
{
    using ScriptedEvents.Conditions.Interfaces;

    /// <summary>
    /// Returns whether or not two floats are NOT equal.
    /// </summary>
    public class NotEqual : IFloatCondition
    {
        /// <inheritdoc/>
        public string Symbol => "!=";

        /// <inheritdoc/>
        public bool Execute(float left, float right) => left != right;
    }
}
