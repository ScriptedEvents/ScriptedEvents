namespace ScriptedEvents.Conditions.Strings
{
    using ScriptedEvents.Conditions.Interfaces;

    /// <summary>
    /// Returns whether or not the right string contains the left string.
    /// </summary>
    public class StringNotContains : IStringCondition
    {
        /// <inheritdoc/>
        public string Symbol => "!IN";

        /// <inheritdoc/>
        public bool Execute(string left, string right)
        {
            return !right.Contains(left);
        }
    }
}
