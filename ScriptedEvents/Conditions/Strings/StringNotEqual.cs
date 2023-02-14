namespace ScriptedEvents.Conditions.Strings
{
    using ScriptedEvents.Conditions.Interfaces;

    /// <summary>
    /// Returns whether or not two strings are NOT equal.
    /// </summary>
    public class StringNotEqual : IStringCondition
    {
        /// <inheritdoc/>
        public string Symbol => "!=";

        /// <inheritdoc/>
        public bool Execute(string left, string right)
        {
            return !left.Equals(right);
        }
    }
}
