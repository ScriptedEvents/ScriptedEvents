namespace ScriptedEvents.Structures
{
    using System.Collections.Generic;
    using System.Linq;

    public class ErrorTrace
    {
        public ErrorTrace()
        {
            Errors = new List<ErrorInfo>();
        }

        public ErrorTrace(List<ErrorInfo> errors)
        {
            Errors = errors;
        }

        public ErrorTrace(ErrorInfo error)
        {
            Errors = new[] { error }.ToList();
        }

        /// <summary>
        /// Gets or sets the stack of errors.
        /// </summary>
        public List<ErrorInfo> Errors { get; set; }

        /// <summary>
        /// Gets the script.
        /// </summary>
        public Script Script { get; private set; }
    }

    public static class ErrorTraceExtensions
    {
        public static ErrorTrace Append(this ErrorTrace trace, ErrorInfo error)
        {
            trace.Errors.Add(error);
            return trace;
        }

        public static void Append(this ErrorTrace trace, IEnumerable<ErrorInfo> errors)
        {
            trace.Errors.AddRange(errors);
        }

        public static string Format(this ErrorTrace trace)
        {
            string msg;
            var errors = trace.Errors.ToArray();

            for (int i = 0; i < errors.Length; i++)
            {
                var temp = errors[i];
                if (temp.Name.EndsWith("."))
                {
                    temp.Name = temp.Name.Substring(0, temp.Name.Length - 1);
                }
                errors[i] = temp;
            }

            if (errors.Length == 0)
            {
                return "No errors to display. If you see this, report this to the developers of Scripted Events.";
            }

            var firstError = errors.First();
            errors = errors.Skip(1).ToArray();

            msg = $"{firstError.Name}: [Initial error] [Source: {firstError.Source}] \n    {firstError.Description}";

            return errors.Aggregate(msg, (current, err) => $"{err.Name}: [Source: {err.Source}] \n    {err.Description}\n\n{current}");
        }
    }
}