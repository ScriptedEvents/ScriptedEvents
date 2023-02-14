namespace ScriptedEvents.API.Helpers
{
    using System.Text;

    using Exiled.API.Features.Pools;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;

    /// <summary>
    /// Tool to generate error messages, for consistency between all actions.
    /// </summary>
    public static class MsgGen
    {
        /// <summary>
        /// Generates an error message, based on provided input.
        /// </summary>
        /// <param name="type">The type of message to show.</param>
        /// <param name="action">The action currently executing.</param>
        /// <param name="paramName">The name of the parameter that is causing a skill issue.</param>
        /// <param name="arguments">The arguments of the MessageType. See <see cref="ActionResponse.ActionResponse(MessageType, IAction, string, object[])"/> for documentation on what MessageTypes require what arguments.</param>
        /// <returns>The string to display to the user.</returns>
        public static string Generate(MessageType type, IAction action, string paramName, params object[] arguments)
        {
            switch (type)
            {
                case MessageType.InvalidUsage when arguments[0] is Argument[] argList:
                    StringBuilder sb = StringBuilderPool.Pool.Get();
                    foreach (Argument arg in argList)
                    {
                        string[] chars = arg.Required ? new[] { "<", ">" } : new[] { "[", "]" };
                        sb.Append($" {chars[0]}{arg.ArgumentName}{chars[1]}");
                    }

                    return $"Invalid command usage. Usage: {action.Name}{StringBuilderPool.Pool.ToStringReturn(sb)}";

                case MessageType.InvalidUsage:
                    return "Invalid command usage.";

                case MessageType.InvalidOption when arguments[0] is string input && arguments[1] is string options:
                    return $"Invalid option {input} provided for the '{paramName}' parameter of the {action.Name} action. This parameter expects one of the following options: {options}.";

                case MessageType.NotANumber when arguments[0] is not null:
                    return $"Invalid number '{arguments[0]}' provided for the '{paramName}' parameter of the {action.Name} action.";

                case MessageType.NotANumberOrCondition when arguments[0] is not null && arguments[1] is MathResult result:
                    return $"Invalid {paramName} condition provided in the {action.Name} action! Condition: {arguments[0]} Error type: '{result.Exception.GetType().Name}' Message: '{result.Message}'.";

                case MessageType.LessThanZeroNumber when arguments[0] is not null:
                    return $"Negative number '{arguments[0]}' cannot be used in the '{paramName}' parameter of the {action.Name} action.";

                case MessageType.InvalidRole when arguments[0] is not null:
                    return $"Invalid {paramName} provided in the {action.Name} action. '{arguments[0]}' is not a valid RoleType.";

                case MessageType.NoPlayersFound:
                    return $"No players were found matching the given criteria ('{paramName}' parameter).";

                case MessageType.CassieCaptionNoAnnouncement:
                    return $"Cannot show captions without a corresponding CASSIE announcement.";
            }

            return "Unknown error";
        }
    }
}
