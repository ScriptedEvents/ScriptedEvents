namespace ScriptedEvents.Enums
{
    /// <summary>
    /// An additional setting for the arg which will change its behavior.
    /// </summary>
    public enum ArgFlag
    {
        /// <summary>
        /// No flag.
        /// </summary>
        None,

        /// <summary>
        /// Value must be bigger than 0.
        /// </summary>
        BiggerThan0,

        /// <summary>
        /// Value must be bigger or equal to 0.
        /// </summary>
        BiggerOrEqual0,

        /// <summary>
        /// With this setting, if the original type fails to parse, the input will be parsed into a string.
        /// </summary>
        ParseToStringOnFail,
    }
}