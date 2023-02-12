namespace ScriptedEvents.Structures
{
    using ScriptedEvents.API.Enums;

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
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the execution of the action was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets message to show (or an error message if <see cref="Success"/> is <see langword="false"/>).
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets flags that control what happens after the execution is complete.
        /// </summary>
        public ActionFlags ResponseFlags { get; set; }
    }
}
