namespace ScriptedEvents.API.Enums
{
    /// <summary>
    /// Type of message for the <see cref="Helpers.MsgGen"/> to generate.
    /// </summary>
    public enum MessageType
    {
        OK,
        GeneratorError,
        InvalidUsage,
        InvalidOption,
        NotANumber,
        NotANumberOrCondition,
        LessThanZeroNumber,
        NoPlayersFound,
        CassieCaptionNoAnnouncement,
        Custom,
    }
}
