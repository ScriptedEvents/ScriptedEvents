namespace ScriptedEvents.API.Enums
{
#pragma warning disable SA1602
    public enum ErrorCode
    {
        AutoRun_Disabled = 100,
        AutoRun_NotFound,

        InvalidAction,

        MultipleFlagDefs,
        MultipleLabelDefs,

        AutoRun_AdminEvent,

        IOPermissionError,
        IOError,

        On_UnknownEvent,
        On_IncompatibleEvent,
        On_DisabledScript,
        On_NotFoundScript,
        On_UnknownError,

        LEGACY_SafetyError,

        IOHelpPermissionError,
        IOHelpError,

        InvalidActionUsage,
        LEGACY_InvalidActionUsage,

        ParameterError_Option,
        ParameterError_Number,
        ParameterError_Condition,
        ParameterError_LessThanZeroNumber,
        ParameterError_RoleType,
        ParameterError_Players,
        ParameterError_Rooms,
        ParameterError_CassieNoAnnc,

        UnknownError,

        IOMissing,

        CustomCommand_NoName,
        CustomCommand_NoScripts,

        MissingArguments,

        LEGACY_InvalidPlayerVariable,
        InvalidVariable,
        InvalidPlayerVariable,
        InvalidInteger,

        IndexTooLarge,

        CustomCommand_MultCooldowns,

        InvalidNumber,

        UnsupportedArgumentVariables,

        ScriptJumpFailed,

        VariableReplaceError,

        UnknownActionError,

        InvalidDoor,
        InvalidLift,
        InvalidEnumGeneric,
        InvalidStringVariable,
        InvalidCharacter,
        InvalidRoleTypeOrTeam,
        InvalidBoolean,
    }
#pragma warning restore SA1602
}
