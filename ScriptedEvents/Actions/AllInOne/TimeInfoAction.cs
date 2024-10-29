namespace ScriptedEvents.Actions.AllInOne
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Interfaces;
    using ScriptedEvents.Structures;

    public class TimeInfoAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "TimeInfo";

        /// <inheritdoc/>
        public string Description => "All-in-one action for getting current time related information.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new OptionValueDepending("Ticks", "Amount of seconds since 1970.", typeof(long)),
                new OptionValueDepending("Second", "Returns a number in range 0-59"),
                new OptionValueDepending("Minute", "Returns a number in range 0-59"),
                new OptionValueDepending("Hour", "Returns a number in range 0-23"),
                new OptionValueDepending("Month", "Returns a number in range 1-12"),
                new OptionValueDepending("Year", "Returns the amount of years since the birth of Christ"),
                new OptionValueDepending("DayOfWeek", "Returns a number in range 1-7 (Warning! This follows the US system, where Sunday is the first day of the week)"),
                new OptionValueDepending("DayOfMonth", "Returns a number in range 0-31"),
                new OptionValueDepending("DayOfYear", "Returns a number in range 0-366")),
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