namespace ScriptedEvents.Conditions.Floats
{
    using ScriptedEvents.Conditions.Interfaces;

    /// <summary>
    /// Returns whether or not two floats are equal.
    /// </summary>
    public class Equal : IFloatCondition
    {
        /// <inheritdoc/>
        public string Symbol => "=";

        /// <inheritdoc/>
        public bool Execute(float left, float right) => left == right;
    }
}
