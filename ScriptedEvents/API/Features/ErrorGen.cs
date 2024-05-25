namespace ScriptedEvents.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Exiled.API.Enums;

    using PlayerRoles;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;

    /// <summary>
    /// Exposes API to generate consistent error messages throughout the entire plugin.
    /// </summary>
    public static class ErrorGen
    {
        /// <summary>
        /// Get a <see cref="ErrorInfo"/> from the given ID.
        /// </summary>
        /// <param name="errorCode">Error ID.</param>
        /// <returns>An <see cref="ErrorInfo"/> object. Its <see cref="ErrorInfo.Id"/> will be <c>0</c> if the operation was unsuccessful.</returns>
        public static ErrorInfo GetError(int errorCode) =>
            ErrorList.Errors.FirstOrDefault(err => err.Id == errorCode);

        /// <summary>
        /// Gets a <see cref="ErrorInfo"/> from the given <see cref="ErrorCode"/>.
        /// </summary>
        /// <param name="code">Error code.</param>
        /// <returns>An <see cref="ErrorInfo"/> object. Its <see cref="ErrorInfo.Id"/> will be <c>0</c> if the operation was unsuccessful.</returns>
        public static ErrorInfo GetError(ErrorCode code) =>
            ErrorList.Errors.FirstOrDefault(err => err.Code == code);

        /// <summary>
        /// Try-get a <see cref="ErrorInfo"/> from the given error ID.
        /// </summary>
        /// <param name="errorID">Error ID.</param>
        /// <param name="errorInfo">The <see cref="ErrorInfo"/> object.</param>
        /// <returns>Whether or not the process was successful.</returns>
        public static bool TryGetError(int errorID, out ErrorInfo errorInfo)
        {
            errorInfo = GetError(errorID);
            return errorInfo.Id != 0;
        }

        /// <summary>
        /// Try-get a <see cref="ErrorInfo"/> from the given code.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        /// <param name="errorInfo">The <see cref="ErrorInfo"/> object.</param>
        /// <returns>Whether or not the process was successful.</returns>
        public static bool TryGetError(ErrorCode errorCode, out ErrorInfo errorInfo)
        {
            errorInfo = GetError(errorCode);
            return errorInfo.Id != 0;
        }

        /// <summary>
        /// Generates an error string given an error ID.
        /// </summary>
        /// <param name="errorID">Error ID.</param>
        /// <param name="arguments">Arguments for the error.</param>
        /// <returns>An error string.</returns>
        public static string Generate(int errorID, params object[] arguments)
        {
            ErrorInfo err = GetError(errorID);

            if (err.Id == 0)
                err = GetError(126);

            return string.Format(err.ToString(), arguments);
        }

        /// <summary>
        /// Generates an error string given an error code.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        /// <param name="arguments">Arguments for the error.</param>
        /// <returns>An error string.</returns>
        public static string Generate(ErrorCode errorCode, params object[] arguments)
        {
            ErrorInfo err = GetError(errorCode);

            if (err.Id == 0)
                err = GetError(126);

            return string.Format(err.ToString(), arguments);
        }

        /// <summary>
        /// Generates an error string given an error ID.
        /// </summary>
        /// <param name="errorID">Error ID.</param>
        /// <param name="arguments">Arguments for the error.</param>
        /// <returns>An error string.</returns>
        [Obsolete("Use overload with enum.")]
        public static string Get(int errorID, params object[] arguments) => Generate(errorID, arguments);

        /// <summary>
        /// Generates an error string given an error code.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        /// <param name="arguments">Arguments for the error.</param>
        /// <returns>An error string.</returns>
        public static string Get(ErrorCode errorCode, params object[] arguments) => Generate(errorCode, arguments);
    }

    /// <summary>
    /// Contains all error messages.
    /// </summary>
    internal static class ErrorList
    {
        /// <summary>
        /// Gets a list of all possible SE errors.
        /// </summary>
        public static ReadOnlyCollection<ErrorInfo> Errors { get; } = new List<ErrorInfo>()
        {
            new ErrorInfo(
                100,
                ErrorCode.AutoRun_Disabled,
                "The '{0}' script is set to run each round, but the script is disabled!",
                "This error occurs when a disabled script is set to run in the auto_run_scripts Exiled config. This error can be resolved by removing the script from the config option, or by enabling the script by removing its !-- DISABLED flag."),

            new ErrorInfo(
                101,
                ErrorCode.AutoRun_NotFound,
                "The '{0}' script is set to run each round, but the script is not found!",
                "This error occurs when a script is specified to run automatically in the auto_run_scripts Exiled config, but the script cannot be found in the Scripted Events directory. This may either be due to a typo in the config, in the script name, or if the script doesn't exist. This error can be resolved by resolving any typos in the script name, by creating a script that doesn't exist, or by deleting a non-existent script from the config option."),

            new ErrorInfo(
                102,
                ErrorCode.InvalidAction,
                "Invalid action '{0}' detected in script '{1}'.",
                "This error occurs when a script is read, and the script reader finds one or more invalid actions in the script. This error can be resolved by checking for typos in the name of the action."),

            new ErrorInfo(
                103,
                ErrorCode.MultipleFlagDefs,
                "Multiple definitions for the '{0}' flag detected in script {1}.",
                "This error occurs when a script has multiple flag declarations (!-- [FLAG]) for the same flag. This error can be resolved by simply removing multiple declarations of the same flag -- these declarations are only necessary once per script."),

            new ErrorInfo(
                104,
                ErrorCode.MultipleLabelDefs,
                "Multiple definitions for the '{0}' label detected in script {1}.",
                "This error occurs when a script has multiple labels (LABELNAME:) with the same name. This error can be resolved by renaming labels so that each label has a unique name."),

            new ErrorInfo(
                105,
                ErrorCode.AutoRun_AdminEvent,
                "The '{0}' script is set to run each round, but the script is marked as an admin event!",
                "This error occurs when a script is specified to run automatically in the auto_run_scripts Exiled config, but the script is marked as an admin event via the !-- ADMINEVENT flag. Admin event scripts are not meant to be run automatically. This error can be resolved by removing the script from the auto_run_scripts config, or removing the !-- ADMINEVENT flag from the script."),

            new ErrorInfo(
                106,
                ErrorCode.IOPermissionError,
                "Unable to create the required ScriptedEvents directories due to a permission error. Please ensure that ScriptedEvents has proper system permissions to Exiled's Config folder.",
                "This error occurs when Scripted Events is unable to initialize the required directory due to an unauthorized permission error (likely due to the PC or server machine's own antivirus software). This error can be resolved by adjusting the system's settings to allow Scripted Events to make directory changes, or creating the directories manually."),

            new ErrorInfo(
                107,
                ErrorCode.IOError,
                "Unable to load ScriptedEvents due to a directory error.",
                "This error occurs when Scripted Events is unable to initialize the required directory due to any non-permission error. Please report this error in the Scripted Events Discord server."),

            new ErrorInfo(
                108,
                ErrorCode.On_UnknownEvent,
                "The specified event '{0}' in the 'On' config was not found!",
                "This error occurs when an invalid event name is provided in the on Exiled config. This error can be resolved by checking for typos in the name of events and referencing Exiled's list of provided events."),

            new ErrorInfo(
                109,
                ErrorCode.On_IncompatibleEvent,
                "The '{0}' event is not currently compatible with the On config.",
                "This error occurs when an unsupported event is present in the on Exiled config. Unfortunately, there isn't much of a solution here at the moment. Due to how the system works, events must have an event argument in order to be usable with the on config. 99% of Exiled's events DO have an event argument, but very few of them do not. These are the unsupported events.\r\n\r\nMaybe this error will be forever gone in the future!"),

            new ErrorInfo(
                110,
                ErrorCode.On_DisabledScript,
                "Error in 'On' handler (event: {0}): Script '{1}' is disabled!",
                "This error occurs when a disabled script is present in the on Exiled config. This error can be resolved by removing the script from the config option, or by enabling the script by removing its !-- DISABLED flag."),

            new ErrorInfo(
                111,
                ErrorCode.On_NotFoundScript,
                "Error in 'On' handler (event: {0}): Script '{1}' cannot be found!",
                "This error occurs when a script is present in the on Exiled config that is not present in the Scripted Events directory. This may either be due to a typo in the config, in the script name, or if the script doesn't exist. This error can be resolved by resolving any typos in the script name, by creating a script that doesn't exist, or by deleting a non-existent script from the config option."),

            new ErrorInfo(
                112,
                ErrorCode.On_UnknownError,
                "Error in 'On' handler (event: {0})",
                "This error occurs when Scripted Events is unable to handle the 'on' config correctly due to an error. Please report this error in the Scripted Events Discord server."),

            new ErrorInfo(
                113,
                ErrorCode.LEGACY_SafetyError,
                "Script '{0}' exceeded safety limit of {1} actions per 1 second and has been force-stopped, saving from a potential crash. If this is intentional, add '!-- NOSAFETY' to the top of the script. All script loops should have a delay in them.",
                "This error occurs when a script executes more actions in 1 second than what is allowed in the Exiled configs, per the max_actions_per_second configuration. This is likely an indicator that the script is trying to do too much at one time, and could be due to a loop without a yield. This error can be resolved by performing less actions per second, and adding delays (such as WAITSEC) where they wont cause issues. Additionally, the max_actions_per_second config can be raised, allowing more actions to occur per-second. Lastly, as a last-case resort, adding the !-- NOSAFETY flag to the top of a script disables this warning for that individual script."),

            new ErrorInfo(
                114,
                ErrorCode.IOHelpPermissionError,
                "Unable to create the help file, the plugin does not have permission to access the ScriptedEvents directory!",
                "This error occurs when the HELP action is executed, however, ScriptedEvents is unable to create the documentation file due to an unauthorized permission error (likely due to the PC or server machine's own antivirus software). This error can be resolved by adjusting the system's settings to allow Scripted Events to write files, or by using the NOFILE argument in the HELP action."),
            new ErrorInfo(
                115,
                ErrorCode.IOHelpError,
                "Error when writing to file.",
                "This error occurs when the HELP action is executed, however, ScriptedEvents is unable to create the documentation file due to any non-permission error. Please report this error in the Scripted Events Discord server."),
            new ErrorInfo(
                116,
                ErrorCode.InvalidActionUsage,
                "Invalid '{0}' action usage. Usage: {1}",
                "This error occurs when an action is executed without the proper arguments. This error can be resolved by executing the action with the proper arguments, as specified in the error message.\r\n\r\n"),

            new ErrorInfo(
                117,
                ErrorCode.LEGACY_InvalidActionUsage,
                "Invalid '{0}' action usage.",
                "This error occurs when an action is executed without the proper arguments. This error can be resolved by executing the action with the proper arguments. This error is obsolete and has been replaced with SE-116."),

            new ErrorInfo(
                118,
                ErrorCode.ParameterError_Option,
                "Invalid option {0} provided for the '{1}' parameter of the {2} action. This parameter expects one of the following options: {3}.",
                "This error occurs when an action is executed successfully, however one of its parameters received a string value that it was not expecting. This error can be resolved by replacing the specified parameter with one of the specified options."),

            new ErrorInfo(
                119,
                ErrorCode.ParameterError_Number,
                "Invalid number '{0}' provided for the '{1}' parameter of the {2} action.",
                "This error occurs when an action is executed successfully. However, a numerical parameter received a non-numerical value. This error can be resolved by replacing the value of the specified parameter with a numerical value."),

            new ErrorInfo(
                120,
                ErrorCode.ParameterError_Condition,
                "Invalid {0} condition provided in the {1} action! Condition: {2} Error type: '{3}' Message: '{4}'.",
                "This error occurs when an action is executed successfully. However, a numerical parameter received a value that cannot be evaluated as numerical. This error can be resolved by replacing the specified parameter with a valid numerical value, or a valid mathematical formula."),

            new ErrorInfo(
                121,
                ErrorCode.ParameterError_LessThanZeroNumber,
                "Negative number '{0}' cannot be used in the '{1}' parameter of the {2} action.",
                "This error occurs when an action is executed successfully. However, a numerical parameter received a less-than-zero value, which is invalid for the specified action. This error can be resolved by ensuring that the result of the numerical expression does not equal a less-than-zero value.\r\n\r\n"),

            new ErrorInfo(
                122,
                ErrorCode.ParameterError_RoleType,
                "Invalid {0} provided in the {1} action. '{2}' is not a valid RoleType.",
                $"This error occurs when an action is executed successfully. However, a role parameter received an invalid role type. This error can be resolved by ensuring that the value of the parameter matches an internal RoleType value. A full list of valid RoleTypes (as of {DateTime.Now:g}) follows:\n{string.Join("\n", ((RoleTypeId[])Enum.GetValues(typeof(RoleTypeId))).Where(r => r is not RoleTypeId.None).Select(r => $"- [{r:d}] {r}"))}"),

            new ErrorInfo(
                123,
                ErrorCode.ParameterError_Players,
                "No players were found matching the given criteria ('{0}' parameter).",
                "This error occurs when an action is executed successfully. However, no players were found matching the given variables. This error can be resolved by ensuring that there is at least one player match when running a script."),

            new ErrorInfo(
                124,
                ErrorCode.ParameterError_Rooms,
                "No rooms were found matching the given criteria '{0}' ('{1}' parameter).",
                $"This error occurs when an action is executed successfully. However, no rooms were found matching the given names or IDs. This error can be resolved by ensuring that there are no typos in the name or ID of rooms. A full list of valid Room IDs (as of {DateTime.Now:g}) follows:\n{string.Join("\n", ((RoomType[])Enum.GetValues(typeof(RoomType))).Where(r => r is not RoomType.Unknown).Select(r => $"- [{r:d}] {r}"))}"),

            new ErrorInfo(
                125,
                ErrorCode.ParameterError_CassieNoAnnc,
                "Cannot show captions without a corresponding CASSIE announcement.",
                "This error occurs when using the CASSIE and SILENTCASSIE actions, if a CASSIE caption is provided but no message is provided. This error can be resolved by ensuring that the 'message' portion of the text parameter is always provided."),

            new ErrorInfo(
                126,
                ErrorCode.UnknownError,
                "Unknown error",
                "This error can occur if there is an action failure without a valid message. Please report this error in the Scripted Events Discord server."),

            new ErrorInfo(
                127,
                ErrorCode.IOMissing,
                "Critical error: Missing script path. Please reload plugin.",
                "This error occurs when the Scripted Events directory is deleted while the plugin is running. Reloading the plugin usually fixes this issue."),

            new ErrorInfo(
                128,
                ErrorCode.CustomCommand_NoName,
                "Custom command is defined without a name.",
                "This error occurs when there is a command specified in the commands Exiled config, but its name parameter is blank. This error can be resolved by simply providing a name to the command, or by removing the unfinished command structure from the config."),

            new ErrorInfo(
                129,
                ErrorCode.CustomCommand_NoScripts,
                "Custom command '{0}' ({1}) will not be created because it is set to run zero scripts.",
                "This error occurs when there is a command specified in the commands Exiled config, but its run parameter is non-existent or empty. Custom commands must have at least one script set to run in order for the command to be created. As such, this error can be resolved by adding a script to run when the command is executed, or by removing the unfinished command structure from the config."),

            new ErrorInfo(
                130,
                ErrorCode.MissingArguments,
                "The '{0}' {1} requires {2} argument(s) ({3})",
                "This error occurs when an action or variable is used with an insufficient amount of arguments. Most actions require arguments, separated by spaces, such as 'LOG TEST'. Likewise, some variables require arguments, separated by :, such as '{FILTER:PLAYERS:ROLE:ClassD}'. This error can be resolved by supplying the proper amount of arguments."),

            new ErrorInfo(
                131,
                ErrorCode.LEGACY_InvalidPlayerVariable,
                "The provided value '{0}' is not a valid variable or has no associated players.",
                "This error occurs when a variable requires another player variable as an argument, but that other variable is not a valid variable. This error will also occur if the variable is valid, but is not a variable that contains players. This error can be resolved by providing a valid variable that contains players."),

            new ErrorInfo(
                132,
                ErrorCode.InvalidVariable,
                "The provided value '{0}' is not a valid variable.",
                "This error occurs when a variable requires another variable as an argument, but that other variable is not a valid variable. This error can be resolved by providing a valid variable."),

            new ErrorInfo(
                133,
                ErrorCode.InvalidPlayerVariable,
                "The provided variable '{0}' has no associated players.",
                "This error occurs when a variable requires another player variable as an argument, but the other variable is not a variable that contains players. This error can be resolved by providing a valid variable that contains players."),

            new ErrorInfo(
                134,
                ErrorCode.InvalidInteger,
                "The provided value '{0}' is not a valid integer or variable containing an integer.",
                "This error occurs when a variable requires a variable that must be an integer, or a variable containing an integer. However, the provided variable is not an integer or a valid integer variable."),

            new ErrorInfo(
                135,
                ErrorCode.IndexTooLarge,
                "The provided index '{index}' is greater than the size of the player collection.",
                "This error only occurs in the {INDEXVAR} variable. It occurs when the index provided is larger than the list it is trying to index. As an example, this error will occur if you try to get the 5th player from a variable containing only two players."),

            new ErrorInfo(
                136,
                ErrorCode.CustomCommand_MultCooldowns,
                "Custom command '{0}' ({1}) will not be created because it has multiple cooldowns set to a value other than -1. Only one cooldown can have a value other than -1.",
                "This error only occurs if both the cooldown and player_cooldown settings are set to a value other than -1 in a custom command. Only one cooldown type may have a value of -1."),

            new ErrorInfo(
                137,
                ErrorCode.InvalidNumber,
                "The provided value '{0}' is not a valid number or variable containing a number.",
                "This error occurs when a variable requires a variable that must be a number, or a variable containing a number. However, the provided variable is not a number or a valid number variable."),

            new ErrorInfo(
                138,
                ErrorCode.UnsupportedArgumentVariables,
                "Argument variables are not supported in the '{0}' variable. Please use a custom variable instead.",
                $"This error occurs when a variable expects a variable as one of its arguments. However, the provided variable is a variable with arguments, which is not supported. This error can be resolved by using a custom variable in its place."),

            new ErrorInfo(
                139,
                ErrorCode.ScriptJumpFailed,
                "Failed to jump to {0}, '{1}' value is not a valid label.",
                $"This error occurs when an invalid label or keyword is provided for an action that jumps to labels."),

            new ErrorInfo(
                140,
                ErrorCode.VariableReplaceError,
                "Error replacing the {0} variable: {1}",
                "This error occurs when an error occurred while replacing a variable. Please report this error in the Scripted Events Discord server."),

            new ErrorInfo(
                141,
                ErrorCode.UnknownActionError,
                "Ran into an error while running '{0}' action (please report to developer)",
                "This error occurs when there was an unexpected error in an action. Please report this error in the Scripted Events Discord server."),

            new ErrorInfo(
                142,
                ErrorCode.InvalidDoor,
                "'{0}' is not a valid door input.",
                "This error occurs when an input to target in-game doors was expected, but the input was invalid. This error can be resolved by using a proper door input, such as 'ALL', a ZoneType/RoomType, or the name/ID of a door."),

            new ErrorInfo(
                143,
                ErrorCode.InvalidLift,
                "'{0} is not a valid lift input.",
                "This error occurs when an input to target in-game lift was expected, but the input was invalid. This error can be resolved by using a proper lift input, such as 'ALL' or the name/ID of a door."),

            new ErrorInfo(
                144,
                ErrorCode.InvalidEnumGeneric,
                "Provided value '{0}' is not a valid {1} input. See all options by running 'shelp {1}' in the server console.",
                "This error occurs when an input to a specific enum was expected, but the input was invalid. This error can be resolved by using a valid enum input. All valid enum inputs can be seen by running 'shelp <name of the enum here>' in the server console."),

            new ErrorInfo(
                145,
                ErrorCode.InvalidStringVariable,
                "Provided variable '{0}' is not a valid string variable.",
                "This error occurs when a string variable was expected, but is not provided. This error can be resolved by providing a valid string variable."),

            new ErrorInfo(
                146,
                ErrorCode.InvalidCharacter,
                "Provided value '{0}' is not a valid character.",
                "This error occurs when a single character input was expected, but the input is not a valid character. This error can be resolved by providing a valid character."),

            new ErrorInfo(
                147,
                ErrorCode.InvalidRoleTypeOrTeam,
                "Provided value '{0}' is not a valid RoleTypeId or Team.",
                $"This error occurs when a RoleTypeId or Team input was expected, but the input is not either. This error can be resolved by providing a valid RoleTypeId or Team. A full list of valid RoleTypeId IDs (as of {DateTime.Now:g}) follows:\n{string.Join("\n", ((RoleTypeId[])Enum.GetValues(typeof(RoleTypeId))).Where(r => r is not RoleTypeId.None).Select(r => $"- [{r:d}] {r}"))}\n A full list of valid Team IDs (as of {DateTime.Now:g}) follows:\n{string.Join("\n", ((Team[])Enum.GetValues(typeof(Team))).Select(r => $"- [{r:d}] {r}"))}"),

            new ErrorInfo(
                148,
                ErrorCode.InvalidBoolean,
                "Provided value '{0}' is not a valid boolean input.",
                "This error occurs when a single boolean input was expected, but the input is not a valid boolean. This error can be resolved by providing a valid boolean input. Valid inputs are as follows: 'TRUE', 'T', 'FALSE', 'F', 'YES', 'Y', 'NO', 'N'."),
        }.AsReadOnly();
    }
}
