namespace ScriptedEvents.Structures
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains the result of a call to <see cref="API.Features.ArgumentProcessor.Process(Argument[], string[], API.Interfaces.IScriptComponent, Script, bool)"/>.
    /// </summary>
    public class ArgumentProcessResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentProcessResult"/> class.
        /// </summary>
        /// <param name="success">Is action permitted to run.</param>
        /// <param name="errored">Did an error occur while processing.</param>
        /// <param name="argument">If unsuccessful, which argument failed.</param>
        /// <param name="message">The error message.</param>
        public ArgumentProcessResult(bool success, bool errored = false, string argument = "", string message = "")
        {
            Success = success;
            Errored = errored;
            FailedArgument = argument;
            Message = message;

            NewParameters = new();
        }

        /// <summary>
        /// Gets a value indicating whether or not the processing was successful.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Gets a value indicating whether or not the processing was successful.
        /// </summary>
        public bool Errored { get; }

        /// <summary>
        /// Gets a value indicating the name of the failed argument, if unsuccessful.
        /// </summary>
        public string FailedArgument { get; }

        /// <summary>
        /// Gets a value indicating the message, if unsuccessful.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets an <see cref="object"/> list of new parameters if the processing was successful.
        /// </summary>
        public List<object> NewParameters { get; }
    }
}
