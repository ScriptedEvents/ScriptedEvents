namespace ScriptedEvents.Structures
{
    using System.Collections.Generic;

    using ScriptedEvents.API.Features;

    /// <summary>
    /// Contains the result of a call to <see cref="ArgumentProcessor.ProcessActionArguments"/>.
    /// </summary>
    public class ArgumentProcessResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentProcessResult"/> class.
        /// </summary>
        /// <param name="shouldExecute">Is action permitted to run.</param>
        /// <param name="errored">Did an error occur while processing.</param>
        /// <param name="errorTrace">The error trace if the processing failed.</param>
        public ArgumentProcessResult(bool shouldExecute, bool errored = false, ErrorTrace? errorTrace = null)
        {
            ShouldExecute = shouldExecute;
            Errored = errored;
            ErrorTrace = errorTrace;

            NewParameters = new();
        }

        /// <summary>
        /// Gets a value indicating whether or not the action should execute based on the result.
        /// </summary>
        public bool ShouldExecute { get; }

        /// <summary>
        /// Gets a value indicating whether or not the processing was successful.
        /// </summary>
        public bool Errored { get; }

        /// <summary>
        /// Gets a value indicating the message, if unsuccessful.
        /// </summary>
        public ErrorTrace? ErrorTrace { get; }

        /// <summary>
        /// Gets an <see cref="object"/> list of new parameters if the processing was successful.
        /// </summary>
        public List<object?> NewParameters { get; }

        /// <summary>
        /// Gets or sets the raw arguments of the action, with filtered out action decorators.
        /// </summary>
        public string[] StrippedRawParameters { get; set; }
    }
}
