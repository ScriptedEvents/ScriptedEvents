namespace ScriptedEvents.Variables.Misc
{
#pragma warning disable SA1402 // File may only contain a single type
    using System;

    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;
    using UnityEngine;

    public class MiscVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Miscellaneous";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new Round(),
            new This(),
        };
    }

    public class Round : IFloatVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{ROUND}";

        /// <inheritdoc/>
        public string Description => "Returns a rounded number.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
                new Argument("variable", typeof(float), "The name of the variable to round. Requires the variable to be a number.", true),
                new Argument("mode", typeof(string), "Way of rounding the variable, either UP or DOWN.", true),
        };

        /// <inheritdoc/>
        public float Value
        {
            get
            {
                float value = (float)Arguments[0];
                string mode = Arguments.Length < 2 ? "UP" : (string)Arguments[1];

                return mode.ToUpper() switch
                {
                    "UP" => Mathf.Ceil(value),
                    "DOWN" => Mathf.Floor(value),
                    _ => throw new ArgumentException($"'{mode}' is not a valid mode. Valid options: UP, DOWN"),
                };
            }
        }
    }

    public class This : IStringVariable, IArgumentVariable, INeedSourceVariable
    {
        /// <inheritdoc/>
        public string Name => "{THIS}";

        /// <inheritdoc/>
        public string Description => "Returns the script name.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments { get; set; }

        /// <inheritdoc/>
        public Script Source { get; set; }

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                return Source.ScriptName;
            }
        }
    }
#pragma warning restore SA1402 // File may only contain a single type.
}
