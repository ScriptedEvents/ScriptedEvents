namespace ScriptedEvents.Variables.WorldTime
{
#pragma warning disable SA1402 // File may only contain a single type
    using System;

    using ScriptedEvents.API.Interfaces;
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

    public class Time : IFloatVariable, IArgumentVariable, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "{TIME}";

        /// <inheritdoc/>
        public string Description => "All-in-one variable for time related information.";

        public string[] RawArguments { get; set; }

        public object[] Arguments { get; set; }

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("mode", typeof(string), "See long description for modes.", true),
        };

        public string LongDescription => $@"See all available modes with respective outputs below.
All time information is based on the UTC time zone.

TICK: The amount of seconds since {new DateTime(1970, 1, 1):f}.
SECOND: 0-59.
MINUTE: 0-59. 
HOUR: 0-23.
YEAR: The amount of years since the birth of Christ.
DAYOFWEEK: 1-7 (This follows the US system, where Saturday is the first day of the week).
DAYOFMONTH: 1-31.
DAYOFYEAR: 1-366.";

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
                    _ => throw new ArgumentException($"Provided mode '{mode}' is incorrect"),
                };
            }
        }
    }
}
