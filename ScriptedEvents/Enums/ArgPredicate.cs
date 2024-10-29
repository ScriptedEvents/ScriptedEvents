namespace ScriptedEvents.Enums
{
    /// <summary>
    /// An additional setting for the arg which will change its behavior.
    /// </summary>
    public enum ArgPredicate
    {
        /// <summary>
        /// No predicate.
        /// </summary>
        None,

        /// <summary>
        /// Value must be bigger than 0.
        /// </summary>
        BiggerThan0,

        /// <summary>
        /// With this setting, if the original type fails to parse, the input will be parsed into a string.
        /// </summary>
        ParseToStringOnFail,
    }
}