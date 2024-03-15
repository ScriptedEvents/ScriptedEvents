namespace ScriptedEvents.Conditions.Strings
{
    using Exiled.API.Features;
    using ScriptedEvents.Conditions.Interfaces;

    /// <summary>
    /// Returns whether or not the right string contains the left string.
    /// </summary>
    public class StringContains : IStringCondition
    {
        /// <inheritdoc/>
        public string Symbol => "IN";

        /// <inheritdoc/>
        public bool Execute(string left, string right)
        {
            Log.Debug($"left '{left}' - right '{right}'");
            Log.Debug($"IN returns {right.Contains(left)}");
            return right.Contains(left);
        }
    }
}
