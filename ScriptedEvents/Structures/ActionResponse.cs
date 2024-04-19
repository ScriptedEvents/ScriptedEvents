namespace ScriptedEvents.Structures
{
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;

    /// <summary>
    /// Represents a response to an action execution.
    /// </summary>
    public class ActionResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionResponse"/> class.
        /// </summary>
        /// <param name="success">Whether or not the execution of the action was successful.</param>
        /// <param name="message">Message to show (or an error message if <see cref="Success"/> is <see langword="false"/>).</param>
        /// <param name="flags">Flags that control what happens after the execution is complete.</param>
        public ActionResponse(bool success, string message = "", ActionFlags flags = ActionFlags.None)
        {
            Success = success;
            Message = message;
            ResponseFlags = flags;
            MessageType = Success ? MessageType.OK : MessageType.Custom;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionResponse"/> class.
        /// </summary>
        /// <param name="responseType">The <see cref="MessageType"/>.</param>
        /// <param name="action">The action creating the response. Usually <see langword="this"/>.</param>
        /// <param name="paramName">The name of the parameter having a problem.</param>
        /// <param name="arguments">Additional arguments. See remarks.</param>
        /// <remarks>
        /// This section details each <see cref="MessageType"/> and the arguments it expects.
        /// <see cref="MessageType.OK"/> - Doesn't need anything. Doesn't need parameter name.<br />
        /// <see cref="MessageType.GeneratorError"/> - Doesn't need anything. Doesn't need parameter name.<br />
        /// <see cref="MessageType.InvalidUsage"/> - Expects a List of <see cref="Argument"/>. Doesn't need parameter name.<br />
        /// <see cref="MessageType.NotANumber"/> - Expects the provided string that is not a number.<br />
        /// <see cref="MessageType.NotANumberOrCondition"/> - Expects a string formula and the <see cref="MathResult"/>.<br />
        /// <see cref="MessageType.LessThanZeroNumber"/> - Expects the provided number that is negative.<br />
        /// <see cref="MessageType.InvalidRole"/> - Expects the provided string. <br />
        /// <see cref="MessageType.NoPlayersFound"/> - Expects only parameter name. <br />
        /// <see cref="MessageType.CassieCaptionNoAnnouncement"/> - Expects only parameter name. <br />
        /// <see cref="MessageType.Custom"/> - Doesn't need anything. Doesn't need parameter name. <br />
        /// </remarks>
        public ActionResponse(MessageType responseType, IAction action, string paramName, params object[] arguments)
        {
            Success = responseType is MessageType.OK;
            MessageType = responseType;
            Message = MsgGen.Generate(responseType, action, paramName, arguments);
        }

        /// <summary>
        /// Gets a value indicating whether or not the execution of the action was successful.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Gets message to show (or an error message if <see cref="Success"/> is <see langword="false"/>).
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the <see cref="MessageType"/> of the response.
        /// </summary>
        public MessageType MessageType { get; }

        /// <summary>
        /// Gets flags that control what happens after the execution is complete.
        /// </summary>
        public ActionFlags ResponseFlags { get; }
    }
}
