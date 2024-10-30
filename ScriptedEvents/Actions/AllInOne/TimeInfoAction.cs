namespace ScriptedEvents.Actions.AllInOne
{
    using System;

    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Enums;
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
                new OptionValueDepending("Second", "Current second, in range 0-59.", typeof(int)),
                new OptionValueDepending("Minute", "Current minute, in range 0-59.", typeof(int)),
                new OptionValueDepending("Hour", "Current hour, in range 0-23.", typeof(int)),
                new OptionValueDepending("Month", "Current month, in range 1-12.", typeof(int)),
                new OptionValueDepending("Year", "Current year.", typeof(int)),
                new OptionValueDepending("DayOfWeek", "Current day of week, in range 1-7, where 1 is Monday and 7 is Sunday.", typeof(int)),
                new OptionValueDepending("DayOfMonth", "Current day of month, in range 0-31.", typeof(int)),
                new OptionValueDepending("DayOfYear", "Current day of year, in range 0-366.", typeof(int))),
        };

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

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

            string ret = Arguments[0]!.ToUpper() switch
            {
                "TICK" => ((long)(DateTime.Now - MainPlugin.Epoch).TotalSeconds).ToString(),
                "SECOND" => Format(DateTime.Now.Second),
                "MINUTE" => Format(DateTime.Now.Minute),
                "HOUR" => Format(DateTime.Now.Hour),
                "MONTH" => DateTime.Now.Month.ToString(),
                "YEAR" => DateTime.Now.Year.ToString(),
                "DAYOFWEEK" => ((((int)DateTime.Now.DayOfWeek + 6) % 7) + 1).ToString(),
                "DAYOFMONTH" => DateTime.Now.Day.ToString(),
                "DAYOFYEAR" => DateTime.Now.DayOfYear.ToString(),
                _ => throw new ArgumentException(),
            };

            return new(true, new(ret));
        }
    }
}