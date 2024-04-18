namespace ScriptedEvents.API.Enums
{
    /// <summary>
    /// Type of message for the <see cref="Helpers.MsgGen"/> to generate.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// Represents a successful action.
        /// </summary>
        OK,

        /// <summary>
        /// Represents a message generator error. Currently unused.
        /// </summary>
        GeneratorError,

        /// <summary>
        /// Invalid command usage.
        /// </summary>
        InvalidUsage,

        /// <summary>
        /// Provided input was not a number.
        /// </summary>
        NotANumber,

        /// <summary>
        /// Provided input was not a number or a valid condition to compute a number.
        /// </summary>
        NotANumberOrCondition,

        /// <summary>
        /// Provided input was a number, but it was negative.
        /// </summary>
        LessThanZeroNumber,

        /// <summary>
        /// No players found with the provided input.
        /// </summary>
        NoPlayersFound,

        /// <summary>
        /// No rooms found with the provided input.
        /// </summary>
        NoRoomsFound,

        /// <summary>
        /// No role found with the provided input.
        /// </summary>
        InvalidRole,

        /// <summary>
        /// Provided input provided a CASSIE caption, but no corresponding announcement.
        /// </summary>
        CassieCaptionNoAnnouncement,

        /// <summary>
        /// Essentially none of the above.
        /// </summary>
        Custom,
    }
}
