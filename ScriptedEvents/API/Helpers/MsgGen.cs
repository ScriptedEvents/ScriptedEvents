namespace ScriptedEvents.API.Helpers
{
    using Exiled.API.Features.Pools;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;
    using System.Text;

    /// <summary>
    /// Tool to generate error messages, for consistency between all actions.
    /// </summary>
    public static class MsgGen
    {
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
                case MessageType.InvalidOption when arguments[0] is string options:
                    return $"Parameter '{arguments[0]}' expects one of the following options: {options}.";
                case MessageType.NotANumber when arguments[0] is string number:
                    return $"Invalid number '{number}' provided for the '{paramName}' parameter.";
                case MessageType.NotANumberOrCondition when arguments[0] is string formula && arguments[1] is MathResult result:
                    return $"Invalid {paramName} condition provided! Condition: {formula} Error type: '{result.Exception.GetType().Name}' Message: '{result.Message}'.";
                case MessageType.LessThanZeroNumber when arguments[0] is string number:
                    return $"Negative number '{number}' cannot be used in the '{paramName}' parameter.";
                case MessageType.NoPlayersFound:
                    return $"No players were found matching the given criteria ('{paramName}' parameter).";
                case MessageType.CassieCaptionNoAnnouncement:
                    return $"Cannot show captions without a corresponding message.";
            }

            return "Unknown error";
        }
    }
}
