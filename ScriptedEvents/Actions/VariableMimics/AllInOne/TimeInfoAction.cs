namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    /// <inheritdoc/>
    public class TimeAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "TIMEINFO";

        /// <inheritdoc/>
        public string Description => "All-in-one action for getting time related information.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("TICKS", "Returns the amount of seconds since 1970."),
                new("SECOND", "Returns a number in range 0-59"),
                new("MINUTE", "Returns a number in range 0-59"),
                new("HOUR", "Returns a number in range 0-23"),
                new("MONTH", "Returns a number in range 1-12"),
                new("YEAR", "Returns the amount of years since the birth of Christ"),
                new("DAYOFWEEK", "Returns a number in range 1-7 (Warning! This follows the US system, where Sunday is the first day of the week)"),
                new("DAYOFMONTH", "Returns a number in range 0-31"),
                new("DAYOFYEAR", "Returns a number in range 0-366")),
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