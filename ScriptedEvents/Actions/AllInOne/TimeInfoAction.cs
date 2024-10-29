using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;

    /// <inheritdoc/>
    public class TimeInfoAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "TIMEINFO";

        /// <inheritdoc/>
        public string Description => "All-in-one action for getting current time related information.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("Ticks", "Returns the amount of seconds since 1970."),
                new("Second", "Returns a number in range 0-59"),
                new("Minute", "Returns a number in range 0-59"),
                new("Hour", "Returns a number in range 0-23"),
                new("Month", "Returns a number in range 1-12"),
                new("Year", "Returns the amount of years since the birth of Christ"),
                new("DayOfWeek", "Returns a number in range 1-7 (Warning! This follows the US system, where Sunday is the first day of the week)"),
                new("DayOfMonth", "Returns a number in range 0-31"),
                new("DayOfYear", "Returns a number in range 0-366")),
        };

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.AllInOneInfo;

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            static string Format(object input)
            {
                string str = input.ToString();
                return str.Length < 2
                    ? "0" + str
                    : str;
            }

            string ret = Arguments[0].ToString().ToUpper() switch
            {
                "TICK" => ((long)(DateTime.Now - MainPlugin.Epoch).TotalSeconds).ToString(),
                "SECOND" => Format(DateTime.Now.Second),
                "MINUTE" => Format(DateTime.Now.Minute),
                "HOUR" => Format(DateTime.Now.Hour),
                "MONTH" => DateTime.Now.Month.ToString(),
                "YEAR" => DateTime.Now.Year.ToString(),
                "DAYOFWEEK" => (((int)DateTime.Now.DayOfWeek) + 1).ToString(),
                "DAYOFMONTH" => DateTime.Now.Day.ToString(),
                "DAYOFYEAR" => DateTime.Now.DayOfYear.ToString(),
                _ => throw new ArgumentException(),
            };

            return new(true, variablesToRet: new[] { ret });
        }
    }
}