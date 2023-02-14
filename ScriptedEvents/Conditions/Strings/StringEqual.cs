namespace ScriptedEvents.Conditions.Strings
{
    using ScriptedEvents.Conditions.Interfaces;

    /// <summary>
    /// Returns whether or not two strings are equal.
    /// </summary>
    public class StringEqual : IStringCondition
    {
        /// <inheritdoc/>
        public string Symbol => "=";

        /// <inheritdoc/>
        public bool Execute(string left, string right)
        {
            return left.Equals(right);
        }
    }
}
