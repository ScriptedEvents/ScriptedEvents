namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class TimeAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "TIME";

        /// <inheritdoc/>
        public string Description => "All-in-one action for getting time related information.";

        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("TICK", "The amount of seconds since 1970."),
                new("SECOND", "In range 0-59"),
                new("MINUTE", "In range 0-59"),
                new("HOUR", "In range 0-23"),
                new("MONTH", "In range 1-12"),
                new("YEAR", "The amount of years since the birth of Christ"),
                new("DAYOFWEEK", "1-7 (Warning! This follows the US system, where Sunday is the first day of the week)"),
                new("DAYOFMONTH", "0-31"),
                new("DAYOFYEAR", "0-366"),
                new("ROUNDMINUTES", "The amount of elapsed round time, in minutes."),
                new("ROUNDSECONDS", "The amount of elapsed round time, in seconds."),
                new("ROUNDSTART", "The amount of time remaining before the round starts. -1 if round already started.")),
        };

        public string[] RawArguments { get; set; }

        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.MapInfo;

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string ret = Arguments[0].ToString().ToUpper() switch
            {
                "TICK" => ((long)(DateTime.UtcNow - MainPlugin.Epoch).TotalSeconds).ToString(),
                "SECOND" => DateTime.UtcNow.Second.ToString(),
                "MINUTE" => DateTime.UtcNow.Minute.ToString(),
                "HOUR" => DateTime.UtcNow.Hour.ToString(),
                "MONTH" => DateTime.UtcNow.Month.ToString(),
                "YEAR" => DateTime.UtcNow.Year.ToString(),
                "DAYOFWEEK" => (((int)DateTime.UtcNow.DayOfWeek) + 1).ToString(),
                "DAYOFMONTH" => DateTime.UtcNow.Day.ToString(),
                "DAYOFYEAR" => DateTime.UtcNow.DayOfYear.ToString(),
                "ROUNDMINUTES" => ((float)Round.ElapsedTime.TotalMinutes).ToString(),
                "ROUNDSECONDS" => ((float)Round.ElapsedTime.TotalSeconds).ToString(),
                "ROUNDSTART" => Round.LobbyWaitingTime.ToString(),
                _ => throw new ArgumentException(),
            };

            return new(true, variablesToRet: new[] { ret });
        }
    }
}