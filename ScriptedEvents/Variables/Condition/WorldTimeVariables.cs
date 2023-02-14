namespace ScriptedEvents.Variables.Condition.WorldTime
{
    using System;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Variables.Interfaces;

    internal class WorldTimeVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public VariableGroupType GroupType => VariableGroupType.Condition;

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new DayOfWeek(),
            new DayOfMonth(),
            new DayOfYear(),
            new Month(),
            new Year(),
        };
    }

    public class DayOfWeek : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{DAYOFWEEK}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public float Value => ((int)DateTime.UtcNow.DayOfWeek) + 1;
    }

    public class DayOfMonth : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{DAYOFMONTH}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public float Value => DateTime.UtcNow.Day;
    }

    public class DayOfYear : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{DAYOFYEAR}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public float Value => DateTime.UtcNow.DayOfYear;
    }

    public class Month : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{MONTH}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public float Value => DateTime.UtcNow.Month;
    }

    public class Year : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{YEAR}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public float Value => DateTime.UtcNow.Year;
    }
}
