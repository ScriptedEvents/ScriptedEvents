namespace ScriptedEvents.Variables.WorldTime
{
#pragma warning disable SA1402 // File may only contain a single type
    using System;

    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class WorldTimeVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "World Time";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new Time(),
        };
    }

    public class Time : IFloatVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{TIME}";

        /// <inheritdoc/>
        public string Description => "All-in-one variable for time related information. All time information is based on the UTC timezone.";

        public string[] RawArguments { get; set; }

        public object[] Arguments { get; set; }

        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("TICK", "The amount of seconds since 1.1.1970"),
                new("SECOND", "0-59"),
                new("MINUTE", "0-59"),
                new("HOUR", "0-23"),
                new("YEAR", "The amount of years since the birth of Christ"),
                new("DAYOFWEEK", "1-7 (Warning! This follows the US system, where Saturday is the first day of the week)"),
                new("DAYOFMONTH", "0-31"),
                new("DAYOFYEAR", "0-366"),
                new("ROUNDMINUTES", "The amount of elapsed round time, in minutes."),
                new("ROUNDSECONDS", "The amount of elapsed round time, in seconds."),
                new("ROUNDSTART", "The amount of time remaining before the round starts. -1 if round already started.")),
        };

        /// <inheritdoc/>
        public float Value
        {
            get
            {
                string mode = Arguments[0].ToString().ToUpper();
                return mode switch
                {
                    "TICK" => (long)(DateTime.UtcNow - MainPlugin.Epoch).TotalSeconds,
                    "SECOND" => DateTime.UtcNow.Second,
                    "MINUTE" => DateTime.UtcNow.Minute,
                    "HOUR" => DateTime.UtcNow.Hour,
                    "YEAR" => DateTime.UtcNow.Year,
                    "DAYOFWEEK" => ((int)DateTime.UtcNow.DayOfWeek) + 1,
                    "DAYOFMONTH" => DateTime.UtcNow.Day,
                    "DAYOFYEAR" => DateTime.UtcNow.DayOfYear,
                    "ROUNDMINUTES" => (float)Exiled.API.Features.Round.ElapsedTime.TotalMinutes,
                    "ROUNDSECONDS" => (float)Exiled.API.Features.Round.ElapsedTime.TotalSeconds,
                    "ROUNDSTART" => Exiled.API.Features.Round.LobbyWaitingTime,
                    _ => throw new ArgumentException($"Provided mode '{mode}' is incorrect"),
                };
            }
        }
    }
}
