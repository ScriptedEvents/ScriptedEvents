namespace ScriptedEvents.Variables.WorldTime
{
#pragma warning disable SA1402 // File may only contain a single type
    using System;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Variables.Interfaces;

    public class WorldTimeVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "World Time";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new DayOfWeek(),
            new DayOfMonth(),
            new DayOfYear(),
            new Month(),
            new Year(),
            new Tick(),
            new Hour(),
        };
    }

    public class DayOfWeek : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{DAYOFWEEK}";

        /// <inheritdoc/>
        public string Description => "The current real-world day of the week, from 1-7, in UTC time.";

        /// <inheritdoc/>
        public float Value => ((int)DateTime.UtcNow.DayOfWeek) + 1;
    }

    public class DayOfMonth : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{DAYOFMONTH}";

        /// <inheritdoc/>
        public string Description => "The current real-world day of the month, from 1-31, in UTC time.";

        /// <inheritdoc/>
        public float Value => DateTime.UtcNow.Day;
    }

    public class DayOfYear : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{DAYOFYEAR}";

        /// <inheritdoc/>
        public string Description => "The current real-world day of the year, from 1-366, in UTC time.";

        /// <inheritdoc/>
        public float Value => DateTime.UtcNow.DayOfYear;
    }

    public class Month : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{MONTH}";

        /// <inheritdoc/>
        public string Description => "The current real-world month, from 1-12, in UTC time.";

        /// <inheritdoc/>
        public float Value => DateTime.UtcNow.Month;
    }

    public class Year : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{YEAR}";

        /// <inheritdoc/>
        public string Description => "The current real-world year, from 1-9999, in UTC time.";

        /// <inheritdoc/>
        public float Value => DateTime.UtcNow.Year;
    }

    public class Tick : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{TICK}";

        /// <inheritdoc/>
        public string Description => $"The amount of seconds since {new DateTime(1970, 1, 1):f}.";

        /// <inheritdoc/>
        public float Value => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public class Hour : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{HOUR}";

        /// <inheritdoc/>
        public string Description => $"The current real-world hour, from 0-23.";

        /// <inheritdoc/>
        public float Value => DateTime.UtcNow.Hour;
    }
}
